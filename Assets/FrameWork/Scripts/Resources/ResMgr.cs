using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using System.IO;

namespace FrameWork.Resources
{
    public class ResMgr : BaseMgr<ResMgr>
    {
        class AssetBundleInfo
        {
            public AssetBundle m_AssetBundle;
            public int m_ReferencedCount;

            public AssetBundleInfo(AssetBundle assetBundle)
            {
                m_AssetBundle = assetBundle;
                m_ReferencedCount = 0;
            }
        }

        public const string ExtName = ".assetBundle";                   //素材扩展名
        public const int UNLOAD_TIME = 60 * 15;
        private AssetBundleManifest manifest;
        private Dictionary<string, Object> _mResDic = null;

        public delegate void LoadCallBack(object obj);

        private void Awake()
        {
            _mResDic = new Dictionary<string, Object>();
#if UNITY_EDITOR
            if (RuntimeEnvironment.Instance.loadResFromAssetBundle)
#endif
            {
                string url = ResDefine.AssetBundlePath + "/StreamingAssets";
                byte[] stream = File.ReadAllBytes(url);
                AssetBundle assetbundle = AssetBundle.LoadFromMemory(stream);
                manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }

            //StartCoroutine(OnTimeRelease());
        }

        float m_UnLoadTime;
        private void Update()
        {
            if (Time.time - m_UnLoadTime >= UNLOAD_TIME)
            {
                m_UnLoadTime = Time.time;
                UnityEngine.Resources.UnloadUnusedAssets();
                GC.Collect();
            }
        }


        Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();
        Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
        Dictionary<string, List<LoadAssetRequest>> m_LoadRequests = new Dictionary<string, List<LoadAssetRequest>>();

        class LoadAssetRequest
        {
            public Action<Object> sharpFunc;
            public bool loadMainAsset;
            public Type assetType;
        }

        string GetRealAssetPath(string abName)
        {
            abName = abName.ToLower();

            if (!abName.EndsWith(ExtName))
            {
                abName += ExtName;
            }
            return abName;
        }


        /// <summary>
        /// 加载资源 csharp调用
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="action"></param>
        public void LoadAsset(string abName, Action<Object> action = null, bool loadMainAsset = true, Type t = null)
        {
            if (t == null)
            {
                t = typeof(Object);
            }
            bool isLoadAb = RuntimeEnvironment.Instance.loadResFromAssetBundle;
#if UNITY_EDITOR
            if (!isLoadAb)
                ResMgrEditor.Ins.LoadForEditorAsync(abName, action, t);
            else
#endif
                loadAsset(abName, action, loadMainAsset, t);
        }

        /// <summary>
        /// 载入素材
        /// </summary>
        void loadAsset(string abName, Action<Object> action = null, bool loadMainAsset = false, Type t = null)
        {
            Debug.Log("LoadAsset：" + abName);

            abName = GetRealAssetPath(abName);
            LoadAssetRequest request = new LoadAssetRequest();
            request.sharpFunc = action;
            request.loadMainAsset = loadMainAsset;
            request.assetType = t;

            List<LoadAssetRequest> requests = null;
            if (!m_LoadRequests.TryGetValue(abName, out requests))
            {
                requests = new List<LoadAssetRequest>();
                requests.Add(request);
                m_LoadRequests.Add(abName, requests);
                LoadDependencies(abName);
                StartCoroutine(OnLoadAsset(abName));
            }
            else
            {
                requests.Add(request);
            }
        }

        IEnumerator OnLoadAsset(string abName)
        {
            yield return new WaitForSeconds(0);
            AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);
            if (bundleInfo == null)
            {
                yield return StartCoroutine(OnLoadAssetBundle(abName));

                bundleInfo = GetLoadedAssetBundle(abName);
                if (bundleInfo == null)
                {
                    m_LoadRequests.Remove(abName);
                    Debug.LogError("OnLoadAsset--->>>" + abName);
                    yield break;
                }
            }
            List<LoadAssetRequest> list = null;
            if (!m_LoadRequests.TryGetValue(abName, out list))
            {
                m_LoadRequests.Remove(abName);
                yield break;
            }

            string[] dependencies = null;
            if (m_Dependencies.TryGetValue(abName, out dependencies))
            {
                while (!DependenciesLoaded(dependencies))
                {
                    yield return null;
                }
            }

            AssetBundle ab = bundleInfo.m_AssetBundle;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].sharpFunc != null)
                {
                    if (list[i].loadMainAsset)
                    {
                        list[i].sharpFunc(ab.GetMainAsset(list[i].assetType));
                        list[i].sharpFunc = null;
                    }

                }
                bundleInfo.m_ReferencedCount++;
            }
            m_LoadRequests.Remove(abName);
        }


        IEnumerator OnLoadAssetBundle(string abName)
        {
            string path = GetAssetBundlePath(abName);
            Debug.Log("开始异步加载资源：" + path);
            AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(path);
            yield return createRequest;

            AssetBundle assetObj = createRequest.assetBundle;
            if (assetObj != null)
            {
                m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));
            }
        }

        AssetBundleInfo GetLoadedAssetBundle(string abName)
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

        bool DependenciesLoaded(string[] dependencies)
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


        /// <summary>
        /// 此函数交给外部卸载专用，自己调整是否需要彻底清除AB
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isThorough"></param>
        public void UnloadAssetBundle(string abName, bool isThorough = false)
        {
            abName = GetRealAssetPath(abName);
            //if(LoggerHelper.isLogDebug)
            Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + abName);
            UnloadAssetBundleInternal(abName, isThorough);
            UnloadDependencies(abName, isThorough);
            //if (LoggerHelper.isLogDebug)
            Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + abName);
        }

        /// <summary>
        /// 载入依赖
        /// </summary>
        /// <param name="name"></param>
        void LoadDependencies(string abName)
        {
            if (manifest == null)
            {
                Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }
            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
            {
                dependencies = manifest.GetAllDependencies(abName);
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

        void UnloadDependencies(string abName, bool isThorough)
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

        void UnloadAssetBundleInternal(string abName, bool isThorough)
        {
            AssetBundleInfo bundle = GetLoadedAssetBundle(abName);
            if (bundle == null) return;

            if (--bundle.m_ReferencedCount <= 0)
            {
                if (m_LoadRequests.ContainsKey(abName))
                {
                    return;     //如果当前AB处于Async Loading过程中，卸载会崩溃，只减去引用计数即可
                }
                bundle.m_AssetBundle.Unload(isThorough);
                m_LoadedAssetBundles.Remove(abName);
                Debug.Log(abName + " has been unloaded successfully");
            }
        }

        string GetAssetBundlePath(string abName)
        {
            return string.Format("{0}/{1}", ResDefine.AssetBundlePath, abName);
        }

        public override void ShutDown()
        {
            
        }
    }
}