using GameFrameWork.Pool;
using UnityEngine;

namespace GameFrameWork.Assets
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