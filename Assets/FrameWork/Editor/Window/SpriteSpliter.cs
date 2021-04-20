using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [Serializable]
    public class SpriteSpliter : EditorWindow
    {
        private int spriteSizeW = 256;
        private int spriteSizeH = 256;
        private string outPutExtName = ".png";
        private UnityEngine.Object _object;

        private string[] extNames = new string[] { ".png", ".jpg" };
        private string outPutPath = string.Empty;
        private string outPutPackTag = string.Empty;
        private bool outPutGenMipmaps = false;
        private TextureImporterType outPutType = TextureImporterType.Sprite;

        public SpriteSpliter()
        {
            titleContent = new GUIContent(this.GetType().Name);
        }

        private void OnGUI()
        {
            //输入框控件
            UnityEngine.Object _temp = _object;
            _object = EditorGUILayout.ObjectField(new GUIContent("拖入要切割的图片"), _object, typeof(Texture2D), true);

            if (_object == null)
            {
                spriteSizeW = 256;
                spriteSizeH = 256;
                outPutExtName = ".png";
                outPutPath = Application.dataPath + "/SplitSprite/";
                outPutType = TextureImporterType.Sprite;
                outPutPackTag = string.Empty;
                outPutGenMipmaps = false;
            }
            else if (_object != null && _temp != _object)
            {
                outPutPath = Application.dataPath + "/SplitSprite/" + _object.name + "/";
                _temp = _object;
            }
            spriteSizeW = EditorGUILayout.IntField("每张小图的宽度:", spriteSizeW);
            spriteSizeH = EditorGUILayout.IntField("每张小图的高度:", spriteSizeH);
            outPutExtName = EditorGUILayout.TextField("导出图片格式:", outPutExtName);
            outPutType = (TextureImporterType)EditorGUILayout.EnumPopup("导出图片类型:", outPutType);
            outPutPath = EditorGUILayout.TextField("导出图片路径:", outPutPath);
            outPutPackTag = EditorGUILayout.TextField("导出图片图集名称:", outPutPackTag);
            outPutGenMipmaps = EditorGUILayout.Toggle("导出图片生成小图:", outPutGenMipmaps);

            GUILayout.FlexibleSpace();

            if (EditorGUILayout.Foldout(true, "图片路径"))
            {
                string path = AssetDatabase.GetAssetPath(_object);
                Rect wr = new Rect(0, 0, path.Length, 260);
                SpriteSpliter window = (SpriteSpliter)EditorWindow.GetWindowWithRect(typeof(SpriteSpliter), wr, true, "SpriteSpliterSettings");
                EditorGUILayout.LabelField("", path);
            }


            if (GUILayout.Button("切割图片"))
            {
                if (_object == null)
                {
                    this.ShowNotification(new GUIContent("先拖入要切割的图片！"));
                    return;
                }
                bool isInExt = false;
                if (string.IsNullOrEmpty(outPutExtName))
                {
                    outPutExtName = ".png";
                    isInExt = true;
                }
                else
                {
                    foreach (string _ext in extNames)
                    {
                        if (outPutExtName.ToLower().Equals(_ext))
                        {
                            isInExt = true;
                            break;
                        }
                    }
                }
                if (!isInExt)
                {
                    string error = "图片格式只能是";

                    foreach (string _ext in extNames)
                    {
                        error += _ext;
                        error += " ";
                    }

                    error += "!";
                    this.ShowNotification(new GUIContent(error));
                    return;
                }

                Texture2D tempTex = _object as Texture2D;
                if (spriteSizeW > tempTex.width || spriteSizeH > tempTex.height)
                {
                    this.ShowNotification(new GUIContent("小图宽高不能比主图大，请重新输入正确的数值!"));
                    return;
                }
                this.SplitSprite();
                this.ShowNotification(new GUIContent("图片切割成功在" + outPutPath + "下查看"));
            }
        }

        private void SplitSprite()
        {
            string resourcesPath = "Assets";
            if (_object == null) return;

            string selectionPath = AssetDatabase.GetAssetPath(_object);

            // 必须最上级是"Assets/Resources/"
            if (selectionPath.StartsWith(resourcesPath))
            {
                string selectionExt = System.IO.Path.GetExtension(selectionPath);
                if (selectionExt.Length == 0)
                {
                    return;
                }
                Texture2D texture = _object as Texture2D;

                int row = texture.height / spriteSizeH;
                int column = texture.width / spriteSizeW;

                SpriteMetaData[] blocks = new SpriteMetaData[row * column];

                for (int j = 0; j < row; ++j)
                {
                    for (int k = 0; k < column; ++k)
                    {
                        int id = j * column + k;
                        SpriteMetaData tmp = new SpriteMetaData();
                        tmp.name = _object.name + "_" + k * spriteSizeW + "_" + j * spriteSizeH;
                        tmp.pivot = new Vector2(0.5f, 0.5f);
                        tmp.rect = new Rect(k * spriteSizeW, j * spriteSizeH, spriteSizeW, spriteSizeH);
                        blocks[id] = tmp;
                    }
                }

                TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_object)) as TextureImporter;
                TextureImporterSettings textureImporterSettings = new TextureImporterSettings();
                TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings();
                textureImporter.ReadTextureSettings(textureImporterSettings);
                textureImporterSettings.ApplyTextureType(TextureImporterType.Sprite);
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                textureImporterSettings.spriteMode = 2;
                textureImporter.spritesheet = blocks;
                textureImporter.isReadable = true;
                textureImporterSettings.readable = true;
                textureImporterPlatformSettings.format = TextureImporterFormat.RGB16;
                textureImporter.fadeout = !textureImporter.fadeout;
                textureImporter.SetTextureSettings(textureImporterSettings);
                textureImporter.SetPlatformTextureSettings(textureImporterPlatformSettings);
                textureImporter.fadeout = false;
                textureImporter.SaveAndReimport();
                string loadPath = selectionPath.Remove(selectionPath.Length - selectionExt.Length);
                texture.Apply();
                AssetDatabase.ImportAsset(loadPath);

                // 加载此文件下的所有资源
                UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath(loadPath + selectionExt);
                List<Sprite> sprites = new List<Sprite>();

                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] is Sprite)
                    {
                        textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(objects[i])) as TextureImporter;
                        textureImporterSettings = new TextureImporterSettings();
                        textureImporter.ReadTextureSettings(textureImporterSettings);
                        textureImporter.isReadable = true;
                        textureImporterSettings.readable = true;
                        textureImporter.SetTextureSettings(textureImporterSettings);
                        sprites.Add(objects[i] as Sprite);
                    }
                }

                if (sprites.Count > 0)
                {
                    string _outPutPath = outPutPath;
                    string realOutPutPath = string.Empty;
                    System.IO.Directory.CreateDirectory(_outPutPath);

                    foreach (Sprite sprite in sprites)
                    {
                        // 创建单独的纹理
                        Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.format, false);
                        tex.SetPixels(sprite.texture.GetPixels((int)sprite.rect.xMin, (int)sprite.rect.yMin,
                            (int)sprite.rect.width, (int)sprite.rect.height));
                        tex.Apply();

                        // 写入成各种格式文件
                        byte[] bytes = null;

                        if (outPutExtName.Equals(".png")) bytes = tex.EncodeToPNG();
                        else if (outPutExtName.Equals(".jpb")) bytes = tex.EncodeToJPG();

                        realOutPutPath = _outPutPath + sprite.name + outPutExtName;
                        System.IO.File.WriteAllBytes(realOutPutPath, bytes);
                    }

                    AssetDatabase.Refresh();

                    foreach (Sprite sprite in sprites)
                    {
                        realOutPutPath = _outPutPath + sprite.name + outPutExtName;
                        realOutPutPath = realOutPutPath.Substring(realOutPutPath.IndexOf("Assets"));
                        Texture2D _tex = AssetDatabase.LoadAssetAtPath<Texture2D>(realOutPutPath);
                        textureImporter = AssetImporter.GetAtPath(realOutPutPath) as TextureImporter;
                        textureImporterSettings = new TextureImporterSettings();
                        textureImporter.ReadTextureSettings(textureImporterSettings);
                        textureImporterSettings.readable = true;
                        textureImporter.isReadable = true;
                        textureImporterSettings.spriteMode = (int)SpriteImportMode.Single;
                        textureImporterSettings.mipmapEnabled = outPutGenMipmaps;
                        textureImporter.textureType = outPutType;
                        textureImporter.spriteImportMode = SpriteImportMode.Single;
                        textureImporter.fadeout = false;
                        textureImporter.mipmapEnabled = outPutGenMipmaps;
                        textureImporter.spritePackingTag = outPutPackTag;
                        textureImporter.SetTextureSettings(textureImporterSettings);
                        textureImporter.SaveAndReimport();
                        _tex.Apply();
                        textureImporter.SaveAndReimport();
                    }
                    Debug.Log("SaveSprite to " + outPutPath);
                }
            }
            Debug.Log("SaveSprite Finished");
        }

        private void OnDestroy()
        {
            _object = null;
        }
    }
}