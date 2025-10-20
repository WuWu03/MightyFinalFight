using System;
using GameFrameWork.Event;
using UnityObject = UnityEngine.Object;

namespace GameFrameWork.Assets
{
    public interface IResourceMgr
    {
        public void InitAssetsMap();
        public T Load<T>(string assetPath) where T : UnityObject;
        public UnityObject Load(string assetPath, Type assetType);
        public void LoadAsync<T>(string assetPath, GameFrameWorkAction<string, UnityObject, object> loadedAction, object arg = null) where T : UnityObject;
        public void LoadAsync(string assetPath, Type assetType, GameFrameWorkAction<string, UnityObject, object> loadedAction, object arg = null);
        public void Unload(string assetPath, bool isThorough = false);
        public void UnloadAll(bool isThorough = false);
    }
}