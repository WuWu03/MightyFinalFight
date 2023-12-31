using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GameFrameWork.Utilities
{
    public class FileUtil
    {
        /// <summary>
        /// 读取文本文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string GetFileText(string filePath)
        {
            string content = string.Empty;

            if (!File.Exists(filePath))
            {
                return content;
            }

            using (StreamReader sr = File.OpenText(filePath))
            {
                content = sr.ReadToEnd();
            }

            return content;
        }

        /// <summary>
        /// 创建文本文件
        /// <param name="filePath">文件路径</param>
        /// <param name="content">文本内容</param>
        /// </summary>
        public static void CreateTextFile(string filePath, string content)
        {
            DeleteFile(filePath);

            using (FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(content);
                }
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        public static void CopyDirectory(string sourceDirName, string destDirName)
        {
            try
            {
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                    File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
                }

                if (destDirName[destDirName.Length - 1] != Path.DirectorySeparatorChar)
                {
                    destDirName = destDirName + Path.DirectorySeparatorChar;
                }

                string[] files = Directory.GetFiles(sourceDirName, "*", SearchOption.TopDirectoryOnly);

                foreach (string file in files)
                {
                    if (File.Exists(destDirName + Path.GetFileName(file)))
                    {
                        continue;
                    }

                    FileInfo fileInfo = new FileInfo(file);

                    if (fileInfo.Extension.Equals(".meta", StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }

                    File.Copy(file, destDirName + Path.GetFileName(file), true);
                    File.SetAttributes(destDirName + Path.GetFileName(file), FileAttributes.Normal);
                }

                string[] dirs = Directory.GetDirectories(sourceDirName);
                foreach (string dir in dirs)
                {
                    CopyDirectory(dir, destDirName + Path.GetFileName(dir));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 遍历目录及其子目录
        /// </summary>
        public static void Recursive(string path, List<string> listFiles, List<string> listPaths)
        {
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            foreach (string filename in files)
            {
                string ext = Path.GetExtension(filename);

                if (ext.Equals(".meta") || ext.Equals(".DS_Store"))
                {
                    continue;
                }

                listFiles.Add(filename.Replace('\\', '/'));
            }

            foreach (string dir in dirs)
            {
                listPaths.Add(dir.Replace('\\', '/'));
                Recursive(dir, listFiles, listPaths);
            }
        }

        /// <summary>
        /// 验证路径是否存在
        /// </summary>
        public static bool VerifyDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 删除路径
        /// </summary>
        public static void DeleteDirectory(string directory, bool recursive = true)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive);
            }
        }


        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        public static string MD5File(string file)
        {
            try
            {
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    FileStream fs = new FileStream(file, FileMode.Open);
                    byte[] retVal = md5.ComputeHash(fs);
                    fs.Close();

                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }

                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("md5file() fail, error:" + ex.Message);
            }
        }

        /// <summary>
        /// 计算路径的MD5值
        /// </summary>
        public static string MD5Path(string path)
        {
            string[] files = Directory.GetFiles(path);
            StringBuilder pathMD5SB = new StringBuilder();

            foreach (string filename in files)
            {
                string ext = Path.GetExtension(filename);

                if (ext.Equals(".meta") || ext.Equals(".DS_Store"))
                {
                    continue;
                }

                pathMD5SB.Append(MD5File(filename.Replace('\\', '/')));
            }
            try
            {
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    byte[] retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(pathMD5SB.ToString()));

                    pathMD5SB.Clear();

                    for (int i = 0; i < retVal.Length; i++)
                    {
                        pathMD5SB.Append(retVal[i].ToString("x2"));
                    }

                    return pathMD5SB.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("md5path() fail, error:" + ex.Message);
            }
        }
    }
}