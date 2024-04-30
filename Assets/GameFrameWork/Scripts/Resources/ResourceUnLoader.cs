using GameFrameWork.Resources;
using UnityEngine;

public class ResourceUnLoader : MonoBehaviour
{
    public string assetPath;
    public string spriteAtlas;
    public string spriteName;


    public void ResetAssetInfo()
    {
        assetPath = string.Empty;
        spriteAtlas = string.Empty;
        spriteName = string.Empty;
    }

    public void BeforeOnDestroy()
    {
        if (!string.IsNullOrEmpty(assetPath))
        {
            ResourcesMgr.instance.UnloadAsset(assetPath);
        }
        else if (!string.IsNullOrEmpty(spriteAtlas))
        {
            ResourcesMgr.instance.UnloadAsset(spriteAtlas);
        }

        ResetAssetInfo();
    }
}
