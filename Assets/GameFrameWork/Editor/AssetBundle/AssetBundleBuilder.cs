using UnityEditor;
using System.IO;
using System.Collections.Generic;
using GameFrameWork.Utilities;
using GameFrameWork.Serialize;

namespace GameFrameWork.Editor
{
    public class AssetBundleBuilder
    {
        /// <summary>
        /// 打包
        /// </summary>
        public static void Build(BuildTarget target, bool isShowNotify = true)
        {
            AssetBundleConfig config = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.assetBundleDataPath);

            FileUitl.DeleteDirectory(config.AssetBuildFullDir);
            FileUitl.VerifyDirectory(config.AssetBuildFullDir);
            AssetDatabase.Refresh();
            m_BuildMaps.Clear();

            if (AppConfig.instance.useLua)
            {
                if (AppConfig.instance.loadLuaAB) HandleLuaBundle();
                else HandleLuaFile();
            }

            if (GenerateBuildMap(config))
            {
                BuildPipeline.BuildAssetBundles(config.AssetBuildDir, m_BuildMaps.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, target);
                BuildFileIndex();

                if (AppConfig.instance.useLua)
                {
                    FileUitl.DeleteDirectory(EditorPathUtil.luaPath);
                }

                AssetDatabase.Refresh();

                if (config.IsCopyAsset)
                {
                    FileUitl.DeleteDirectory(config.AssetCopyDir);
                    FileUitl.CopyDirectory(config.AssetBuildFullDir, config.AssetCopyDir);
                }

                AssetDatabase.Refresh();

                if (!isShowNotify)
                {
                    return;
                }

                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(config.AssetBuildDir);
                UnityEditor.EditorUtility.DisplayDialog("提示", "打包成功", "确定");
            }
        }

        /// <summary>
        /// 生成打包列表
        /// </summary>
        private static bool GenerateBuildMap(AssetBundleConfig config)
        {
            for (int i = 0; i < config.Datas.Count; i++)
            {
                if (config.Datas[i].BundleType == AssetBundleData.AssetType.MapSingle)
                {
                    bool result = AddBuildMapSingle(config.Datas[i].Pattern, config.Datas[i].AssetPath, config.Datas[i].AssetBundlePath, config.Datas[i].BundleExtend);
                    if (!result) return false;
                }
                else
                {
                    bool result =  AddBuildMap(config.Datas[i].BundleName + config.Datas[i].BundleExtend, config.Datas[i].Pattern, config.Datas[i].AssetPath);
                    if (!result) return false;
                }
            }

            return true;
            //AddBuildMap("fonts.unity3d", "*.TTF", "Assets/AssetsLibrary/Font");
            //AddBuildMapSingle("*.prefab", "Assets/AssetsLibrary/UI/Prefabs", "UI/Prefabs/");
        }

        private static bool AddBuildMap(string bundleName, string pattern, string path)
        {
            if (!Directory.Exists(path))
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "资源路径不存在\n" + path, "确定");
                return false;
            }

            string[] files = GetFilesWithoutMetaFile(Directory.GetFiles(path, pattern));

            if (files.Length < 1)
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "该路径下无任何文件\n" + path, "确定");
                return false;
            }

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Replace('\\', '/');
            }

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName;
            build.assetNames = files;
            m_BuildMaps.Add(build);

            return true;
        }

        private static bool AddBuildMapSingle(string pattern, string path, string abPath, string extend)
        {
            if(!Directory.Exists(path))
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "资源路径不存在\n" + path, "确定");
                return false;
            }

            string[] files = GetFilesWithoutMetaFile(Directory.GetFiles(path, pattern));
            if (files.Length < 1)
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "该路径下无任何文件\n"+ path, "确定"); 
                return false;
            }

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Replace('\\', '/');

                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = abPath + Path.GetFileNameWithoutExtension(files[i]) + extend;
                build.assetNames = new string[] { files[i] };
                m_BuildMaps.Add(build);
            }

            return true;
        }

        private static string[] GetFilesWithoutMetaFile(string[] files)
        {
            List<string> fileList = new List<string>();

            if (files != null && files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (Path.GetExtension(files[i]).ToLower() == ".meta")
                    {
                        continue;
                    }

                    fileList.Add(files[i]);
                }
            }

            return fileList.ToArray();
        }

       
        private static void BuildFileIndex()
        {
            ///----------------------创建文件列表-----------------------
            AssetBundleConfig config = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.assetBundleDataPath);
            string versionPath = config.AssetBuildFullDir + "/Version.txt";
            FileUitl.DeleteFile(versionPath);

            m_ListPaths.Clear();
            m_ListFiles.Clear();

            FileUitl.Recursive(config.AssetBuildFullDir, m_ListFiles, m_ListPaths);

            FileStream fileStream = new FileStream(versionPath, FileMode.CreateNew);
            StreamWriter streamWriter = new StreamWriter(fileStream);

            for (int i = 0; i < m_ListFiles.Count; i++)
            {
                if (m_ListFiles[i].EndsWith(".meta") || m_ListFiles[i].Contains(".DS_Store")) continue;

                string md5 = FileUitl.MD5File(m_ListFiles[i]);
                string value = m_ListFiles[i].Replace(config.AssetBuildFullDir, string.Empty);
                string directory = Path.GetDirectoryName(value).Replace("\\", "/");
                string fileName = Path.GetFileNameWithoutExtension(value);          
                string ext = Path.GetExtension(value);
                if (!string.IsNullOrEmpty(directory)) directory += "/";
                streamWriter.Write(directory + fileName + "|" + ext + "|" + md5 + (i < m_ListFiles.Count - 1 ? "\n" : string.Empty));
            }

            streamWriter.Close();
            fileStream.Close();
        }

        #region Lua
        /// <summary>
        /// 处理Lua代码包
        /// </summary>
        private static void HandleLuaBundle()
        {
            //if (!Directory.Exists(AppConfig.GetLuaTempDir())) Directory.CreateDirectory(AppConfig.GetLuaTempDir());

            //string[] srcDirs = { };// CustomSettings.luaDir, CustomSettings.FrameworkPath + "/ToLua/Lua" };
            //for (int i = 0; i < srcDirs.Length; i++) 
            //{
            //    if (AppConfig.Instance.LuaByteMode)
            //    {
            //        string sourceDir = srcDirs[i];
            //        string[] files = Directory.GetFiles(sourceDir, "*.lua", SearchOption.AllDirectories);
            //        int len = sourceDir.Length;

            //        if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\') --len;

            //        for (int j = 0; j < files.Length; j++) 
            //        {
            //            string str = files[j].Remove(0, len);
            //            string dest = streamDir + str + ".bytes";
            //            string dir = Path.GetDirectoryName(dest);
            //            Directory.CreateDirectory(dir);
            //            EncodeLuaFile(files[j], dest);
            //        }
            //    } 
            //    else 
            //    {
            //        //ToLuaMenu.CopyLuaBytesFiles(srcDirs[i], streamDir);
            //    }
            //}

            //string[] dirs = Directory.GetDirectories(streamDir, "*", SearchOption.AllDirectories);
            //for (int i = 0; i < dirs.Length; i++) 
            //{
            //    string name = dirs[i].Replace(streamDir, string.Empty);
            //    name = name.Replace('\\', '_').Replace('/', '_');
            //    //name = "lua/lua_" + name.ToLower() + AppConfig.Instance.ExtName;

            //    string path = "Assets" + dirs[i].Replace(Application.dataPath, "");
            //    AddBuildMap(name, "*.bytes", path);
            //}
            ////AddBuildMap("lua/lua" + AppConfig.Instance.ExtName, "*.bytes", "Assets/" + AppConfig.Instance.LuaTempDir);

            ////-------------------------------处理非Lua文件----------------------------------
            //string luaPath = AppDataPath + "/StreamingAssets/lua/";

            //for (int i = 0; i < srcDirs.Length; i++) 
            //{
            //    paths.Clear(); files.Clear();
            //    string luaDataPath = srcDirs[i].ToLower();
            //    Recursive(luaDataPath);
            //    foreach (string f in files) 
            //    {
            //        if (f.EndsWith(".meta") || f.EndsWith(".lua")) continue;
            //        string newfile = f.Replace(luaDataPath, "");
            //        string path = Path.GetDirectoryName(luaPath + newfile);
            //        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            //        string destfile = path + "/" + Path.GetFileName(f);
            //        File.Copy(f, destfile, true);
            //    }
            //}
            //AssetDatabase.Refresh();
        }

        /// <summary>
        /// 处理Lua文件
        /// </summary>
        private static void HandleLuaFile()
        {
            //string resPath = Utils.EditorPathUtil.GetAssetFullDir();
            //string luaPath = resPath + "lua/";

            ////----------复制Lua文件----------------
            //if (!Directory.Exists(luaPath)) Directory.CreateDirectory(luaPath);

            //string[] luaPaths = {
            //    AppConfig.Ins.LuaDirectory,
            //    //AppDataPath + "/LuaFramework/lua/",
            //    //AppDataPath + "/LuaFramework/Tolua/Lua/" 
            //};

            //for (int i = 0; i < luaPaths.Length; i++)
            //{
            //    paths.Clear(); files.Clear();
            //    string luaDataPath = luaPaths[i].ToLower();
            //    Recursive(luaDataPath);
            //    int n = 0;
            //    foreach (string f in files)
            //    {
            //        if (f.EndsWith(".meta")) continue;
            //        string newfile = f.Replace(luaDataPath, "");
            //        string newpath = luaPath + newfile;
            //        string path = Path.GetDirectoryName(newpath);

            //        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            //        if (File.Exists(newpath)) File.Delete(newpath);

            //        if (AppConfig.Ins.LuaByteMode) EncodeLuaFile(f, newpath);
            //        else File.Copy(f, newpath, true);
            //        UpdateProgress(n++, files.Count, newpath);
            //    }
            //}
            //UnityEditor.EditorUtility.ClearProgressBar();
            //AssetDatabase.Refresh();
        }

        public static void EncodeLuaFile(string srcFile, string outFile)
        {
            //if (!srcFile.ToLower().EndsWith(".lua"))
            //{
            //    File.Copy(srcFile, outFile, true);
            //    return;
            //}

            //bool isWin = true;
            //string luaexe = string.Empty;
            //string args = string.Empty;
            //string exedir = string.Empty;
            //string currDir = Directory.GetCurrentDirectory();
            //if (Application.platform == RuntimePlatform.WindowsEditor)
            //{

            //    isWin = true;
            //    luaexe = "luajit.exe";
            //    args = "-b " + srcFile + " " + outFile;
            //    exedir = Utils.EditorPathUtil.AppDataPath.Replace("assets", "") + "LuaEncoder/luajit/";
            //}
            //else if (Application.platform == RuntimePlatform.OSXEditor)
            //{
            //    isWin = false;
            //    luaexe = "./luajit";
            //    args = "-b " + srcFile + " " + outFile;
            //    exedir = Utils.EditorPathUtil.AppDataPath.Replace("assets", "") + "LuaEncoder/luajit_mac/";
            //}

            //Directory.SetCurrentDirectory(exedir);
            //ProcessStartInfo info = new ProcessStartInfo();
            //info.FileName = luaexe;
            //info.Arguments = args;
            //info.WindowStyle = ProcessWindowStyle.Hidden;
            //info.UseShellExecute = isWin;
            //info.ErrorDialog = true;
            //Log.Debugger.Log(info.FileName + " " + info.Arguments);
            //Process pro = Process.Start(info);
            //pro.WaitForExit();
            //Directory.SetCurrentDirectory(currDir);
        }
        #endregion

        private static List<string> m_ListPaths = new List<string>();
        private static List<string> m_ListFiles = new List<string>();
        private static List<AssetBundleBuild> m_BuildMaps = new List<AssetBundleBuild>();
    }
}