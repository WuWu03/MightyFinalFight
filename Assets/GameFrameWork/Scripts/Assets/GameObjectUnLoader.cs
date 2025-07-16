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

        public void Release()
        {
            if (!string.IsNullOrEmpty(gameObjectPath))
            {
                GameObjectPoolMgr.instance.Put(gameObjectPath, gameObject);
            }

            ResetAssetInfo();
        }
    }
}