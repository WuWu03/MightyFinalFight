using GameFrameWork.Event;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace GameFrameWork.Pool
{
    public interface IGameObjectPoolMgr
    {
        public void SetResourcePoolMgr(IResourcePoolMgr resourcePoolMgr, Transform poolRoot);
        public void AddPool(string tag, GameObject obj, int count = 1);
        public void RemovePool(string tag);
        public bool HasPool(string tag);
        public GameObject Get(string tag, Transform parent, string layer, bool isActive = true);
        public void GetFromAsset(string assetPath, GameFrameWorkAction<string, UnityObject, object> loadedAction, object arg = null);
        public void Put(string tag, GameObject go, bool isReleaseImmediately = false);
        public void CheckRelease();
    }
}