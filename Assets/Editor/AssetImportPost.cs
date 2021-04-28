using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class AssetImportPost : AssetPostprocessor
{
	public void OnPostprocessTexture(Texture2D texture)
	{
	
	}

	public void OnPreprocessTexture()
    {
		if (IsSprite())
		{
			TextureImporter textureImporter = assetImporter as TextureImporter;
			textureImporter.spriteImportMode = SpriteImportMode.Single;
			textureImporter.textureType = TextureImporterType.Sprite;
			textureImporter.isReadable = false;
			textureImporter.maxTextureSize = 2048;
			textureImporter.textureCompression = TextureImporterCompression.Compressed;
			textureImporter.compressionQuality = 2;
			textureImporter.wrapMode = TextureWrapMode.Clamp;
			textureImporter.filterMode = FilterMode.Point;
			textureImporter.sRGBTexture = true;
			textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
			textureImporter.alphaIsTransparency = true;
			textureImporter.mipmapEnabled = false;
		}
	}

	private bool IsSprite()
    {
		for (int i = 0; i < m_SpritePath.Length; i++)
		{
			if (m_SpritePath[i].Contains(m_SpritePath[i]))
			{
				return true;
			}
		}

		return false;
	}

	private string[] m_SpritePath = new string[]
	{
		"ArtResources/Texture/Stage",
		"ArtResources/Models/Character",
		"ArtResources/Fx",
	};
}
