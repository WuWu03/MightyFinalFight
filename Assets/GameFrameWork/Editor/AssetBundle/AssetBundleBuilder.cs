using GameFrameWork.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
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
        public bool Build(BuildTarget target, bool isShowNotify = true)
        {
            FileUtil.VerifyDirectory(EditorPathUtil.streamingAssetsFullPath);

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

            AssetBundleConfig config = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.assetBundleWindowDataPath);

            if (!GenerateBuildMap(config))
            {
                return false;
            }

            if (m_BuildMaps.Count < 1)
            {
                return true;
            }

            BuildPipeline.BuildAssetBundles(EditorPathUtil.streamingAssetsPath, m_BuildMaps.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, target);

            if (EditorMgr.GetGameFrameWorkConfig().isUseLua)
            {
                //FileUtil.DeleteDirectory(EditorPathUtil.luaPath);
            }

            AssetDatabase.Refresh();

            if (config.isCopyAsset && !string.IsNullOrEmpty(config.assetCopyDir))
            {
                FileUtil.DeleteDirectory(config.assetCopyDir);
                FileUtil.CopyDirectory(EditorPathUtil.streamingAssetsPath, config.assetCopyDir);
            }

            //删除无用ab包
            FileUtil.VerifyDirectory(EditorPathUtil.streamingAssetsFullPath);
            FileUtil.Recursive(EditorPathUtil.streamingAssetsFullPath, "*.*", m_ListFiles, m_ListPaths);

            for (int i = 0; i < m_ListFiles.Count; i++)
            {
                string directoryName = Path.GetDirectoryName(m_ListFiles[i]).Replace("\\", "/") + "/";
                directoryName = directoryName.Substring(directoryName.IndexOf(EditorPathUtil.streamingAssetsPath));

                if (directoryName.Equals(EditorPathUtil.streamingAssetsPath))
                {
                    continue;
                }

                string filePath = m_ListFiles[i].Substring(m_ListFiles[i].IndexOf(EditorPathUtil.streamingAssetsPath));
                filePath = filePath.Substring(0, filePath.IndexOf(".")).Replace(EditorPathUtil.streamingAssetsPath, string.Empty);

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
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(EditorPathUtil.streamingAssetsPath);
                EditorUtility.DisplayDialog("提示", "打包成功", "确定");
            }

            return true;
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
                    if (!AddMulityBuildMap(config.listDatas[i], i))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!AddSingleBuildMap(config.listDatas[i], i))
                    {
                        return false;
                    }
                }
            }

            //框架资源
            AddMulityBuildMap("ArtResources/Materials/", "Assets/GameFrameWork/Materials/", ".assetbundle", "*", config.listDatas.Count);
            AddSingleBuildMap("Shaders", "Assets/GameFrameWork/Shaders/", ".assetbundle", "*", config.listDatas.Count + 1);

            return true;
        }

        private bool AddSingleBuildMap(AssetBundleData assetBundleData, int index)
        {
            return AddSingleBuildMap(assetBundleData.bundleName, assetBundleData.assetPath, assetBundleData.bundleExtend, assetBundleData.pattern, index);
        }

        private bool AddSingleBuildMap(string bundleName, string assetPath, string extend, string pattern, int index)
        {
            if (!Directory.Exists(assetPath))
            {
                EditorUtility.DisplayDialog("错误", "编号：" + (index + 1).ToString() + "\n" + assetPath + "\n资源路径不存在", "确定");
                return false;
            }

            List<string> listFiles = new List<string>();
            List<string> listPaths = new List<string>();
            FileUtil.Recursive(assetPath, pattern, listFiles, listPaths);

            if (listFiles.Count < 1)
            {
                EditorUtility.DisplayDialog("错误", "编号：" + (index + 1).ToString() + "\n" + assetPath + "\n该路径下无任何文件", "确定");
                return false;
            }

            for (int i = 0; i < listFiles.Count; i++)
            {
                listFiles[i] = listFiles[i].Replace('\\', '/');
            }

            string bundleLowerName = bundleName.ToLower();
            AddBuildMap(bundleLowerName, extend, listFiles.ToArray());
            return true;
        }

        private bool AddMulityBuildMap(AssetBundleData assetBundleData, int index)
        {
            return AddMulityBuildMap(assetBundleData.bundleName, assetBundleData.assetPath, assetBundleData.bundleExtend, assetBundleData.pattern, index);
        }

        private bool AddMulityBuildMap(string bundleName, string assetPath, string extend, string pattern, int index)
        {
            if (!Directory.Exists(assetPath))
            {
                EditorUtility.DisplayDialog("错误", "编号：" + (index + 1).ToString() + "\n" + assetPath + "\n资源路径不存在", "确定");
                return false;
            }

            string[] assetDirectories = Directory.GetDirectories(assetPath);

            List<string> listFiles = new List<string>();
            List<string> listPaths = new List<string>();
            FileUtil.Recursive(assetPath, pattern, listFiles, listPaths);

            if (listFiles.Count < 1)
            {
                EditorUtility.DisplayDialog("错误", "编号：" + (index + 1).ToString() + "\n" + assetPath + "\n该路径下无任何文件", "确定");
                return false;
            }

            for (int i = 0; i < listFiles.Count; i++)
            {
                listFiles[i] = listFiles[i].Replace('\\', '/');
            }

            if (assetDirectories.Length > 0)
            {
                for (int i = 0; i < assetDirectories.Length; i++)
                {
                    string assetDirectory = assetDirectories[i].Replace('\\', '/');
                    string[] tempFiles = listFiles.FindAll(x => x.Contains(assetDirectory)).ToArray();
                    string bundleLowerName = (bundleName + assetDirectory.Substring(assetDirectory.LastIndexOf("/") + 1)).ToLower();

                    if (bundleLowerName.EndsWith("/"))
                    {
                        bundleLowerName = bundleLowerName.Substring(0, bundleName.Length - 1);
                    }

                    for (int j = 0; j < tempFiles.Length; j++)
                    {
                        listFiles.Remove(tempFiles[j]);
                    }

                    AddBuildMap(bundleLowerName, extend, tempFiles);
                }
            }

            for (int i = 0; i < listFiles.Count; i++)
            {
                string[] tempFiles = new string[] { listFiles[i] };
                string bundleLowerName = (bundleName + Path.GetFileNameWithoutExtension(listFiles[i])).ToLower();

                if (bundleLowerName.EndsWith("/"))
                {
                    bundleLowerName = bundleLowerName.Substring(0, bundleName.Length - 1);
                }

                AddBuildMap(bundleLowerName, extend, tempFiles);
            }

            return true;
        }

        private void AddBuildMap(string bundleName, string extend, string[] assetFiles)
        {
            if (m_BuildMaps != null && m_BuildMaps.Count > 0)
            {
                for (int i = 0; i < m_BuildMaps.Count; i++)
                {
                    if (Path.GetFileNameWithoutExtension(m_BuildMaps[i].assetBundleName) == bundleName)
                    {
                        List<string> assetList = new List<string>();
                        assetList.AddRange(m_BuildMaps[i].assetNames);
                        assetList.AddRange(assetFiles);
                        AssetBundleBuild temp = new AssetBundleBuild();
                        temp.assetBundleName = m_BuildMaps[i].assetBundleName;
                        temp.assetNames = assetList.ToArray();
                        m_BuildMaps[i] = temp;

                        for (int j = 0; j < assetFiles.Length; j++)
                        {
                            AddAssetMap(assetFiles[j], bundleName, extend);
                        }

                        return;
                    }
                }
            }

            if (!m_ListBundlePath.Contains(bundleName))
            {
                m_ListBundlePath.Add(bundleName);
            }

            for (int i = 0; i < assetFiles.Length; i++)
            {
                AddAssetMap(assetFiles[i], bundleName, extend);
            }

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName + extend;
            build.assetNames = assetFiles;
            m_BuildMaps.Add(build);
        }

        private void AddAssetMap(string assetPath, string bundleName, string extend)
        {
            m_AssetMap.Add(assetPath.Substring(7), bundleName + extend);
        }

        /// <summary>
        /// 创建资源版本文件
        /// </summary>
        private void CreateVersionFile()
        {
            string versionPath = EditorPathUtil.streamingAssetsFullPath + EditorMgr.GetGameFrameWorkConfig().versionFileName;

            m_ListPaths.Clear();
            m_ListFiles.Clear();

            FileUtil.Recursive(EditorPathUtil.streamingAssetsFullPath, "*.*", m_ListFiles, m_ListPaths);

            string content = string.Empty;

            for (int i = 0; i < m_ListFiles.Count; i++)
            {
                string md5 = FileUtil.MD5File(m_ListFiles[i]);
                string value = m_ListFiles[i].Replace(EditorPathUtil.streamingAssetsFullPath, string.Empty).Replace("\\", "/");
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
            string mapFile = EditorPathUtil.streamingAssetsFullPath + EditorMgr.GetGameFrameWorkConfig().assetMapFileName;

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