using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class AssetImportPost : AssetPostprocessor
{
	public void OnPostprocessTexture(Texture2D texture)
	{
		if (assetPath.IndexOf("Texture/Round") > -1)
		{
			TextureImporter ti = assetImporter as TextureImporter;
			ti.textureType = TextureImporterType.Sprite;
			ti.maxTextureSize = 1024;
			ti.textureCompression = TextureImporterCompression.Compressed;
			ti.spriteImportMode = SpriteImportMode.Single;
			AssetDatabase.Refresh();
		}
	}
}
