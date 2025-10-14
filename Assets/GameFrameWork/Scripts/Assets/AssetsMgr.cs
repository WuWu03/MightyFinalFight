using GameFrameWork.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFrameWork.Event;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFrameWork.Assets
{
    public class AssetsMgr : BaseMgr<AssetsMgr>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            m_LoadedAssetBundles = new();
            m_LoadRequests = new();
            m_AssetsMap = new();
            InitAssetsMap();
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();
            UnloadAll();
            m_AssetsMap.Clear();
        }

        protected override void OnDestory()
        {
            base.OnDestory();
            m_Manifest = null;
            m_LoadedAssetBundles = null;
            m_LoadRequests = null;
            m_AssetsMap = null;
        }

        public void InitAssetsMap()
        {
#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                return;
            }
#endif        
            if (m_ManifestAssetBundle != null)
            {
                UnloadAll(true);
                m_ManifestAssetBundle.Unload(true);
            }

            string maniFestPath = PathUtil.FormatPath(PathUtil.runTimeAssetsPath, PathUtil.maniFestName);
            byte[] maniFestData = File.ReadAllBytes(maniFestPath);

            m_ManifestAssetBundle = AssetBundle.LoadFromMemory(maniFestData);
            m_Manifest = m_ManifestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            string assetMapPath = PathUtil.FormatPath(PathUtil.runTimeAssetsPath, GameFrameWorkEntry.config.assetMapFileName);
            string[] assetsMap = File.ReadAllText(assetMapPath).Split('\n');

            m_AssetsMap.Clear();

            for (int i = 0; i < assetsMap.Length; i++)
            {
                string[] data = assetsMap[i].Split('|', 2);
                m_AssetsMap.Add(data[0], data[1]);
            }
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        public T LoadAssetSync<T>(string assetPath) where T : Object
        {
            Object obj = LoadAssetSync(assetPath, typeof(T));

            if (obj == null)
            {
                return null;
            }

            return obj as T;
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        public Object LoadAssetSync(string assetPath, Type assetType)
        {
            if (assetType == null)
            {
                assetType = typeof(Object);
            }
#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                return EditorAssetsMgr.LoadAssetSync(assetPath, assetType);
            }
#endif
            return InnerLoadAssetSync(assetPath, assetType);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAssetAsync<T>(string assetPath, GameFrameWorkAction<string, Object, object> loadedAction, object arg = null) where T : Object
        {
            LoadAssetAsync(assetPath, typeof(T), loadedAction, arg);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAssetAsync(string assetPath, Type assetType, GameFrameWorkAction<string, Object, object> loadedAction, object arg = null)
        {
            if (assetType == null)
            {
                assetType = typeof(Object);
            }
#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                EditorAssetsMgr.LoadAssetAsync(this, assetPath, assetType, loadedAction, arg);
                return;
            }
#endif
            InnerLoadAssetAsync(assetPath, assetType, loadedAction, false, arg);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        public void UnloadAsset(string assetPath, bool isThorough = false)
        {
#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                EditorAssetsMgr.UnLoadAssetEditor(assetPath);
                return;
            }
#endif
            string assetBundleName = GetAssetBundleName(assetPath);
            UnloadAssetBundle(assetBundleName, isThorough);
        }

        /// <summary>
        /// 卸载AB包
        /// </summary>
        public void UnloadAssetBundle(string assetBundleName, bool isThorough)
        {
            Log.LogInfo("开始卸载资源 : [<color=#FF0000>", assetBundleName, "</color>] , ", "卸载前资源数为 : ", m_LoadedAssetBundles.Count.ToString());
            Unload(assetBundleName, isThorough);
            Log.LogInfo("卸载资源 : [<color=#FF0000>", assetBundleName, "</color>] 完成 , ", "卸载后资源数为 : ", m_LoadedAssetBundles.Count.ToString());
        }

        public void UnloadAll(bool isThorough = false)
        {
#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                EditorAssetsMgr.UnLoadAll();
                return;
            }
#endif
            List<string> list = m_LoadedAssetBundles.Keys.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                UnloadAssetBundle(list[i], isThorough);
            }

            list.Clear();
            m_LoadedAssetBundles.Clear();
            m_LoadRequests.Clear();
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        private Object InnerLoadAssetSync(string assetPath, Type assetType)
        {
            string assetBundleName = GetAssetBundleName(assetPath);
            string[] dependencies = GetDependencies(assetBundleName);

            if (dependencies != null && dependencies.Length > 0)
            {
                for (int i = 0; i < dependencies.Length; i++)
                {
                    InnerLoadAssetSync(dependencies[i], typeof(UnityEngine.Object));
                }
            }

            AssetBundleInfo assetBundleInfo = GetLoadedAssetBundle(assetBundleName);

            if (assetBundleInfo == null)
            {
                OnLoadAssetSync(assetBundleName);
                assetBundleInfo = GetLoadedAssetBundle(assetBundleName);

                if (assetBundleInfo == null)
                {
                    Log.LogError("加载失败 , AB包不存在 : ", assetBundleName);
                    return null;
                }
            }

            return assetBundleInfo.assetBundle.LoadAsset(Path.GetFileNameWithoutExtension(assetPath).ToLower(), assetType);
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        private void InnerLoadAssetAsync(string assetPath, Type assetType, GameFrameWorkAction<string, Object, object> loadedAction, bool isDependence, object arg = null)
        {
            string assetBundleName = isDependence ? assetPath : GetAssetBundleName(assetPath);

            if (string.IsNullOrEmpty(assetBundleName))
            {
                return;
            }

            LoadRequest request = LoadRequest.Create(assetPath, assetType, loadedAction, arg);

            if (!m_LoadRequests.TryGetValue(assetBundleName, out List<LoadRequest> listRequests))
            {
                string[] dependencies = GetDependencies(assetBundleName);

                if (dependencies != null && dependencies.Length > 0)
                {
                    for (int i = 0; i < dependencies.Length; i++)
                    {
                        InnerLoadAssetAsync(dependencies[i], typeof(UnityEngine.Object), null, true);
                    }
                }

                listRequests = new() { request };
                m_LoadRequests.Add(assetBundleName, listRequests);

                StartCoroutine(OnLoadAssetAsync(assetBundleName));
            }
            else
            {
                listRequests.Add(request);
            }
        }

        private void OnLoadAssetSync(string assetBundleName)
        {
            string assetBunldePath = GetAssetBundlePath(assetBundleName);
            Log.LogInfo("开始同步加载资源 ：[<color=#FFFF00>", assetBunldePath, "</color>]");

            AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBunldePath);

            if (assetBundle != null)
            {
                m_LoadedAssetBundles.Add(assetBundleName, AssetBundleInfo.Create(assetBundle));
            }
        }

        private IEnumerator OnLoadAssetAsync(string assetBundleName)
        {
            string[] dependencies = GetDependencies(assetBundleName);
            bool hasDependencies = dependencies != null || dependencies.Length > 0;
            bool dependenciesLoaded = false;

            while (hasDependencies && !dependenciesLoaded)
            {
                dependenciesLoaded = true;

                for (int i = 0; i < dependencies.Length; i++)
                {
                    if (GetLoadedAssetBundle(dependencies[i]) == null)
                    {
                        dependenciesLoaded = false;
                        break;
                    }
                }

                yield return null;
            }

            AssetBundleInfo assetBundleInfo = GetLoadedAssetBundle(assetBundleName);

            if (assetBundleInfo == null)
            {
                string assetBundlePath = GetAssetBundlePath(assetBundleName);
                Log.LogInfo("开始异步加载资源 ：<color=#FFFF00>[", assetBundlePath, "</color>]");
                AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);

                while (!createRequest.isDone)
                {
                    yield return null;
                }

                AssetBundle assetBundle = createRequest.assetBundle;

                if (assetBundle != null)
                {
                    m_LoadedAssetBundles.Add(assetBundleName, AssetBundleInfo.Create(assetBundle));
                }

                assetBundleInfo = GetLoadedAssetBundle(assetBundleName);
            }

            if (assetBundleInfo == null)
            {
                Log.LogError("加载失败 , AB包不存在 : ", assetBundleName);
            }

            if (m_LoadRequests.TryGetValue(assetBundleName, out List<LoadRequest> listRequests) && assetBundleInfo != null)
            {
                m_LoadRequests.Remove(assetBundleName);

                for (int i = 0; i < listRequests.Count; i++)
                {
                    if (!assetBundleInfo.assetBundle.isStreamedSceneAssetBundle)
                    {
                        string assetName = Path.GetFileNameWithoutExtension(listRequests[i].assetPath);
                        AssetBundleRequest request = assetBundleInfo.assetBundle.LoadAssetAsync(assetName, listRequests[i].assetType);

                        while (!request.isDone)
                        {
                            yield return null;
                        }

                        listRequests[i].Loaded(request.asset);
                    }
                    else
                    {
                        listRequests[i].Loaded(null);
                    }

                    listRequests[i].Release();
                    assetBundleInfo.referencedCount++;
                }
            }
        }

        private void Unload(string assetBundleName, bool isThorough)
        {
            string[] dependencies = GetDependencies(assetBundleName);

            if (dependencies != null && dependencies.Length > 0)
            {
                for (int i = 0; i < dependencies.Length; i++)
                {
                    Unload(dependencies[i], isThorough);
                }
            }

            AssetBundleInfo assetBundleInfo = GetLoadedAssetBundle(assetBundleName);

            if (assetBundleInfo == null)
            {
                return;
            }

            assetBundleInfo.referencedCount--;

            if (assetBundleInfo.referencedCount <= 0)
            {
                if (m_LoadRequests.ContainsKey(assetBundleName))
                {
                    return;//如果当前AB处于异步加载过程中，卸载会崩溃，只减去引用计数即可
                }

                assetBundleInfo.assetBundle.Unload(isThorough);
                assetBundleInfo.Release();
                m_LoadedAssetBundles.Remove(assetBundleName);
            }
        }

        private AssetBundleInfo GetLoadedAssetBundle(string assetBundleName)
        {
            if (m_LoadedAssetBundles.TryGetValue(assetBundleName, out AssetBundleInfo bundle))
            {
                return bundle;
            }

            return null;
        }

        /// <summary>
        /// 获取所有依赖
        /// </summary>
        private string[] GetDependencies(string assetBundleName)
        {
            if (m_Manifest == null)
            {
                Log.LogError("获取依赖失败 , 请先初始化 assetbundle manifest");
                return null;
            }

            return m_Manifest.GetAllDependencies(assetBundleName);
        }

        private string GetAssetBundleName(string assetPath)
        {
            if (m_AssetsMap == null || m_AssetsMap.Count < 1)
            {
                Log.LogError("获取资源映射失败 : ", assetPath);
            }

            if (!m_AssetsMap.TryGetValue(assetPath, out string assetBundleName))
            {
                Log.LogError("获取资源映射失败 : ", assetPath);
            }

            return assetBundleName.ToLower();
        }

        private string GetAssetBundlePath(string assetBundleName)
        {
            return PathUtil.FormatPath(PathUtil.runTimeAssetsPath, assetBundleName);
        }

        private AssetBundle m_ManifestAssetBundle = null;
        private AssetBundleManifest m_Manifest = null;
        private Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = null;
        private Dictionary<string, List<LoadRequest>> m_LoadRequests = null;
        private Dictionary<string, string> m_AssetsMap = null;
    }
}