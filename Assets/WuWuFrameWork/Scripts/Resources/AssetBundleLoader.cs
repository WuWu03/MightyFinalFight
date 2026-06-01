using WuWuFramework.Event;
using UnityEngine;

namespace WuWuFramework.Resources
{
    public class AssetBundleLoader : WuWuFrameworkEventArg
    {
        public bool isDone { get; private set; }
        public string assetBundleName { get; private set; }
        public string assetBundlePath { get; private set; }
        public AssetBundleInfo assetBundleInfo { get; private set; }

        private AssetBundleCreateRequest m_AssetBundleCreateRequest;
        private WuWuFrameworkFunc<string, AssetBundleInfo> m_GetLoadedAssetBundleFunc;
        private WuWuFrameworkFunc<string, string[]> m_GetDependenciesFunc;
        private WuWuFrameworkAction<AssetBundleLoader> m_OnLoadCompleteAction;

        public static AssetBundleLoader Create(string assetBundleName, string assetBundlePath, bool isDependence,
                                               WuWuFrameworkFunc<string, AssetBundleInfo> getLoadedAssetBundleFunc,
                                               WuWuFrameworkFunc<string, string[]> getDependenciesFunc,
                                               WuWuFrameworkAction<AssetBundleLoader> m_OnLoadCompleteAction)
        {
            AssetBundleLoader assetBundleLoader = ReferencePool.Acquire<AssetBundleLoader>();
            assetBundleLoader.assetBundleName = assetBundleName;
            assetBundleLoader.assetBundlePath = assetBundlePath;
            assetBundleLoader.m_GetLoadedAssetBundleFunc = getLoadedAssetBundleFunc;
            assetBundleLoader.m_GetDependenciesFunc = getDependenciesFunc;
            assetBundleLoader.m_OnLoadCompleteAction = m_OnLoadCompleteAction;
            MonoBehaviourMgr.instance.updateEvent += assetBundleLoader.Update;
            Log.LogInfo("创建资源加载器 ：<color=#FF4500>[", assetBundlePath, "</color>] ，", "是否为依赖资源 : ", isDependence.ToString());
            return assetBundleLoader;
        }

        private void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            if (isDone)
            {
                return;
            }

            string[] dependencies = m_GetDependenciesFunc(assetBundleName);
            bool hasDependencies = dependencies is { Length: > 0 };
            bool dependenciesLoaded = false;

            if (hasDependencies && !dependenciesLoaded)
            {
                dependenciesLoaded = true;

                foreach (string dependency in dependencies)
                {
                    if (m_GetLoadedAssetBundleFunc(dependency) == null)
                    {
                        dependenciesLoaded = false;
                        break;
                    }
                }

                if (!dependenciesLoaded)
                {
                    return;
                }
            }

            assetBundleInfo = m_GetLoadedAssetBundleFunc(assetBundleName);

            if (assetBundleInfo == null)
            {
                if (m_AssetBundleCreateRequest == null)
                {
                    Log.LogInfo("开始异步加载资源 ：<color=#FFFF00>[", assetBundlePath, "</color>]");
                    m_AssetBundleCreateRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
                }

                if (!m_AssetBundleCreateRequest.isDone)
                {
                    return;
                }

                isDone = true;
                assetBundleInfo = AssetBundleInfo.Create(m_AssetBundleCreateRequest.assetBundle);
                m_OnLoadCompleteAction?.Invoke(this);
            }
            else
            {
                isDone = true;
                m_OnLoadCompleteAction?.Invoke(this);
            }
        }

        public override void Clear()
        {
            MonoBehaviourMgr.instance.updateEvent -= Update;
            isDone = false;
            assetBundleName = string.Empty;
            assetBundlePath = string.Empty;
            assetBundleInfo = null;
            m_AssetBundleCreateRequest = null;
            m_GetLoadedAssetBundleFunc = null;
            m_GetDependenciesFunc = null;
            m_OnLoadCompleteAction = null;
        }
    }
}