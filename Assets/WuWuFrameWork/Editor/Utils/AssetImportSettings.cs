using System.IO;
using UnityEditor;
using UnityEngine;

namespace WuWuFramework.Editor
{
    public class AssetImportSettings : AssetPostprocessor
    {
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

            TextureImporterSettings dest = new();
            textureImporter.ReadTextureSettings(dest);
            dest.spriteGenerateFallbackPhysicsShape = false;
            textureImporter.SetTextureSettings(dest);
            textureImporter.SaveAndReimport();
        }

        private bool IsUITexture(string assetPath)
        {
            string uiAtlasPath = WuWuFramework.Editor.EditorMgr.GetWuWuFrameworkConfig().uiAtlasPath;
            string path = Path.GetDirectoryName(assetPath).Replace(@"\", "/");
            return path.Contains(uiAtlasPath);
        }
    }
}