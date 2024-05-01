using GameFrameWork.Pool;
using UnityEngine;

public class ResourceUnLoader : MonoBehaviour
{
    public string gameObjectPath;
    public string spriteAtlasPath;
    public string spriteName;
    public GameObject go;
    public UnityEngine.Object spriteAtlas;

    public void ResetAssetInfo()
    {
        gameObjectPath = string.Empty;
        spriteAtlasPath = string.Empty;
        spriteName = string.Empty;
        go = null;
        spriteAtlas = null;
    }

    public void BeforeOnDestroy()
    {
        if (!string.IsNullOrEmpty(gameObjectPath))
        {
            GameObjectPool.instance.Put(gameObjectPath, go);
        }
        else if (!string.IsNullOrEmpty(spriteAtlasPath))
        {
            ResourcesPool.instance.Put(spriteAtlasPath, spriteAtlas);
        }

        ResetAssetInfo();
    }
}
