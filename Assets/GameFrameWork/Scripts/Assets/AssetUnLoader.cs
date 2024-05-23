using GameFrameWork.Pool;
using UnityEngine;

namespace GameFrameWork.Assets
{
    public class AssetUnLoader : MonoBehaviour
    {
        public string gameObjectPath;
        public string spritePath;
        public GameObject go;
        public UnityEngine.Object sprite;

        public void ResetAssetInfo()
        {
            gameObjectPath = string.Empty;
            spritePath = string.Empty;
            go = null;
            sprite = null;
        }

        public void BeforeOnDestroy()
        {
            if (!string.IsNullOrEmpty(gameObjectPath))
            {
                GameObjectPool.instance.Put(gameObjectPath, go);
            }
            else if (!string.IsNullOrEmpty(spritePath))
            {
                AssetsPool.instance.Put(spritePath, sprite);
            }

            ResetAssetInfo();
        }
    }
}