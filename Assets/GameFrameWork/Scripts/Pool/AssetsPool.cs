using GameFrameWork.Assets;
using GameFrameWork.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Pool
{
    public class AssetsPool : BaseMgr<AssetsPool>
    {
        protected override void OnAwake()
        {
            m_PoolRoot = new GameObject("AssetsPool").transform;
            m_PoolRoot.SetParent(transform, false);
            m_PoolRoot.localPosition = new Vector3(9999f, 9999f, 9999f);
            m_DicLoadedAssets = new Dictionary<string, PoolObjectInfo>();
            m_DicLoadRequests = new Dictionary<string, List<LoadRequest>>();
            m_RemoveList = new List<string>();
        }

        public void CheckRelease()
        {
            if (m_DicLoadedAssets == null || m_DicLoadedAssets.Count < 1)
            {
                return;
            }

            m_RemoveList.Clear();

            foreach (KeyValuePair<string, PoolObjectInfo> kvp in m_DicLoadedAssets)
            {
                PoolObjectInfo info = kvp.Value;

                if (info.releaseTime > 0 && Time.time - info.releaseTime >= ConstField.CollectTime)
                {
                    AssetsMgr.instance.UnloadAsset(info.assetPath, false);
                    ReferencePool.ReleaseReference(info);
                    m_RemoveList.Add(kvp.Key);
                }
            }

            for (int i = 0; i < m_RemoveList.Count; i++)
            {
                m_DicLoadedAssets.Remove(m_RemoveList[i]);
            }

            UnityEngine.Resources.UnloadUnusedAssets();
        }

        public void Cache<T>(string assetPath) where T : UnityEngine.Object
        {
            Get(assetPath, null, typeof(T));
        }

        public void Cache(string assetPath, Type t)
        {
            Get(assetPath, null, t);
        }

        public void Get<T>(string assetPath, GameFrameWorkAction<string, UnityEngine.Object, object[]> call, params object[] args) where T : UnityEngine.Object
        {
            Get(assetPath, call, typeof(T), args);
        }

        public void Get(string assetPath, GameFrameWorkAction<string, UnityEngine.Object, object[]> call, Type t, params object[] args)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Log.LogError("资源路径无效");
                return;
            }

            if (m_DicLoadedAssets.TryGetValue(assetPath, out PoolObjectInfo info))
            {
                UnityEngine.Object obj = info.poolObject;
                info.referenceCount++;
                call?.Invoke(assetPath, obj, args);
                return;
            }

            LoadRequest request = LoadRequest.Create();
            request.assetPath = assetPath;
            request.action = call;
            request.args = args;

            if (!m_DicLoadRequests.TryGetValue(assetPath, out List<LoadRequest> listLoadRequest))
            {
                listLoadRequest = new List<LoadRequest>() { request };
                m_DicLoadRequests.Add(assetPath, listLoadRequest);
                AssetsMgr.instance.LoadAssetAsync(assetPath, OnLoaded, t);
            }   
            else
            {
                listLoadRequest.Add(request);
            }
        }


        public void Put(string assetPath, UnityEngine.Object obj)
        {
            if (string.IsNullOrEmpty(assetPath) || obj == null)
            {
                return;
            }

            if (!m_DicLoadedAssets.TryGetValue(assetPath, out PoolObjectInfo info))
            {
                info = PoolObjectInfo.Create(obj, Time.time, false, assetPath);
                m_DicLoadedAssets.Add(assetPath, info);
            }

            info.releaseTime = Time.time;
            info.isReleaseImmediate = false;
        }

        private void OnLoaded(string assetPath, UnityEngine.Object obj, object[] args)
        {
            if (!m_DicLoadRequests.TryGetValue(assetPath, out List<LoadRequest> listLoadRequest))
            {
                Log.LogError(StringUtil.Format("[", assetPath, "] 资源加载完成 , 但加载回调不存在"));
                return;
            }

            if (!m_DicLoadedAssets.TryGetValue(assetPath, out PoolObjectInfo info))
            {
                info = PoolObjectInfo.Create(obj, -1, false, assetPath);
                m_DicLoadedAssets.Add(assetPath, info);
            }

            for (int i = 0; i < listLoadRequest.Count; i++)
            {
                if (listLoadRequest[i].action != null)
                {
                    info.referenceCount++;
                }

                listLoadRequest[i].Call(obj);
                ReferencePool.ReleaseReference(listLoadRequest[i]);
            }

            m_DicLoadRequests.Remove(assetPath);
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();

            foreach(KeyValuePair<string, PoolObjectInfo> kvp in m_DicLoadedAssets)
            {
                AssetsMgr.instance.UnloadAsset(kvp.Value.assetPath);
            }

            m_DicLoadedAssets.Clear();
            m_DicLoadRequests.Clear();
            m_RemoveList.Clear();

            m_DicLoadedAssets = null;
            m_DicLoadRequests = null;
            m_RemoveList = null;
        }

        protected Transform m_PoolRoot = null;
        private List<string> m_RemoveList = null;
        private Dictionary<string, PoolObjectInfo> m_DicLoadedAssets = null;
        private Dictionary<string, List<LoadRequest>> m_DicLoadRequests = null;
    }
}