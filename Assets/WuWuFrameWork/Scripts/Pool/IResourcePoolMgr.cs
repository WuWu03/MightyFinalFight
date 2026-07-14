using System;
using WuWuFramework.Event;
using WuWuFramework.Resources;
using UnityObject = UnityEngine.Object;

namespace WuWuFramework.Pool
{
    public interface IResourcePoolMgr
    {
        public void SetResourcesMgr(IResourcesMgr resourceMgr);
        public void CheckRelease();
        public void Cache<T>(string assetPath) where T : UnityObject;
        public void Cache(string assetPath, Type assetType);
        public void Get<T>(string assetPath, WuWuFrameworkAction<string, UnityObject, object> loadedAction, object arg = null) where T : UnityObject;
        public void Get(string assetPath, Type assetType, WuWuFrameworkAction<string, UnityObject, object> loadedAction, object arg = null);
        public void Put(string assetPath, UnityObject obj);
    }
}