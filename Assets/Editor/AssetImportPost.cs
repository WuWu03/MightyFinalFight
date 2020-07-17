using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class AssetImportPost : AssetPostprocessor
{
	public void OnPostprocessTexture(Texture2D texture)
	{
		bool isSprite = false;
		for (int i = 0; i < m_SpritePath.Length; i++)
		{
			if (m_SpritePath[i].Contains(m_SpritePath[i]))
			{
				isSprite = true;
				break;
			}
		}

		if (isSprite)
		{
			TextureImporter ti = assetImporter as TextureImporter;
			ti.textureType = TextureImporterType.Sprite;
			ti.maxTextureSize = 2048;
			ti.textureCompression = TextureImporterCompression.Compressed;
			ti.spriteImportMode = SpriteImportMode.Single;
			ti.compressionQuality = 2;
			ti.wrapMode = TextureWrapMode.Clamp;
			ti.filterMode = FilterMode.Point;
			ti.sRGBTexture = true;
			ti.alphaSource = TextureImporterAlphaSource.FromInput;
			ti.alphaIsTransparency = true;
			ti.isReadable = false;
			ti.mipmapEnabled = false;
			ti.SaveAndReimport();
			AssetDatabase.Refresh();
		}
	}

	private string[] m_SpritePath = new string[]
	{
		"ArtResources/Texture/Stage",
		"ArtResources/Models/Character",
		"ArtResources/Fx",
	};
}
