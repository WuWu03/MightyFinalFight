using GameFrameWork;
using GameFrameWork.Utilities;
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
        textureImporter.spriteImportMode = SpriteImportMode.Single;
        textureImporter.textureCompression = TextureImporterCompression.Compressed;

        textureImporter.SetPlatformTextureSettings(new TextureImporterPlatformSettings() 
        {
            name = "Standalone",
            compressionQuality = 50,
            crunchedCompression = false,
            format = TextureImporterFormat.DXT5,
            textureCompression = TextureImporterCompression.Compressed,
            maxTextureSize = 2048,
            allowsAlphaSplitting = false,
            overridden = true,
            
        });

        textureImporter.SetTextureSettings(new TextureImporterSettings()
        {
            alphaIsTransparency = true,
            alphaSource = TextureImporterAlphaSource.FromInput,
            borderMipmap = false,
            filterMode = FilterMode.Bilinear,
            mipmapEnabled = false,
            readable = false,
            spriteGenerateFallbackPhysicsShape = false,
            spriteMeshType = SpriteMeshType.Tight,
            spriteMode = 1,//None = 0、Single = 1、Manual = 2。
            spritePivot = Vector2.one * 0.5f,
            spritePixelsPerUnit = 100f,
            sRGBTexture = true,
            textureShape = TextureImporterShape.Texture2D,
            textureType = TextureImporterType.Sprite,
            wrapMode = TextureWrapMode.Clamp,
            
        });

        textureImporter.SaveAndReimport();
    }

	private bool IsUITexture(string assetPath)
	{
        string uiPath = PathUtil.FormatPath(PathUtil.GetAssetPath(AppConfig.instance.uiDirectory), "UISprites");
        return assetPath.Contains(uiPath);
	}
}
