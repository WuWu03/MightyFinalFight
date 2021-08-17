using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using System.IO;
using GameFrameWork.Utility;
using UnityEngine.U2D;

namespace GameFrameWork.Resources
{
    public class ResMgr : BaseMgr<ResMgr>
    {
        class AssetBundleInfo
        {
            public AssetBundle AssetBundle;
            public int ReferencedCount;

            public AssetBundleInfo(AssetBundle assetBundle)
            {
                AssetBundle = assetBundle;
                ReferencedCount = 0;
            }
        }

        class LoadAssetRequest
        {
            public GameFrameWorkAction<string, Object, object[]> SharpFunc;
            public bool LoadMainAsset;
            public Type AssetType;
            public string AssetName;
            public string AssetPath;
            public object[] Args;
        }

        class AssetVersion
        {
            public string FilePath;
            public string ExtendName;
            public string MD5;

            public AssetVersion(string filePath, string extendName, string md5)
            {
                FilePath = filePath;
                ExtendName = extendName;
                MD5 = md5;
            }
        }

        private void Awake()
        {
            m_Dependencies = new Dictionary<string, string[]>();
            m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
            m_LoadRequests = new Dictionary<string, List<LoadAssetRequest>>();
            m_DicAssetVerson = new Dictionary<string, AssetVersion>();

#if UNITY_EDITOR
            if (AppConfig.Ins.LoadAB)
#endif
            {
                string maniFesturl = PathUtil.RunTimeAssetPath + PathUtil.ManiFest;
                string versionUrl = PathUtil.RunTimeAssetPath + PathUtil.AssetBundleVersion;

                byte[] stream = File.ReadAllBytes(maniFesturl);
                AssetBundle assetbundle = AssetBundle.LoadFromMemory(stream);
                m_Manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

                string[] version = File.ReadAllText(versionUrl).Split('\n');

                for (int i = 0; i < version.Length; i++)
                {
                    string[] data = version[i].Split('|');
                    if (!data[1].Equals(".manifest"))
                    {
                        m_DicAssetVerson.Add(data[0], new AssetVersion(data[0], data[1], data[2]));
                    }
                }
            }
        }

        private void Update()
        {
            if (Time.time - m_UnLoadTime >= UNLOAD_TIME)
            {
                m_UnLoadTime = Time.time;
                UnityEngine.Resources.UnloadUnusedAssets();
                GC.Collect();
            }
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        public T LoadAsset<T>(string abName,bool loadMainAsset = true) where T: Object
        {
            Object o = LoadAsset(abName, loadMainAsset, typeof(T));

            if(o == null)
            {
                return null;
            }

            return o as T;
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        public Object LoadAsset(string abName,bool loadMainAsset = true,Type t = null)
        {
            if (t == null)
            {
                t = typeof(Object);
            }
            bool isLoadAb = AppConfig.Ins.LoadAB;
#if UNITY_EDITOR
            if (!isLoadAb)
                return ResMgrEditor.Ins.LoadForEditor(abName, t);
            else
#endif
                return Load(abName, loadMainAsset, t);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAssetAsync<T>(string abName, GameFrameWorkAction<string, Object, object[]> action = null, bool loadMainAsset = true, params object[] param)
        {
            LoadAssetAsync(abName, action, loadMainAsset, typeof(T), param);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAssetAsync(string abName, GameFrameWorkAction<string, Object, object[]> action = null, bool loadMainAsset = true, Type t = null, params object[] param)
        {
            if (t == null)
            {
                t = typeof(Object);
            }
            bool isLoadAb = AppConfig.Ins.LoadAB;
#if UNITY_EDITOR
            if (!isLoadAb)
                ResMgrEditor.Ins.LoadForEditorAsync(abName, action, t, param);
            else
#endif
                LoadAsync(abName, action, loadMainAsset, t, param);
        }

        /// <summary>
        /// 此函数交给外部卸载专用，自己调整是否需要彻底清除AB
        /// </summary>
        public void UnloadAssetBundle(string abName, bool isThorough = false)
        {
            abName = GetRealAssetPath(abName);
            Log.GameFrameworkLog.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + abName);
            UnloadAssetBundleInternal(abName, isThorough);
            UnloadDependencies(abName, isThorough);
            Log.GameFrameworkLog.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + abName);
        }

        /// <summary>
        /// 同步加载
        /// </summary>

        private Object Load(string abName, bool loadMainAsset = false, Type t = null)
        {
            Log.GameFrameworkLog.Log("LoadAsset：" + abName);

            abName = GetRealAssetPath(abName);
            LoadDependencies(abName);

            AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);
            if (bundleInfo == null)
            {
                OnLoadAssetBundle(abName);
                bundleInfo = GetLoadedAssetBundle(abName);

                if (bundleInfo == null)
                {
                    m_LoadRequests.Remove(abName);
                    Log.GameFrameworkLog.LogError("OnLoadAsset--->>>" + abName);
                    return null;
                }
            }

            string[] dependencies = null;
            AssetBundle ab = null;

            if (m_Dependencies.TryGetValue(abName, out dependencies))
            {
                while (DependenciesLoaded(dependencies))
                {
                    ab = bundleInfo.AssetBundle;
                    return ab.GetAsset(Path.GetFileNameWithoutExtension(abName), t);
                }
            }

            ab = bundleInfo.AssetBundle;
            return ab.GetAsset(Path.GetFileNameWithoutExtension(abName), t);
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        private void LoadAsync(string abName, GameFrameWorkAction<string, Object, object[]> action = null, bool loadMainAsset = false, Type t = null, object[] param = null)
        {
            Log.GameFrameworkLog.Log("LoadAsset：" + abName);

            string realAssetPath = GetRealAssetPath(abName);
            LoadAssetRequest request = new LoadAssetRequest();
            request.SharpFunc = action;
            request.LoadMainAsset = loadMainAsset;
            request.AssetType = t;
            request.Args = param;
            request.AssetName = Path.GetFileNameWithoutExtension(realAssetPath);
            request.AssetPath = abName;
            List<LoadAssetRequest> requests = null;
            if (!m_LoadRequests.TryGetValue(realAssetPath, out requests))
            {
                requests = new List<LoadAssetRequest>();
                requests.Add(request);
                m_LoadRequests.Add(realAssetPath, requests);
                LoadDependencies(realAssetPath);
                StartCoroutine(OnLoadAsset(realAssetPath));
            }
            else
            {
                requests.Add(request);
            }
        }

        private IEnumerator OnLoadAsset(string realAssetPath)
        {
            yield return new WaitForSeconds(0);
            AssetBundleInfo bundleInfo = GetLoadedAssetBundle(realAssetPath);
            if (bundleInfo == null)
            {
                yield return StartCoroutine(OnLoadAssetBundleAsync(realAssetPath));

                bundleInfo = GetLoadedAssetBundle(realAssetPath);
                if (bundleInfo == null)
                {
                    m_LoadRequests.Remove(realAssetPath);
                    Log.GameFrameworkLog.LogError("OnLoadAsset--->>>" + realAssetPath);
                    yield break;
                }
            }
            List<LoadAssetRequest> list = null;
            if (!m_LoadRequests.TryGetValue(realAssetPath, out list))
            {
                m_LoadRequests.Remove(realAssetPath);
                yield break;
            }

            string[] dependencies = null;
            if (m_Dependencies.TryGetValue(realAssetPath, out dependencies))
            {
                while (!DependenciesLoaded(dependencies))
                {
                    yield return null;
                }
            }

            AssetBundle ab = bundleInfo.AssetBundle;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].SharpFunc != null)
                {
                    if (list[i].LoadMainAsset)
                    {
                        list[i].SharpFunc(list[i].AssetPath, ab.GetAsset(list[i].AssetName, list[i].AssetType), list[i].Args);
                        list[i].SharpFunc = null;
                    }
                }
                bundleInfo.ReferencedCount++;
            }
            m_LoadRequests.Remove(realAssetPath);
        }


        private void OnLoadAssetBundle(string abName)
        {
            string path = GetAssetBundlePath(abName);
            Log.GameFrameworkLog.Log("开始同步加载资源：" + path);

            AssetBundle assetObj = AssetBundle.LoadFromFile(path);
            if (assetObj != null)
            {
                m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));
            }
        }

        private IEnumerator OnLoadAssetBundleAsync(string abName)
        {
            string path = GetAssetBundlePath(abName);
            Log.GameFrameworkLog.Log("开始异步加载资源：" + path);
            AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(path);
            yield return createRequest;

            AssetBundle assetObj = createRequest.assetBundle;
            if (assetObj != null)
            {
                m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));
            }
        }

        private AssetBundleInfo GetLoadedAssetBundle(string abName)
        {
            AssetBundleInfo bundle = null;
            m_LoadedAssetBundles.TryGetValue(abName, out bundle);
            if (bundle == null) return null;
            // No dependencies are recorded, only the bundle itself is required.
            //string[] dependencies = null;
            //if (!m_Dependencies.TryGetValue(abName, out dependencies))
            //    return bundle;
            return bundle;
        }

        private bool DependenciesLoaded(string[] dependencies)
        {
            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies)
            {
                AssetBundleInfo dependentBundle;
                m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null) return false;
            }

            return true;
        }

        private string GetRealAssetPath(string abName)
        {
            abName = abName.ToLower();
            AssetVersion version = null;

            if(m_DicAssetVerson.TryGetValue(abName,out version))
            {
                if (!abName.EndsWith(version.ExtendName))
                {
                    abName += version.ExtendName;
                }

                return abName;
            }

            Log.GameFrameworkLog.LogError("Can't find the version of" + abName);
            return string.Empty;
        }

        /// <summary>
        /// 载入依赖
        /// </summary>
        /// <param name="name"></param>
        private void LoadDependencies(string abName)
        {
            if (m_Manifest == null)
            {
                Log.GameFrameworkLog.LogError("Please initialize AssetBundleManifest first.");
                return;
            }
            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
            {
                dependencies = m_Manifest.GetAllDependencies(abName);
                if (dependencies.Length > 0)
                {
                    m_Dependencies.Add(abName, dependencies);
                }
            }

            if (dependencies.Length <= 0) return;

            for (int i = 0; i < dependencies.Length; i++)
            {
                StartCoroutine(OnLoadAsset(dependencies[i]));
            }
        }

        private void UnloadDependencies(string abName, bool isThorough)
        {
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return;

            // Loop dependencies.
            foreach (var dependency in dependencies)
            {
                UnloadAssetBundleInternal(dependency, isThorough);
            }
            m_Dependencies.Remove(abName);
        }

        private void UnloadAssetBundleInternal(string abName, bool isThorough)
        {
            AssetBundleInfo bundle = GetLoadedAssetBundle(abName);
            if (bundle == null) return;

            if (--bundle.ReferencedCount <= 0)
            {
                if (m_LoadRequests.ContainsKey(abName))
                {
                    return;     //如果当前AB处于Async Loading过程中，卸载会崩溃，只减去引用计数即可
                }
                bundle.AssetBundle.Unload(isThorough);
                m_LoadedAssetBundles.Remove(abName);
                Log.GameFrameworkLog.Log(TextUtil.FormatDefault(abName, " has been unloaded successfully"));
            }
        }

        private string GetAssetBundlePath(string abName)
        {
            return PathUtil.FormatPath(PathUtil.RunTimeAssetPath, abName);
        }

        protected override void OnShutDown()
        {
            m_Dependencies.Clear();
            m_LoadedAssetBundles.Clear();
            m_LoadRequests.Clear();
            m_DicAssetVerson.Clear();
        }

        public const int UNLOAD_TIME = 60 * 15;
        private float m_UnLoadTime;
        private AssetBundleManifest m_Manifest;
        private Dictionary<string, string[]> m_Dependencies = null;
        private Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = null;
        private Dictionary<string, List<LoadAssetRequest>> m_LoadRequests = null;
        private Dictionary<string, AssetVersion> m_DicAssetVerson = null;
    }
}