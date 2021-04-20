using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using GameFrameWork.Utils;

namespace GameFrameWork.Editor
{
    public class Packager
    {
        public static string platform = string.Empty;
        static List<string> paths = new List<string>();
        static List<string> files = new List<string>();
        static List<AssetBundleBuild> maps = new List<AssetBundleBuild>();

        ///-----------------------------------------------------------
        static string[] exts = { ".txt", ".xml", ".lua", ".assetbundle", ".json" };
        static bool CanCopy(string ext)
        {   //能不能复制
            foreach (string e in exts)
            {
                if (ext.Equals(e)) return true;
            }
            return false;
        }


        /// <summary>
        /// 打包
        /// </summary>
        public static void Build(BuildTarget target)
        {
            if (Directory.Exists(Utils.PathUtil.StreamingAssetsPath))
                Directory.Delete(Utils.PathUtil.StreamingAssetsPath, true);

            Directory.CreateDirectory(Utils.PathUtil.StreamingAssetsPath);
            AssetDatabase.Refresh();

            maps.Clear();

            if (AppConfig.Ins.UseLua)
            {
                if (AppConfig.Ins.LoadLuaAB) HandleLuaBundle();
                else HandleLuaFile();
            }

            if (GenerateBuildMap())
            {
                BuildPipeline.BuildAssetBundles(PathUtil.StreamingAssetsPath, maps.ToArray(), BuildAssetBundleOptions.None, target);
                BuildFileIndex();

                if (Directory.Exists(Utils.PathUtil.GetLuaTempDir())) Directory.Delete(Utils.PathUtil.GetLuaTempDir(), true);
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 生产打包列表
        /// </summary>
        private static bool GenerateBuildMap()
        {
            AssetBundleConfig config = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(PathUtil.AssetBundleConfig);
            for (int i = 0; i < config.Datas.Length; i++)
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
                EditorUtility.DisplayDialog("错误", "资源路径不存在", "确定");
                return false;
            }

            string[] files = Directory.GetFiles(path, pattern);
            if (files.Length == 0)
            {
                EditorUtility.DisplayDialog("错误", "该路径下无任何文件", "确定");
                return false;
            }

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Replace('\\', '/');
            }

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName;
            build.assetNames = files;
            maps.Add(build);
            return true;
        }

        private static bool AddBuildMapSingle(string pattern, string path, string abPath, string extend)
        {
            if(!Directory.Exists(path))
            {
                EditorUtility.DisplayDialog("错误", "资源路径不存在", "确定");
                return false;
            }

            string[] files = Directory.GetFiles(path, pattern);
            if (files.Length == 0)
            {
                EditorUtility.DisplayDialog("错误", "该路径下无任何文件", "确定");
                return false;
            }

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Replace('\\', '/');

                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = abPath + Path.GetFileNameWithoutExtension(files[i]) + extend;
                build.assetNames = new string[] { files[i] };
                maps.Add(build);
            }

            return true;
        }

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
            string resPath = Utils.PathUtil.GetAssetFullDir();
            string luaPath = resPath + "lua/";

            //----------复制Lua文件----------------
            if (!Directory.Exists(luaPath)) Directory.CreateDirectory(luaPath);

            string[] luaPaths = {
                AppConfig.Ins.LuaDirectory,
                //AppDataPath + "/LuaFramework/lua/",
                //AppDataPath + "/LuaFramework/Tolua/Lua/" 
            };

            for (int i = 0; i < luaPaths.Length; i++)
            {
                paths.Clear(); files.Clear();
                string luaDataPath = luaPaths[i].ToLower();
                Recursive(luaDataPath);
                int n = 0;
                foreach (string f in files)
                {
                    if (f.EndsWith(".meta")) continue;
                    string newfile = f.Replace(luaDataPath, "");
                    string newpath = luaPath + newfile;
                    string path = Path.GetDirectoryName(newpath);

                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    if (File.Exists(newpath)) File.Delete(newpath);

                    if (AppConfig.Ins.LuaByteMode) EncodeLuaFile(f, newpath);
                    else File.Copy(f, newpath, true);
                    UpdateProgress(n++, files.Count, newpath);
                }
            }
            UnityEditor.EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        private static void BuildFileIndex()
        {
            ///----------------------创建文件列表-----------------------
            string versionPath = Utils.PathUtil.GetAssetFullDir() + "/Version.txt";
            if (File.Exists(versionPath)) File.Delete(versionPath);

            paths.Clear();
            files.Clear();
            Recursive(Utils.PathUtil.GetAssetFullDir());

            FileStream fs = new FileStream(versionPath, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];
                string ext = Path.GetExtension(file);

                if (file.EndsWith(".meta") || file.Contains(".DS_Store")) continue;

                string md5 = Utils.Utility.MD5File(file);
                string value = file.Replace(Utils.PathUtil.GetAssetFullDir(), string.Empty);
                sw.WriteLine(value + "|" + md5);
            }
            sw.Close();
            fs.Close();
        }


        /// <summary>
        /// 遍历目录及其子目录
        /// </summary>
        static void Recursive(string path)
        {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string filename in names)
            {
                string ext = Path.GetExtension(filename);
                if (ext.Equals(".meta")) continue;
                files.Add(filename.Replace('\\', '/'));
            }
            foreach (string dir in dirs)
            {
                paths.Add(dir.Replace('\\', '/'));
                Recursive(dir);
            }
        }

        static void UpdateProgress(int progress, int progressMax, string desc)
        {
            string title = "Processing...[" + progress + " - " + progressMax + "]";
            float value = (float)progress / (float)progressMax;
            UnityEditor.EditorUtility.DisplayProgressBar(title, desc, value);
        }

        public static void EncodeLuaFile(string srcFile, string outFile)
        {
            if (!srcFile.ToLower().EndsWith(".lua"))
            {
                File.Copy(srcFile, outFile, true);
                return;
            }

            bool isWin = true;
            string luaexe = string.Empty;
            string args = string.Empty;
            string exedir = string.Empty;
            string currDir = Directory.GetCurrentDirectory();
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {

                isWin = true;
                luaexe = "luajit.exe";
                args = "-b " + srcFile + " " + outFile;
                exedir = Utils.PathUtil.AppDataPath.Replace("assets", "") + "LuaEncoder/luajit/";
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                isWin = false;
                luaexe = "./luajit";
                args = "-b " + srcFile + " " + outFile;
                exedir = Utils.PathUtil.AppDataPath.Replace("assets", "") + "LuaEncoder/luajit_mac/";
            }

            Directory.SetCurrentDirectory(exedir);
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = luaexe;
            info.Arguments = args;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = isWin;
            info.ErrorDialog = true;
            Log.Debugger.Log(info.FileName + " " + info.Arguments);
            Process pro = Process.Start(info);
            pro.WaitForExit();
            Directory.SetCurrentDirectory(currDir);
        }
    }
}