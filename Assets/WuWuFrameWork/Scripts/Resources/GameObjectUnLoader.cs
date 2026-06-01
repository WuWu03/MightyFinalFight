using WuWuFramework.Pool;
using UnityEngine;

namespace WuWuFramework.Resources
{
    public class GameObjectUnLoader : MonoBehaviour
    {
        public string gameObjectPath;

        public void ResetAssetInfo()
        {
            gameObjectPath = string.Empty;
        }

        public void Release(IGameObjectPoolMgr gameObjectPoolMgr)
        {
            if (!string.IsNullOrEmpty(gameObjectPath))
            {
                gameObjectPoolMgr.Put(gameObjectPath, gameObject);
            }

            ResetAssetInfo();
        }
    }
}