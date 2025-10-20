using GameFrameWork.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFrameWork.Event;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace GameFrameWork.Assets
{
    public class ResourceMgr : GameFrameWorkModule,IResourceMgr
    {
        private readonly Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles;
        private readonly Dictionary<string, List<LoadRequest>> m_LoadRequests;
        private readonly Dictionary<string, string> m_AssetsMap;
        private AssetBundle m_ManifestAssetBundle;
        private AssetBundleManifest m_Manifest;
        
        public ResourceMgr()
        {
            m_LoadedAssetBundles = new();
            m_LoadRequests = new();
            m_AssetsMap = new();
            InitAssetsMap();
        }

        public override void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            
        }

        public override void Shutdown()
        {
            UnloadAll(true);
            m_ManifestAssetBundle?.Unload(true);
            m_AssetsMap.Clear();
            m_ManifestAssetBundle = null;
            m_Manifest = null;
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

            foreach (string assetInfo in assetsMap)
            {
                string[] data = assetInfo.Split('|', 2);
                m_AssetsMap.Add(data[0], data[1]);
            }
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        public T Load<T>(string assetPath) where T : UnityObject
        {
            UnityObject obj = Load(assetPath, typeof(T));

            if (obj == null)
            {
                return null;
            }

            return obj as T;
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        public UnityObject Load(string assetPath, Type assetType = null)
        {
            assetType ??= typeof(UnityObject);
#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                return EditorResourceMgr.LoadAssetSync(assetPath, assetType);
            }
#endif
            return InnerLoad(assetPath, assetType);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAsync<T>(string assetPath, GameFrameWorkAction<string, UnityObject, object> loadedAction, object arg = null) where T : UnityObject
        {
            LoadAsync(assetPath, typeof(T), loadedAction, arg);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAsync(string assetPath, Type assetType, GameFrameWorkAction<string, UnityObject, object> loadedAction, object arg = null)
        {
            assetType ??= typeof(UnityObject);
#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                EditorResourceMgr.LoadAssetAsync(assetPath, assetType, loadedAction, arg);
                return;
            }
#endif
            InnerLoadAsync(assetPath, assetType, loadedAction, false, arg);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        public void Unload(string assetPath, bool isThorough = false)
        {
#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                EditorResourceMgr.UnLoadAssetEditor(assetPath);
                return;
            }
#endif
            string assetBundleName = GetAssetBundleName(assetPath);
            InnerUnload(assetBundleName, isThorough);
        }
        
        public void UnloadAll(bool isThorough = false)
        {
#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                EditorResourceMgr.UnLoadAll();
                return;
            }
#endif
            List<string> list = m_LoadedAssetBundles.Keys.ToList();

            foreach (string assetBundleName in list)
            {
                InnerUnload(assetBundleName, isThorough);
            }

            list.Clear();
            m_LoadedAssetBundles.Clear();
            m_LoadRequests.Clear();
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        private UnityObject InnerLoad(string assetPath, Type assetType)
        {
            string assetBundleName = GetAssetBundleName(assetPath);
            string[] dependencies = GetDependencies(assetBundleName);

            if (dependencies != null && dependencies.Length > 0)
            {
                for (int i = 0; i < dependencies.Length; i++)
                {
                    InnerLoad(dependencies[i], typeof(UnityEngine.Object));
                }
            }

            AssetBundleInfo assetBundleInfo = GetLoadedAssetBundle(assetBundleName);

            if (assetBundleInfo == null)
            {
                OnLoad(assetBundleName);
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
        private void InnerLoadAsync(string assetPath, Type assetType, GameFrameWorkAction<string, UnityObject, object> loadedAction, bool isDependence, object arg = null)
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

                if (dependencies is { Length: > 0 })
                {
                    foreach (var dependency in dependencies)
                    {
                        InnerLoadAsync(dependency, typeof(UnityObject), null, true);
                    }
                }

                listRequests = new() { request };
                m_LoadRequests.Add(assetBundleName, listRequests);
                MonoBehaviourMgr.instance.StartCoroutine(OnLoadAsync(assetBundleName));
            }
            else
            {
                listRequests.Add(request);
            }
        }

        private void OnLoad(string assetBundleName)
        {
            string assetBundlePath = GetAssetBundlePath(assetBundleName);
            Log.LogInfo("开始同步加载资源 ：[<color=#FFFF00>", assetBundlePath, "</color>]");
            AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);

            if (assetBundle != null)
            {
                m_LoadedAssetBundles.Add(assetBundleName, AssetBundleInfo.Create(assetBundle));
            }
        }

        private IEnumerator OnLoadAsync(string assetBundleName)
        {
            string[] dependencies = GetDependencies(assetBundleName);
            bool hasDependencies = dependencies is { Length: > 0 };
            bool dependenciesLoaded = false;

            while (hasDependencies && !dependenciesLoaded)
            {
                dependenciesLoaded = true;

                foreach (string dependency in dependencies)
                {
                    if (GetLoadedAssetBundle(dependency) == null)
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
                AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
                
                while (!assetBundleCreateRequest.isDone)
                {
                    yield return null;
                }
                
                AssetBundle assetBundle = assetBundleCreateRequest.assetBundle;

                if (assetBundle != null)
                {
                    m_LoadedAssetBundles.Add(assetBundleName, AssetBundleInfo.Create(assetBundle));
                }

                assetBundleInfo = GetLoadedAssetBundle(assetBundleName);
            }

            if (assetBundleInfo == null)
            {
                throw new Exception(StringUtil.Append("加载 [", assetBundleName,"] 失败，AB包不存在"));
            }

            if (!m_LoadRequests.Remove(assetBundleName, out List<LoadRequest> loadRequests))
            {
                throw new Exception(StringUtil.Append("加载 [", assetBundleName,"] 成功，但回调不存在"));
            }
            
            foreach (LoadRequest loadRequest in loadRequests)
            {
                if (!assetBundleInfo.assetBundle.isStreamedSceneAssetBundle)
                {
                    string assetName = Path.GetFileNameWithoutExtension(loadRequest.assetPath);
                    AssetBundleRequest assetBundleRequest = assetBundleInfo.assetBundle.LoadAssetAsync(assetName, loadRequest.assetType);
           
                    while (!assetBundleRequest.isDone)
                    {
                        yield return null;
                    }
                    
                    loadRequest.Loaded(assetBundleRequest.asset);
                }
                else
                {
                    loadRequest.Loaded(null);
                }

                loadRequest.Release();
                assetBundleInfo.referencedCount++;
            }
        }

        private void InnerUnload(string assetBundleName, bool isThorough)
        {
            string[] dependencies = GetDependencies(assetBundleName);

            if (dependencies is { Length: > 0 })
            {
                foreach (var dependency in dependencies)
                {
                    InnerUnload(dependency, isThorough);
                }
            }

            AssetBundleInfo assetBundleInfo = GetLoadedAssetBundle(assetBundleName);

            if (assetBundleInfo == null)
            {
                return;
            }

            assetBundleInfo.referencedCount--;
            Log.LogInfo("开始卸载资源 : [<color=#FF0000>", assetBundleName, "</color>] , ", "卸载前资源数为 : ", m_LoadedAssetBundles.Count.ToString());
            //如果当前AB处于异步加载过程中，卸载会崩溃，只减去引用计数即可
            if (assetBundleInfo.referencedCount <= 0 && !m_LoadRequests.ContainsKey(assetBundleName))
            {
                assetBundleInfo.assetBundle.Unload(isThorough);
                assetBundleInfo.Release();
                m_LoadedAssetBundles.Remove(assetBundleName);
            }
            Log.LogInfo("卸载资源 : [<color=#FF0000>", assetBundleName, "</color>] 完成 , ", "卸载后资源数为 : ", m_LoadedAssetBundles.Count.ToString());
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
                throw new Exception("获取依赖失败 , 请先初始化 AssetBundleManifest");
            }

            return m_Manifest.GetAllDependencies(assetBundleName);
        }

        private string GetAssetBundleName(string assetPath)
        {
            if (m_AssetsMap == null || m_AssetsMap.Count < 1)
            {
                throw new Exception(StringUtil.Append("获取资源映射失败 : ", assetPath));
            }

            if (!m_AssetsMap.TryGetValue(assetPath, out string assetBundleName))
            {
                throw new Exception(StringUtil.Append("获取资源映射失败 : ", assetPath));
            }

            return assetBundleName.ToLower();
        }

        private string GetAssetBundlePath(string assetBundleName)
        {
            return PathUtil.FormatPath(PathUtil.runTimeAssetsPath, assetBundleName);
        }
    }
}