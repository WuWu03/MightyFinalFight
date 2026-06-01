using WuWuFramework.Resources;
using WuWuFramework.Utils;
using System;
using System.Collections.Generic;
using WuWuFramework.Event;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace WuWuFramework.Pool
{
    public class ResourcePoolMgr : WuWuFrameworkModule,IResourcePoolMgr
    {
        private readonly List<string> m_RemoveList;
        private readonly Dictionary<string, PoolObjectInfo> m_LoadedAssets;
        private readonly Dictionary<string, List<LoadRequest>> m_LoadRequests;
        private IResourcesMgr m_ResourceMgr;
        private Transform m_PoolRoot;
        public ResourcePoolMgr()
        {
            m_LoadedAssets = new();
            m_LoadRequests = new();
            m_RemoveList = new();
        }

        public override void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            
        }

        public override void Shutdown()
        {
            foreach (KeyValuePair<string, PoolObjectInfo> kvp in m_LoadedAssets)
            {
                m_ResourceMgr.Unload(kvp.Value.assetPath);
            }

            m_RemoveList.Clear();
            m_LoadedAssets.Clear();
            m_LoadRequests.Clear();
        }

        public void SetResourceMgr(IResourcesMgr resourceMgr, Transform poolRoot)
        {
            m_ResourceMgr = resourceMgr;
            m_PoolRoot = new GameObject("ResourcePool").transform;
            m_PoolRoot.SetParent(poolRoot, false);
            m_PoolRoot.localPosition = new Vector3(9999f, 9999f, 9999f);
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

                    m_ResourceMgr.Unload(info.assetPath, false);
                    info.Release();
                    m_RemoveList.Add(kvp.Key);
                }
            }

            foreach (var assetName in m_RemoveList)
            {
                m_LoadedAssets.Remove(assetName);
            }

            UnityEngine.Resources.UnloadUnusedAssets();
        }

        public void Cache<T>(string assetPath) where T : UnityObject
        {
            Get(assetPath, typeof(T), null);
        }

        public void Cache(string assetPath, Type assetType)
        {
            Get(assetPath, assetType, null);
        }

        public void Get<T>(string assetPath, WuWuFrameworkAction<string, UnityObject, object> loadedAction, object arg = null) where T : UnityObject
        {
            Get(assetPath, typeof(T), loadedAction, arg);
        }

        public void Get(string assetPath, Type assetType, WuWuFrameworkAction<string, UnityObject, object> loadedAction, object arg = null)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                throw new Exception("资源路径无效");
            }

            if (m_LoadedAssets.TryGetValue(assetPath, out PoolObjectInfo info))
            {
                UnityObject obj = info.poolObject;
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
                m_ResourceMgr.LoadAsync(assetPath, assetType, OnLoaded, arg);
            }
            else
            {
                listLoadRequest.Add(request);
            }
        }

        public void Put(string assetPath, UnityObject obj)
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

        private void OnLoaded(string assetPath, UnityObject obj, object arg)
        {
            if (!m_LoadRequests.TryGetValue(assetPath, out List<LoadRequest> loadRequests))
            {
                throw new Exception(StringUtil.Append("[", assetPath, "] 资源加载完成 , 但加载回调不存在"));
            }

            if (!m_LoadedAssets.TryGetValue(assetPath, out PoolObjectInfo info))
            {
                info = PoolObjectInfo.Create(obj, -1, false, assetPath);
                m_LoadedAssets.Add(assetPath, info);
            }

            info.releaseTime = -1;

            foreach (var loadRequest in loadRequests)
            {
                if (loadRequest.loadedAction != null)
                {
                    info.referenceCount++;
                }

                loadRequest.Loaded(obj);
                loadRequest.Release();
            }

            m_LoadRequests.Remove(assetPath);
        }
    }
}