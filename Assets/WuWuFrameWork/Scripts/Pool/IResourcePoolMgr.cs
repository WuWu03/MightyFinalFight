using System;
using WuWuFramework.Resources;
using WuWuFramework.Event;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace WuWuFramework.Pool
{
    public interface IResourcePoolMgr
    {
        public void SetResourceMgr(IResourcesMgr resourceMgr,Transform poolRoot);
        public void CheckRelease();
        public void Cache<T>(string assetPath) where T : UnityObject;
        public void Cache(string assetPath, Type assetType);
        public void Get<T>(string assetPath, WuWuFrameworkAction<string, UnityObject, object> loadedAction, object arg = null) where T : UnityObject;
        public void Get(string assetPath, Type assetType, WuWuFrameworkAction<string, UnityObject, object> loadedAction, object arg = null);
        public void Put(string assetPath, UnityObject obj);
    }
}