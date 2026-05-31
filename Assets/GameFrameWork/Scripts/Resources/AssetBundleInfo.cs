using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFrameWork.Resources
{
    public class AssetBundleInfo : GameFrameWorkEventArg
    {
        public AssetBundle assetBundle { get; set; }
        public int referencedCount { get; set; }

        private Queue<LoadRequest> m_LoadRequests = null;
        private LoadRequest m_CurrLoadRequest = null;
        private AssetBundleRequest m_AssetBundleRequest = null;
        private bool m_IsLoading = false;

        public AssetBundleInfo()
        {
            m_LoadRequests = new();
        }

        public static AssetBundleInfo Create(AssetBundle assetBundle)
        {
            AssetBundleInfo assetBundleInfo = ReferencePool.Acquire<AssetBundleInfo>();
            assetBundleInfo.assetBundle = assetBundle;
            assetBundleInfo.referencedCount = 0;

            return assetBundleInfo;
        }

        public void LoadAssetAsync(List<LoadRequest> loadRequests)
        {
            foreach (LoadRequest loadRequest in loadRequests)
            {
                m_LoadRequests.Enqueue(loadRequest);
            }

            if (m_LoadRequests.Count > 0)
            {
                MonoBehaviourMgr.instance.updateEvent += Update;
                m_IsLoading = true;
            }
            else
            {
                MonoBehaviourMgr.instance.updateEvent -= Update;
                m_IsLoading = false;
            }
        }

        public override void Clear()
        {
            MonoBehaviourMgr.instance.updateEvent -= Update;
            m_LoadRequests.Clear();
            m_AssetBundleRequest = null;
            assetBundle = null;
            referencedCount = 0;
        }

        private void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            if (!m_IsLoading)
            {
                return;
            }

            if (assetBundle.isStreamedSceneAssetBundle)
            {
                while (m_LoadRequests.Count > 0)
                {
                    m_CurrLoadRequest = m_LoadRequests.Dequeue();
                    m_CurrLoadRequest.Loaded(null);
                    referencedCount++;
                    m_CurrLoadRequest.Release();
                }

                MonoBehaviourMgr.instance.updateEvent -= Update;
                m_IsLoading = false;
                return;
            }

            if (m_AssetBundleRequest == null)
            {
                m_CurrLoadRequest = m_LoadRequests.Dequeue();
                string assetName = Path.GetFileNameWithoutExtension(m_CurrLoadRequest.assetPath);
                m_AssetBundleRequest = assetBundle.LoadAssetAsync(assetName, m_CurrLoadRequest.assetType);
            }
            else if (m_AssetBundleRequest.isDone)
            {
                m_CurrLoadRequest.Loaded(m_AssetBundleRequest.asset);
                referencedCount++;
                m_CurrLoadRequest.Release();
                m_CurrLoadRequest = null;
                m_AssetBundleRequest = null;

                if (m_LoadRequests.Count < 1)
                {
                    MonoBehaviourMgr.instance.updateEvent -= Update;
                    m_IsLoading = false;
                }
            }
        }
    }
}