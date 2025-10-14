using GameFrameWork.Assets;
using GameFrameWork.Utils;
using System;
using System.Collections.Generic;
using GameFrameWork.Event;
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
            m_LoadedAssets = new();
            m_LoadRequests = new();
            m_RemoveList = new();
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();

            foreach (KeyValuePair<string, PoolObjectInfo> kvp in m_LoadedAssets)
            {
                AssetsMgr.instance.UnloadAsset(kvp.Value.assetPath);
            }

            m_RemoveList.Clear();
            m_LoadedAssets.Clear();
            m_LoadRequests.Clear();
        }

        protected override void OnDestory()
        {
            base.OnDestory();

            m_RemoveList = null;
            m_LoadedAssets = null;
            m_LoadRequests = null;
        }

        public void CheckRelease()
        {
            if (m_LoadedAssets == null || m_LoadedAssets.Count < 1)
            {
                return;
            }

            m_RemoveList.Clear();

            foreach (KeyValuePair<string, PoolObjectInfo> kvp in m_LoadedAssets)
            {
                PoolObjectInfo info = kvp.Value;

                if (info.releaseTime > 0 && Time.time - info.releaseTime >= ConstField.CollectTime)
                {

                    AssetsMgr.instance.UnloadAsset(info.assetPath, false);
                    info.Release();
                    m_RemoveList.Add(kvp.Key);
                }
            }

            for (int i = 0; i < m_RemoveList.Count; i++)
            {
                m_LoadedAssets.Remove(m_RemoveList[i]);
            }

            UnityEngine.Resources.UnloadUnusedAssets();
        }

        public void Cache<T>(string assetPath) where T : UnityEngine.Object
        {
            Get(assetPath, typeof(T), null);
        }

        public void Cache(string assetPath, Type assetType)
        {
            Get(assetPath, assetType, null);
        }

        public void Get<T>(string assetPath, GameFrameWorkAction<string, UnityEngine.Object, object> loadedAction, object arg = null) where T : UnityEngine.Object
        {
            Get(assetPath, typeof(T), loadedAction, arg);
        }

        public void Get(string assetPath, Type assetType, GameFrameWorkAction<string, UnityEngine.Object, object> loadedAction, object arg = null)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Log.LogError("资源路径无效");
                return;
            }

            if (m_LoadedAssets.TryGetValue(assetPath, out PoolObjectInfo info))
            {
                UnityEngine.Object obj = info.poolObject;
                info.referenceCount++;
                info.releaseTime = -1;
                loadedAction?.Invoke(assetPath, obj, arg);
                return;
            }

            LoadRequest request = LoadRequest.Create(assetPath, assetType, loadedAction, arg);

            if (!m_LoadRequests.TryGetValue(assetPath, out List<LoadRequest> listLoadRequest))
            {
                listLoadRequest = new List<LoadRequest>() { request };
                m_LoadRequests.Add(assetPath, listLoadRequest);
                AssetsMgr.instance.LoadAssetAsync(assetPath, assetType, OnLoaded, arg);
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

            if (!m_LoadedAssets.TryGetValue(assetPath, out PoolObjectInfo info))
            {
                info = PoolObjectInfo.Create(obj, Time.time, false, assetPath);
                m_LoadedAssets.Add(assetPath, info);
            }

            info.releaseTime = Time.time;
            info.referenceCount--;
            info.isReleaseImmediate = false;
        }

        private void OnLoaded(string assetPath, UnityEngine.Object obj, object arg)
        {
            if (!m_LoadRequests.TryGetValue(assetPath, out List<LoadRequest> listLoadRequest))
            {
                Log.LogError(StringUtil.Append("[", assetPath, "] 资源加载完成 , 但加载回调不存在"));
                return;
            }

            if (!m_LoadedAssets.TryGetValue(assetPath, out PoolObjectInfo info))
            {
                info = PoolObjectInfo.Create(obj, -1, false, assetPath);
                m_LoadedAssets.Add(assetPath, info);
            }

            info.releaseTime = -1;

            for (int i = 0; i < listLoadRequest.Count; i++)
            {
                if (listLoadRequest[i].loadedAction != null)
                {
                    info.referenceCount++;
                }

                listLoadRequest[i].Loaded(obj);
                listLoadRequest[i].Release();
            }

            m_LoadRequests.Remove(assetPath);
        }


        protected Transform m_PoolRoot = null;
        private List<string> m_RemoveList = null;
        private Dictionary<string, PoolObjectInfo> m_LoadedAssets = null;
        private Dictionary<string, List<LoadRequest>> m_LoadRequests = null;
    }
}