using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class FontMaker
    {
        public static void CreateMyFontSprite()
        {
            if (Selection.objects == null || Selection.objects.Length < 1)
            {
                Debug.LogWarning("没有选中Sprite文件，需要将Sprite Mode设置成Multiple，切分好，并且以以名字的最后一个字符当做ascii码");
                return;
            }

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                GenerateFont(Selection.objects[i]);
            }
        }

        private static void GenerateFont(UnityEngine.Object o)
        {
            if (o.GetType() != typeof(Texture2D))
            {
                Debug.LogWarning("选中的并不是图片文件");
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(o);
            string assetExt = Path.GetExtension(assetPath);

            if (string.IsNullOrEmpty(assetExt))
            {
                Debug.LogWarning("选中的并不是图片文件");
                return;
            }

            string loadPath = assetPath.Remove(assetPath.Length - assetExt.Length);
            string fontPath = loadPath + ".fontsettings";
            string matPath = loadPath + ".mat";
            float lineSpace = 0.1f;//字体行间距，下面会根据最高的字体得到行间距，如果是固定高度，可以在这里自行调整  

            UnityEngine.Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            if (sprites == null || sprites.Length < 1)
            {
                Debug.LogWarning("Sprite文件没有切分，需要设置[Texture Type] = [Sprite],[Sprite Mode] = [Multiple]");
            }

            Texture2D tex = o as Texture2D;
            List<CharacterInfo> characterInfo = new List<CharacterInfo>();

            //得到最高的高度，设置行高和进行偏移计算  
            for (int i = 1; i < sprites.Length; i++)
            {
                Sprite sprite = sprites[i] as Sprite;

                if (sprite == null)
                {
                    continue;
                }

                Rect rect = sprite.rect;

                if (sprite.rect.height > lineSpace)
                {
                    lineSpace = sprite.rect.height;
                }

                //根据pivot设置字符的偏移，具体需要做成什么样的，可以根据自己需要修改公式
                float pivot = sprite.pivot.y / rect.height - 0.5f;
                if (pivot > 0)
                {
                    pivot = -lineSpace / 2 - sprite.pivot.y;
                }
                else if (pivot < 0)
                {
                    pivot = -lineSpace / 2 + rect.height - sprite.pivot.y;
                }
                else
                {
                    pivot = -lineSpace / 2;
                }
                
                int offsetY = (int)(pivot + (lineSpace - rect.height) / 2);
                CharacterInfo info = new();
                info.index = (int)sprite.name[^1];//设置ascii码，使用切分sprite的最后一个字母
                info.uvBottomLeft = new Vector2((float)rect.x / tex.width, (float)(rect.y / tex.height));
                info.uvBottomRight = new Vector2((float)(rect.x + rect.width) / tex.width, (float)(rect.y) / tex.height);
                info.uvTopLeft = new Vector2((float)rect.x / tex.width, (float)(rect.y + rect.height) / tex.height);
                info.uvTopRight = new Vector2((float)(rect.x + rect.width) / tex.width, (float)(rect.y + rect.height) / tex.height);
                info.minX = 0;//设置字符顶点的偏移位置和宽高  
                info.minY = -(int)rect.height - offsetY;
                info.maxX = (int)rect.width;
                info.maxY = -offsetY;
                info.advance = (int)rect.width;//设置字符的宽度 
                info.glyphHeight = (int)rect.height;
                characterInfo.Add(info);
            }

            Material mat = new Material(Shader.Find("GUI/Text Shader"));
            mat.SetTexture("_MainTex", tex);

            Font font = new Font();
            font.material = mat;
            font.characterInfo = characterInfo.ToArray();

            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.CreateAsset(font, fontPath);
            EditorUtility.SetDirty(font);//设置变更过的资源
            AssetDatabase.SaveAssets();//保存变更的资源
            AssetDatabase.Refresh();

            //由于上面fresh之后在编辑器中依然没有刷新，所以暂时想到这个方法
            //先把生成的字体导出成一个包，然后再重新导入进来，这样就可以直接刷新了
            AssetDatabase.ExportPackage(fontPath, "temp.unitypackage");
            AssetDatabase.DeleteAsset(fontPath);
            AssetDatabase.ImportPackage("temp.unitypackage", true);
            AssetDatabase.Refresh();

            //最佳高度：上下各留一个像素的间距，如果不需要可以注释掉，根据需求更改  
            //打印是为了使使用者方便填写行高，因为font不支持设置行高。  
            Debug.Log("创建字体成功, 最大高度：" + lineSpace + ", 最佳高度：" + (lineSpace + 2));
        }
    }
}