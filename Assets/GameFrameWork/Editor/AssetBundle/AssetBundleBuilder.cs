using GameFrameWork.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using FileUtil = GameFrameWork.Utils.FileUtil;

namespace GameFrameWork.Editor
{
    public class AssetBundleBuilder : IDisposable
    {
        public AssetBundleBuilder()
        {
            m_ListBundlePath = new List<string>();
            m_ListPaths = new List<string>();
            m_ListFiles = new List<string>();
            m_AssetMap = new Dictionary<string, string>();
        }

        /// <summary>
        /// 打包
        /// </summary>
        public void Build(BuildTarget target, bool isShowNotify = true)
        {
            AssetBundleConfig config = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.assetBundleWindowDataPath);
            FileUtil.VerifyDirectory(PathUtil.GetAssetFullPath(config.assetBuildDir));

            if (EditorMgr.GetGameFrameWorkConfig().isUseLua)
            {
                if (EditorMgr.GetGameFrameWorkConfig().isLoadLuaFromAssetBundle)
                {
                    HandleLuaBundle();
                }
                else
                {
                    HandleLuaFile();
                }
            }

            m_ListBundlePath.Clear();
            m_ListPaths.Clear();
            m_ListFiles.Clear();
            m_BuildMaps.Clear();

            if (GenerateBuildMap(config))
            {
                if (m_BuildMaps.Count > 0)
                {
                    BuildPipeline.BuildAssetBundles(config.assetBuildDir, m_BuildMaps.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, target);

                    if (EditorMgr.GetGameFrameWorkConfig().isUseLua)
                    {
                        //FileUtil.DeleteDirectory(EditorPathUtil.luaPath);

                    }

                    AssetDatabase.Refresh();

                    if (config.isCopyAsset)
                    {
                        FileUtil.DeleteDirectory(config.assetCopyDir);
                        FileUtil.CopyDirectory(config.assetBuildFullDir, config.assetCopyDir);
                    }
                }

                AssetDatabase.Refresh();
            }

            //删除无用ab包
            FileUtil.VerifyDirectory(config.assetBuildFullDir);
            FileUtil.Recursive(config.assetBuildFullDir, m_ListFiles, m_ListPaths);

            for (int i = 0; i < m_ListFiles.Count; i++)
            {
                string directoryName = Path.GetDirectoryName(m_ListFiles[i]).Replace("\\", "/") + "/";
                directoryName = directoryName.Substring(directoryName.IndexOf(config.assetBuildDir));

                if (directoryName.Equals(config.assetBuildDir))
                {
                    continue;
                }

                string filePath = m_ListFiles[i].Substring(m_ListFiles[i].IndexOf(config.assetBuildDir));
                filePath = filePath.Substring(0, filePath.IndexOf(".")).Replace(config.assetBuildDir, string.Empty);

                if (!m_ListBundlePath.Contains(filePath))
                {
                    FileUtil.DeleteFile(m_ListFiles[i]);
                }
            }

            CreateVersionFile();
            CreateAssetMapFile();
            AssetDatabase.Refresh();

            if (isShowNotify)
            {
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(config.assetBuildDir);
                EditorUtility.DisplayDialog("提示", "打包成功", "确定");
            }
        }

        /// <summary>
        /// 生成打包列表
        /// </summary>
        private bool GenerateBuildMap(AssetBundleConfig config)
        {
            for (int i = 0; i < config.listDatas.Count; i++)
            {
                if (config.listDatas[i].bundleBuildType == AssetBundleData.BundleBuildType.Mulity)
                {
                    if (!AddMulityBuildMap(config.listDatas[i].bundlePath, config.listDatas[i].bundleExtend, config.listDatas[i].pattern, config.listDatas[i].assetPath, i))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!AddSingleBuildMap(config.listDatas[i].bundleName, config.listDatas[i].bundleExtend, config.listDatas[i].pattern, config.listDatas[i].assetPath, i))
                    {
                        return false;
                    }
                }
            }

            //框架资源
            AddMulityBuildMap("ArtResources/Materials/", ".assetbundle", "*", "Assets/GameFrameWork/Materials/", config.listDatas.Count);
            AddSingleBuildMap("Shaders", ".assetbundle", "*", "Assets/GameFrameWork/Shaders/", config.listDatas.Count + 1);

            return true;
            //AddBuildMap("fonts.unity3d", "*.TTF", "Assets/AssetsLibrary/Font");
            //AddBuildMapSingle("*.prefab", "Assets/AssetsLibrary/UI/Prefabs", "UI/Prefabs/");
        }

        private bool AddSingleBuildMap(string bundleName, string extend, string pattern, string path, int index)
        {
            if (!Directory.Exists(path))
            {
                EditorUtility.DisplayDialog("错误", "编号：" + (index + 1).ToString() + "\n" + path + "\n资源路径不存在", "确定");
                return false;
            }

            List<string> listFiles = new List<string>();
            List<string> listPaths = new List<string>();
            FileUtil.Recursive(path, listFiles, listPaths);

            if (listFiles.Count < 1)
            {
                EditorUtility.DisplayDialog("错误", "编号：" + (index + 1).ToString() + "\n" + path + "\n该路径下无任何文件", "确定");
                return false;
            }

            for (int i = 0; i < listFiles.Count; i++)
            {
                listFiles[i] = listFiles[i].Replace('\\', '/');
            }

            if (m_BuildMaps != null && m_BuildMaps.Count > 0)
            {
                for (int i = 0; i < m_BuildMaps.Count; i++)
                {
                    if (Path.GetFileNameWithoutExtension(m_BuildMaps[i].assetBundleName) == bundleName)
                    {
                        List<string> assetList = new List<string>();
                        assetList.AddRange(m_BuildMaps[i].assetNames);
                        assetList.AddRange(listFiles);
                        AssetBundleBuild temp = new AssetBundleBuild();
                        temp.assetBundleName = m_BuildMaps[i].assetBundleName;
                        temp.assetNames = assetList.ToArray();
                        m_BuildMaps[i] = temp;

                        for (int j = 0; j < listFiles.Count; j++)
                        {
                            m_AssetMap.Add(listFiles[j].Substring(7), bundleName + extend);
                        }

                        return true;
                    }
                }
            }

            string lowerBundleName = bundleName.ToLower();

            if (!m_ListBundlePath.Contains(lowerBundleName))
            {
                m_ListBundlePath.Add(lowerBundleName);
            }

            for (int i = 0; i < listFiles.Count; i++)
            {
                m_AssetMap.Add(listFiles[i].Substring(7), bundleName + extend);
            }

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName + extend;
            build.assetNames = listFiles.ToArray();
            m_BuildMaps.Add(build);

            return true;
        }

        private bool AddMulityBuildMap(string abPath, string extend, string pattern, string path, int index)
        {
            if (!Directory.Exists(path))
            {
                EditorUtility.DisplayDialog("错误", "编号：" + (index + 1).ToString() + "\n" + path + "\n资源路径不存在", "确定");
                return false;
            }

            List<string> listFiles = new List<string>();
            List<string> listPaths = new List<string>();
            FileUtil.Recursive(path, listFiles, listPaths);

            if (listFiles.Count < 1)
            {
                EditorUtility.DisplayDialog("错误", "编号：" + (index + 1).ToString() + "\n" + path + "\n该路径下无任何文件", "确定");
                return false;
            }

            for (int i = 0; i < listFiles.Count; i++)
            {
                string bundleName = abPath + Path.GetFileNameWithoutExtension(listFiles[i]);
                string lowerBundleName = bundleName.ToLower();

                if (!m_ListBundlePath.Contains(lowerBundleName))
                {
                    m_ListBundlePath.Add(lowerBundleName);
                }

                listFiles[i] = listFiles[i].Replace('\\', '/');
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = bundleName + extend;
                build.assetNames = new string[] { listFiles[i] };
                m_BuildMaps.Add(build);
                m_AssetMap.Add(listFiles[i].Substring(7), bundleName + extend);
            }

            return true;
        }

        /// <summary>
        /// 创建资源版本文件
        /// </summary>
        private void CreateVersionFile()
        {
            AssetBundleConfig config = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.assetBundleWindowDataPath);
            string versionPath = config.assetBuildFullDir + EditorMgr.GetGameFrameWorkConfig().versionFileName;

            m_ListPaths.Clear();
            m_ListFiles.Clear();

            FileUtil.Recursive(config.assetBuildFullDir, m_ListFiles, m_ListPaths);

            string content = string.Empty;

            for (int i = 0; i < m_ListFiles.Count; i++)
            {
                string md5 = FileUtil.MD5File(m_ListFiles[i]);
                string value = m_ListFiles[i].Replace(config.assetBuildFullDir, string.Empty).Replace("\\", "/");
                //string directory = Path.GetDirectoryName(value).Replace("\\", "/");
                //string fileName = Path.GetFileNameWithoutExtension(value);
                //string ext = Path.GetExtension(value);

                //if (!string.IsNullOrEmpty(directory))
                //{
                //    directory += "/";
                //}

                content += value + "|" + md5 + (i < m_ListFiles.Count - 1 ? "\n" : string.Empty);
            }

            FileUtil.CreateTextFile(versionPath, content);
        }

        /// <summary>
        /// 创建资源和ab映射文件
        /// </summary>
        private void CreateAssetMapFile()
        {
            AssetBundleConfig config = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.assetBundleWindowDataPath);
            string mapFile = config.assetBuildFullDir + "AssetMap.txt";

            string content = string.Empty;
            int index = 0;

            foreach (KeyValuePair<string, string> keyValuePair in m_AssetMap)
            {
                content += keyValuePair.Key + "|" + keyValuePair.Value;

                if (index < m_AssetMap.Count - 1)
                {
                    content += "\n";
                }

                index++;
            }

            FileUtil.CreateTextFile(mapFile, content);
        }

        #region Lua
        /// <summary>
        /// 处理Lua代码包
        /// </summary>
        private void HandleLuaBundle()
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
        private void HandleLuaFile()
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

        public void EncodeLuaFile(string srcFile, string outFile)
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

        public void Dispose()
        {
            m_ListBundlePath.Clear();
            m_ListPaths.Clear();
            m_ListFiles.Clear();
            m_AssetMap.Clear();

            m_ListBundlePath = null;
            m_ListPaths = null;
            m_ListFiles = null;
            m_AssetMap = null;
        }

        private List<string> m_ListBundlePath = null;
        private List<string> m_ListPaths = null;
        private List<string> m_ListFiles = null;
        private Dictionary<string, string> m_AssetMap = null;
        private List<AssetBundleBuild> m_BuildMaps = new List<AssetBundleBuild>();
    }
}