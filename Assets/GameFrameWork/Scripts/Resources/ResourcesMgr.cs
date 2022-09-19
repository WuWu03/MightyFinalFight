using GameFrameWork.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFrameWork.Resources
{
    public class ResourcesMgr : BaseMgr<ResourcesMgr>
    {
        class AssetBundleInfo
        {
            public AssetBundle assetBundle;
            public int referencedCount;

            public AssetBundleInfo(AssetBundle assetBundle)
            {
                this.assetBundle = assetBundle;
                referencedCount = 0;
            }
        }

        class LoadAssetRequest
        {
            public GameFrameWorkAction<string, Object, object[]> onLoadEvent;
            public bool loadMainAsset;
            public Type assetType;
            public string assetName;
            public string assetPath;
            public object[] args;
        }

        class AssetVersion
        {
            public string filePath;
            public string extendName;
            public string md5Value;

            public AssetVersion(string filePath, string extendName, string md5Value)
            {
                this.filePath = filePath;
                this.extendName = extendName;
                this.md5Value = md5Value;
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            m_Dependencies = new Dictionary<string, string[]>();
            m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
            m_LoadRequests = new Dictionary<string, List<LoadAssetRequest>>();
            m_DicAssetVerson = new Dictionary<string, AssetVersion>();

#if UNITY_EDITOR
            if (AppConfig.instance.loadAB)
#endif
            {
                string maniFesturl = PathUtil.runTimeAssetPath + PathUtil.maniFestName;
                string versionUrl = PathUtil.runTimeAssetPath + PathUtil.assetBundleVersionName;

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

        /// <summary>
        /// 同步加载资源
        /// </summary>
        public T LoadAsset<T>(string abName, bool loadMainAsset = true) where T : Object
        {
            Object obj = LoadAsset(abName, loadMainAsset, typeof(T));

            if (obj == null)
            {
                return null;
            }

            return obj as T;
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        public Object LoadAsset(string abName, bool loadMainAsset = true, Type t = null)
        {
            if (t == null)
            {
                t = typeof(Object);
            }
            bool isLoadAb = AppConfig.instance.loadAB;
#if UNITY_EDITOR
            if (!isLoadAb)
                return EditorResourcesMgr.Instance.LoadForEditor(abName, t);
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
            bool isLoadAb = AppConfig.instance.loadAB;
#if UNITY_EDITOR
            if (!isLoadAb)
                EditorResourcesMgr.Instance.LoadForEditorAsync(abName, action, t, param);
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

            AssetBundle ab = null;

            if (m_Dependencies.TryGetValue(abName, out string[] dependencies))
            {
                while (DependenciesLoaded(dependencies))
                {
                    ab = bundleInfo.assetBundle;
                    return ab.GetAsset(Path.GetFileNameWithoutExtension(abName), t);
                }
            }

            ab = bundleInfo.assetBundle;
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
            request.onLoadEvent = action;
            request.loadMainAsset = loadMainAsset;
            request.assetType = t;
            request.args = param;
            request.assetName = Path.GetFileNameWithoutExtension(realAssetPath);
            request.assetPath = abName;

            if (!m_LoadRequests.TryGetValue(realAssetPath, out List<LoadAssetRequest> requests))
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

            if (!m_LoadRequests.TryGetValue(realAssetPath, out List<LoadAssetRequest> list))
            {
                m_LoadRequests.Remove(realAssetPath);
                yield break;
            }

            if (m_Dependencies.TryGetValue(realAssetPath, out string[] dependencies))
            {
                while (!DependenciesLoaded(dependencies))
                {
                    yield return null;
                }
            }

            AssetBundle ab = bundleInfo.assetBundle;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].onLoadEvent != null)
                {
                    if (list[i].loadMainAsset)
                    {
                        list[i].onLoadEvent(list[i].assetPath, ab.GetAsset(list[i].assetName, list[i].assetType), list[i].args);
                        list[i].onLoadEvent = null;
                    }
                }
                bundleInfo.referencedCount++;
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
            m_LoadedAssetBundles.TryGetValue(abName, out AssetBundleInfo bundle);

            if (bundle == null)
            {
                return null;
            }
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
                if (!m_LoadedAssetBundles.ContainsKey(dependency))
                {
                    return false;
                }
            }

            return true;
        }

        private string GetRealAssetPath(string abName)
        {
            abName = abName.ToLower();

            if (m_DicAssetVerson.TryGetValue(abName, out AssetVersion version))
            {
                if (!abName.EndsWith(version.extendName))
                {
                    abName += version.extendName;
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

            if (!m_Dependencies.TryGetValue(abName, out string[] dependencies))
            {
                dependencies = m_Manifest.GetAllDependencies(abName);
                if (dependencies != null && dependencies.Length > 0)
                {
                    m_Dependencies.Add(abName, dependencies);
                }
            }

            if (dependencies == null || dependencies.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < dependencies.Length; i++)
            {
                StartCoroutine(OnLoadAsset(dependencies[i]));
            }
        }

        private void UnloadDependencies(string abName, bool isThorough)
        {
            if (!m_Dependencies.TryGetValue(abName, out string[] dependencies))
            {
                return;
            }

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

            if (--bundle.referencedCount <= 0)
            {
                if (m_LoadRequests.ContainsKey(abName))
                {
                    return;     //如果当前AB处于Async Loading过程中，卸载会崩溃，只减去引用计数即可
                }
                bundle.assetBundle.Unload(isThorough);
                m_LoadedAssetBundles.Remove(abName);
                Log.GameFrameworkLog.Log(StringUtil.FormatDefault(abName, " has been unloaded successfully"));
            }
        }

        private string GetAssetBundlePath(string abName)
        {
            return PathUtil.FormatPath(PathUtil.runTimeAssetPath, abName);
        }

        protected override void OnShutDown()
        {
            m_Dependencies.Clear();
            m_LoadedAssetBundles.Clear();
            m_LoadRequests.Clear();
            m_DicAssetVerson.Clear();
        }

        private AssetBundleManifest m_Manifest;
        private Dictionary<string, string[]> m_Dependencies = null;
        private Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = null;
        private Dictionary<string, List<LoadAssetRequest>> m_LoadRequests = null;
        private Dictionary<string, AssetVersion> m_DicAssetVerson = null;
    }
}