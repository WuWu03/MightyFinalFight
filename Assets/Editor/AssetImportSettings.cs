using GameFrameWork;
using GameFrameWork.Utils;
using UnityEditor;
using UnityEngine;
public class AssetImportSettings : AssetPostprocessor
{
	public void OnPostprocessTexture(Texture2D texture)
	{
	
	}

	public void OnPreprocessTexture()
    {
        TextureImporter textureImporter = assetImporter as TextureImporter;

        if (IsUITexture(assetPath))
        {
            SetUITexture(textureImporter);
        }
    }

	private void SetUITexture(TextureImporter textureImporter)
	{
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Single;
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        textureImporter.isReadable = false;
        textureImporter.mipmapEnabled = false;
        textureImporter.compressionQuality = 0;
        textureImporter.wrapMode = TextureWrapMode.Clamp;
        textureImporter.filterMode = FilterMode.Bilinear;
        textureImporter.maxTextureSize = 2048;

        textureImporter.SetPlatformTextureSettings(new TextureImporterPlatformSettings()
        {
           name = "Standalone",
           overridden = false,
        });

        textureImporter.SaveAndReimport();
    }

	private bool IsUITexture(string assetPath)
	{
        string uiAtlasPath = GameFrameWork.Editor.EditorMgr.GetGameFrameWorkConfig().uiAtlasPath;
        return assetPath.Contains(uiAtlasPath);
	}
}
