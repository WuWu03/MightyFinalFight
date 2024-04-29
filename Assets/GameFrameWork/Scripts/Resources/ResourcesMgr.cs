using GameFrameWork.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFrameWork.Resources
{
    public class ResourcesMgr : BaseMgr<ResourcesMgr>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            m_DicLoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
            m_DicLoadRequests = new Dictionary<string, List<LoadRequest>>();
            m_DicAssetVersions = new Dictionary<string, AssetVersion>();

#if UNITY_EDITOR
            if (!AppConfig.instance.loadAB)
            {
                return;
            }
#endif
            string maniFesturl = PathUtil.FormatPath(PathUtil.runTimeAssetPath, PathUtil.maniFestName);
            string versionUrl = PathUtil.FormatPath(PathUtil.runTimeAssetPath, PathUtil.assetBundleVersionName);

            byte[] stream = File.ReadAllBytes(maniFesturl);
            AssetBundle assetbundle = AssetBundle.LoadFromMemory(stream);
            m_Manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            string[] version = File.ReadAllText(versionUrl).Split('\n');

            for (int i = 0; i < version.Length; i++)
            {
                string[] data = version[i].Split('|');
                if (!data[1].Equals(".manifest"))
                {
                    m_DicAssetVersions.Add(data[0], new AssetVersion(data[0], data[1], data[2]));
                }
            }
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        public T LoadAsset<T>(string assetPath) where T : Object
        {
            Object obj = LoadAsset(assetPath, null, typeof(T));

            if (obj == null)
            {
                return null;
            }

            return obj as T;
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        public Object LoadAsset(string assetPath, Type t = null)
        {
            return LoadAsset(assetPath, null, t);
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        public T LoadAsset<T>(string assetPath, string assetName) where T : Object
        {
            Object obj = LoadAsset(assetPath, assetName, typeof(T));

            if (obj == null)
            {
                return null;
            }

            return obj as T;
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        public Object LoadAsset(string assetPath, string assetName, Type t = null)
        {
            if (t == null)
            {
                t = typeof(Object);
            }
#if UNITY_EDITOR
            if (!AppConfig.instance.loadAB)
            {
                return EditorResourcesMgr.Instance.LoadAssetEditor(assetPath, t);
            }
#endif
            return Load(assetPath, assetName, t);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAssetAsync<T>(string assetPath, GameFrameWorkAction<string, Object, object[]> action = null, params object[] args)
        {
            LoadAssetAsync(assetPath, null, action, typeof(T), args);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAssetAsync(string assetPath, GameFrameWorkAction<string, Object, object[]> action = null, Type t = null, params object[] args)
        {
            LoadAssetAsync(assetPath, null, action, t, args);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAssetAsync<T>(string assetPath, string assetName, GameFrameWorkAction<string, Object, object[]> action = null, params object[] args)
        {
            LoadAssetAsync(assetPath, assetName, action, typeof(T), args);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public void LoadAssetAsync(string assetPath, string assetName, GameFrameWorkAction<string, Object, object[]> action = null, Type t = null, params object[] args)
        {
            if (t == null)
            {
                t = typeof(Object);
            }
#if UNITY_EDITOR
            if (!AppConfig.instance.loadAB)
            {
                EditorResourcesMgr.Instance.LoadAssetEditorAsync(assetPath, action, t, args);
                return;
            }
#endif
            LoadAsync(assetPath, assetName, action, t, args);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        public void UnloadAsset(string assetPath, bool isThorough = false)
        {
#if UNITY_EDITOR
            if(!AppConfig.instance.loadAB)
            {
                EditorResourcesMgr.Instance.UnLoadAssetEditor(assetPath);
                return;
            }
#endif
            string assetBundleName = GetAssetBundleName(assetPath);
            Log.LogInfo("开始卸载资源 : [", assetBundleName, "] , ", "卸载前资源数为 : ", m_DicLoadedAssetBundles.Count);
            UnloadAssetBundle(assetBundleName, isThorough);
            Log.LogInfo("卸载资源 : [", assetBundleName, "] 完成 , ", "卸载后资源数为 : ", m_DicLoadedAssetBundles.Count);
        }

        /// <summary>
        /// 同步加载
        /// </summary>

        private Object Load(string assetPath, string assetName, Type t = null)
        {
            string assetBundleName = GetAssetBundleName(assetPath);
            string[] dependencies = GetDependencies(assetBundleName);

            if (dependencies != null && dependencies.Length > 0)
            {
                for (int i = 0; i < dependencies.Length; i++)
                {
                    Load(dependencies[i], null, typeof(UnityEngine.Object));
                }
            }

            AssetBundleInfo assetBundleInfo = GetLoadedAssetBundle(assetBundleName);

            if (assetBundleInfo == null)
            {
                OnLoadAsset(assetBundleName);
                assetBundleInfo = GetLoadedAssetBundle(assetBundleName);

                if (assetBundleInfo == null)
                {
                    Log.LogError("加载失败 , AB包不存在 : ", assetBundleName);
                    return null;
                }
            }

            if (string.IsNullOrEmpty(assetName))
            {
                return assetBundleInfo.assetBundle.LoadAsset(Path.GetFileNameWithoutExtension(assetBundleName).ToLower(), t);
            }

            return assetBundleInfo.assetBundle.LoadAsset(assetName.ToLower(), t);
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        private void LoadAsync(string assetPath, string assetName, GameFrameWorkAction<string, Object, object[]> action = null, Type t = null, object[] args = null)
        {
            string assetBundleName = GetAssetBundleName(assetPath);

            if (string.IsNullOrEmpty(assetBundleName))
            {
                return;
            }

            LoadRequest request = LoadRequest.Create();
            request.assetPath = assetPath;
            request.assetName = string.IsNullOrEmpty(assetName) ? Path.GetFileNameWithoutExtension(assetBundleName).ToLower() : assetName.ToLower();
            request.assetType = t;
            request.action = action;
            request.args = args;

            if (!m_DicLoadRequests.TryGetValue(assetBundleName, out List<LoadRequest> requests))
            {
                string[] dependencies = GetDependencies(assetBundleName);

                if (dependencies != null && dependencies.Length > 0)
                {
                    for (int i = 0; i < dependencies.Length; i++)
                    {
                        LoadAsync(dependencies[i], null, null, typeof(UnityEngine.Object));
                    }
                }

                requests = new List<LoadRequest>() { request };
                m_DicLoadRequests.Add(assetBundleName, requests);

                StartCoroutine(OnLoadAssetAsync(assetBundleName));
            }
            else
            {
                requests.Add(request);
            }
        }

        private void OnLoadAsset(string assetBundleName)
        {
            string assetBunldePath = GetAssetBundlePath(assetBundleName);
            Log.LogInfo("开始同步加载资源 ：", assetBunldePath);

            AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBunldePath);

            if (assetBundle != null)
            {
                m_DicLoadedAssetBundles.Add(assetBundleName, new AssetBundleInfo(assetBundle));
            }
        }

        private IEnumerator OnLoadAssetAsync(string assetBundleName)
        {
            yield return null;
            AssetBundleInfo assetBundleInfo = GetLoadedAssetBundle(assetBundleName);

            if (assetBundleInfo == null)
            {
                string assetBundlePath = GetAssetBundlePath(assetBundleName);
                Log.LogInfo("开始异步加载资源 ：", assetBundlePath);
                AssetBundleCreateRequest createRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);

                while (!createRequest.isDone)
                {
                    yield return null;
                }

                AssetBundle assetBundle = createRequest.assetBundle;

                if (assetBundle != null)
                {
                    m_DicLoadedAssetBundles.Add(assetBundleName, new AssetBundleInfo(assetBundle));
                }

                assetBundleInfo = GetLoadedAssetBundle(assetBundleName);

                if (assetBundleInfo == null)
                {
                    m_DicLoadRequests.Remove(assetBundleName);
                    Log.LogError("加载失败 , AB包不存在 : ", assetBundleName);
                    yield break;
                }
            }

            if (m_DicLoadRequests.TryGetValue(assetBundleName, out List<LoadRequest> list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    assetBundleInfo.assetBundle.LoadAssetAsync(list[i]);
                    assetBundleInfo.referencedCount++;
                }
            }

            m_DicLoadRequests.Remove(assetBundleName);
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


        private void UnloadAssetBundle(string assetBundleName, bool isThorough)
        {
            string[] dependencies = GetDependencies(assetBundleName);

            if (dependencies != null && dependencies.Length > 0)
            {
                for (int i = 0; i < dependencies.Length; i++)
                {
                    UnloadAssetBundle(dependencies[i], isThorough);
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
                m_DicLoadedAssetBundles.Remove(assetBundleName);
            }
        }

        private string GetAssetBundleName(string assetPath)
        {
            string assetName = Path.GetFileNameWithoutExtension(assetPath);
            string path = Path.GetDirectoryName(assetPath).Replace("\\", "/");
            string assetBundleName = assetName;

            if (!string.IsNullOrEmpty(path))
            {
                assetBundleName = PathUtil.FormatPath(path, assetName).ToLower();
            }

            if (m_DicAssetVersions.TryGetValue(assetBundleName, out AssetVersion version))
            {
                return StringUtil.Format(assetBundleName, version.extendName);
            }

            Log.LogError("获取资源版本失败 : ", assetPath);

            if (!assetBundleName.EndsWith(PathUtil.assetBundleExtension))
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
            if (!AppConfig.instance.loadAB)
            {
                EditorResourcesMgr.Instance.UnLoadAll();
                return;
            }
#endif

            List<string> list = m_DicLoadedAssetBundles.Keys.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                UnloadAsset(list[i], true);
            }

            list.Clear();
            m_DicLoadedAssetBundles.Clear();
            m_DicLoadRequests.Clear();
            m_DicAssetVersions.Clear();
        }

        private AssetBundleManifest m_Manifest;
        private Dictionary<string, AssetBundleInfo> m_DicLoadedAssetBundles = null;
        private Dictionary<string, List<LoadRequest>> m_DicLoadRequests = null;
        private Dictionary<string, AssetVersion> m_DicAssetVersions = null;
    }
}