using GameFrameWork.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFrameWork.Assets
{
    public class AssetsMgr : BaseMgr<AssetsMgr>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            m_DicLoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
            m_DicLoadRequests = new Dictionary<string, List<LoadRequest>>();
            m_DicAssetMap = new Dictionary<string, string>();

#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                return;
            }
#endif
            string maniFesturl = PathUtil.FormatPath(PathUtil.runTimeAssetPath, PathUtil.maniFestName);
            string assetMapUrl = PathUtil.FormatPath(PathUtil.runTimeAssetPath, PathUtil.assetMapName);

            byte[] stream = File.ReadAllBytes(maniFesturl);
            AssetBundle assetbundle = AssetBundle.LoadFromMemory(stream);
            m_Manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            string[] assetMap = File.ReadAllText(assetMapUrl).Split('\n');

            for (int i = 0; i < assetMap.Length; i++)
            {
                string[] data = assetMap[i].Split('|', 2);
                m_DicAssetMap.Add(data[0], data[1]);
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
        public Object LoadAssetSync(string assetPath, Type t = null)
        {
            if (t == null)
            {
                t = typeof(Object);
            }
#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                return EditorAssetsMgr.Instance.LoadAssetSync(assetPath, t);
            }
#endif
            return InnerLoadAssetSync(assetPath, t);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAssetAsync<T>(string assetPath, GameFrameWorkAction<string, Object, object[]> action = null) where T : Object
        {
            LoadAssetAsync(assetPath, action, typeof(T));
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAssetAsync<T>(string assetPath, GameFrameWorkAction<string, Object, object[]> action = null, params object[] args) where T : Object
        {
            LoadAssetAsync(assetPath, action, typeof(T), args);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAssetAsync(string assetPath, GameFrameWorkAction<string, Object, object[]> action = null, Type t = null)
        {
            LoadAssetAsync(assetPath, action, t, null);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAssetAsync(string assetPath, GameFrameWorkAction<string, Object, object[]> action, Type t, params object[] args)
        {
            if (t == null)
            {
                t = typeof(Object);
            }
#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                EditorAssetsMgr.Instance.LoadAssetAsync(assetPath, action, t, args);
                return;
            }
#endif
            InnerLoadAssetAsync(assetPath, false, action, t, args);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        public void UnloadAsset(string assetPath, bool isThorough = false)
        {
#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                EditorAssetsMgr.Instance.UnLoadAssetEditor(assetPath);
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
            Log.LogInfo("开始卸载资源 : [<color=#FF0000>", assetBundleName, "</color>] , ", "卸载前资源数为 : ", m_DicLoadedAssetBundles.Count);
            Unload(assetBundleName, isThorough);
            Log.LogInfo("卸载资源 : [<color=#FF0000>", assetBundleName, "</color>] 完成 , ", "卸载后资源数为 : ", m_DicLoadedAssetBundles.Count);
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        private Object InnerLoadAssetSync(string assetPath, Type t = null)
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

            return assetBundleInfo.assetBundle.LoadAsset(Path.GetFileNameWithoutExtension(assetPath).ToLower(), t);
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        private void InnerLoadAssetAsync(string assetPath, bool isDependence, GameFrameWorkAction<string, Object, object[]> action = null, Type t = null, object[] args = null)
        {
            string assetBundleName = isDependence ? assetPath : GetAssetBundleName(assetPath);

            if (string.IsNullOrEmpty(assetBundleName))
            {
                return;
            }

            LoadRequest request = LoadRequest.Create();
            request.assetPath = assetPath;
            request.assetType = t;
            request.action = action;
            request.args = args;

            if (!m_DicLoadRequests.TryGetValue(assetBundleName, out List<LoadRequest> listRequests))
            {
                string[] dependencies = GetDependencies(assetBundleName);

                if (dependencies != null && dependencies.Length > 0)
                {
                    for (int i = 0; i < dependencies.Length; i++)
                    {
                        InnerLoadAssetAsync(dependencies[i], true, null, typeof(UnityEngine.Object), args);
                    }
                }

                listRequests = new List<LoadRequest>() { request };
                m_DicLoadRequests.Add(assetBundleName, listRequests);

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
                m_DicLoadedAssetBundles.Add(assetBundleName, AssetBundleInfo.Create(assetBundle));
            }
        }

        private IEnumerator OnLoadAssetAsync(string assetBundleName)
        {
            yield return null;
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
                    m_DicLoadedAssetBundles.Add(assetBundleName, AssetBundleInfo.Create(assetBundle));
                }

                assetBundleInfo = GetLoadedAssetBundle(assetBundleName);
            }

            if (assetBundleInfo == null)
            {
                Log.LogError("加载失败 , AB包不存在 : ", assetBundleName);
            }

            if (m_DicLoadRequests.TryGetValue(assetBundleName, out List<LoadRequest> listRequests) && assetBundleInfo != null)
            {
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

                        listRequests[i].Call(request.asset);
                    }
                    else
                    {
                        listRequests[i].Call(null);
                    }

                    assetBundleInfo.referencedCount++;
                }
            }

            for (int i = 0; i < listRequests.Count; i++)
            {
                ReferencePool.ReleaseReference(listRequests[i]);
            }

            m_DicLoadRequests.Remove(assetBundleName);
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
                if (m_DicLoadRequests.ContainsKey(assetBundleName))
                {
                    return;//如果当前AB处于异步加载过程中，卸载会崩溃，只减去引用计数即可
                }

                assetBundleInfo.assetBundle.Unload(isThorough);
                ReferencePool.ReleaseReference(assetBundleInfo);
                m_DicLoadedAssetBundles.Remove(assetBundleName);
            }
        }

        private AssetBundleInfo GetLoadedAssetBundle(string assetBundleName)
        {
            if (m_DicLoadedAssetBundles.TryGetValue(assetBundleName, out AssetBundleInfo bundle))
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
            if (m_DicAssetMap == null && m_DicAssetMap.Count < 1)
            {
                Log.LogError("获取资源映射失败 : ", assetPath);
            }

            if (!m_DicAssetMap.TryGetValue(assetPath, out string assetBundleName))
            {
                Log.LogError("获取资源映射失败 : ", assetPath);
            }

            assetBundleName = assetBundleName.ToLower();
            string ext = Path.GetExtension(assetBundleName);

            if (string.IsNullOrEmpty(ext))
            {
                return StringUtil.Format(assetBundleName, PathUtil.assetBundleExtension);
            }

            return assetBundleName;
        }

        private string GetAssetBundlePath(string assetBundleName)
        {
            return PathUtil.FormatPath(PathUtil.runTimeAssetPath, assetBundleName);
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();

#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                EditorAssetsMgr.Instance.UnLoadAll();
                return;
            }
#endif
            List<string> list = m_DicLoadedAssetBundles.Keys.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                UnloadAssetBundle(list[i], false);
            }

            list.Clear();
            m_DicLoadedAssetBundles.Clear();
            m_DicLoadRequests.Clear();
            m_DicAssetMap.Clear();
        }

        private AssetBundleManifest m_Manifest;
        private Dictionary<string, AssetBundleInfo> m_DicLoadedAssetBundles = null;
        private Dictionary<string, List<LoadRequest>> m_DicLoadRequests = null;
        private Dictionary<string, string> m_DicAssetMap = null;
    }
}