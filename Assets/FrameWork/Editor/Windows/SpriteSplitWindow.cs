using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class SpriteSplitWindow : EditorWindow
    {
        public SpriteSplitWindow()
        {
            titleContent = new GUIContent(this.GetType().Name);
        }

        private void OnGUI()
        {
            //输入框控件
            UnityEngine.Object _temp = m_SelectObject;
            m_SelectObject = EditorGUILayout.ObjectField(new GUIContent("拖入要切割的图片"), m_SelectObject, typeof(Texture2D), true);

            if (m_SelectObject == null)
            {
                m_SpriteWidth = 256;
                m_SpriteHeight = 256;
                m_OutPutExtName = ".png";
                m_OutPutPath = Application.dataPath + "/SplitSprite/";
                m_OutPutType = TextureImporterType.Sprite;
                m_OutPutPackTag = string.Empty;
                m_OutPutGenMipmaps = false;
            }
            else if (m_SelectObject != null && _temp != m_SelectObject)
            {
                m_OutPutPath = Application.dataPath + "/SplitSprite/" + m_SelectObject.name + "/";
                _temp = m_SelectObject;

                TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(m_SelectObject)) as TextureImporter;
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.isReadable = true;
                textureImporter.fadeout = false;
                textureImporter.SaveAndReimport();
                (m_SelectObject as Texture2D).Apply();
                AssetDatabase.Refresh();
            }

            m_SpriteWidth = EditorGUILayout.IntField("每张小图的宽度:", m_SpriteWidth);
            m_SpriteHeight = EditorGUILayout.IntField("每张小图的高度:", m_SpriteHeight);
            m_OutPutExtName = EditorGUILayout.TextField("导出图片格式:", m_OutPutExtName);
            m_OutPutType = (TextureImporterType)EditorGUILayout.EnumPopup("导出图片类型:", m_OutPutType);
            m_OutPutPath = EditorGUILayout.TextField("导出图片路径:", m_OutPutPath);
            m_OutPutPackTag = EditorGUILayout.TextField("导出图片图集名称:", m_OutPutPackTag);
            m_OutPutGenMipmaps = EditorGUILayout.Toggle("导出图片生成小图:", m_OutPutGenMipmaps);

            m_FoldOut = EditorGUILayout.Foldout(m_FoldOut, "图片路径");
            if (m_FoldOut)
            {
                string path = AssetDatabase.GetAssetPath(m_SelectObject);
                EditorGUILayout.LabelField("", path);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("切割图片"))
            {
                if (m_SelectObject == null)
                {
                    ShowNotification(new GUIContent("先拖入要切割的图片！"));
                    return;
                }

                bool isInExt = false;

                if (string.IsNullOrEmpty(m_OutPutExtName))
                {
                    m_OutPutExtName = ".png";
                    isInExt = true;
                }
                else
                {
                    foreach (string _ext in m_ExtNames)
                    {
                        if (m_OutPutExtName.ToLower().Equals(_ext))
                        {
                            isInExt = true;
                            break;
                        }
                    }
                }

                if (!isInExt)
                {
                    string error = "图片格式只能是";

                    foreach (string ext in m_ExtNames)
                    {
                        error += ext;
                        error += " ";
                    }

                    error += "!";
                    ShowNotification(new GUIContent(error));
                    return;
                }

                Texture2D tempTex = m_SelectObject as Texture2D;

                if (m_SpriteWidth > tempTex.width || m_SpriteHeight > tempTex.height)
                {
                    ShowNotification(new GUIContent("小图宽高不能比主图大，请重新输入正确的数值!"));
                    return;
                }


                SplitSprite();
                ShowNotification(new GUIContent("图片切割成功在" + m_OutPutPath + "下查看"));
            }
        }

        private void SplitSprite()
        {
            if (m_SelectObject == null) return;

            Texture2D texture = m_SelectObject as Texture2D;

            string selectionPath = AssetDatabase.GetAssetPath(m_SelectObject);
            string selectionExt = System.IO.Path.GetExtension(selectionPath);
            string loadPath = selectionPath.Remove(selectionPath.Length - selectionExt.Length);

            int row = Mathf.CeilToInt((float)texture.height / m_SpriteHeight);
            int column = Mathf.CeilToInt((float)texture.width / m_SpriteWidth);

            SpriteMetaData[] blocks = new SpriteMetaData[row * column];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    SpriteMetaData tmp = new SpriteMetaData();
                    int id = i * column + j;
                    float x = j * m_SpriteWidth;
                    float y = i * m_SpriteHeight;
                    
                    float width = (x + m_SpriteWidth) <= texture.width ? m_SpriteWidth : texture.width - x;
                    float height = (y + m_SpriteHeight) <= texture.height ? m_SpriteHeight : texture.height - y;

                    tmp.name = m_SelectObject.name + "_" + i * m_SpriteWidth + "_" + j * m_SpriteHeight;
                    tmp.pivot = new Vector2(0.5f, 0.5f);
                    tmp.rect = new Rect(x, y, width, height);
                    blocks[id] = tmp;
                }
            }

            TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(m_SelectObject)) as TextureImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.isReadable = true;
            textureImporter.fadeout = false;
            textureImporter.spritesheet = blocks;
            textureImporter.SaveAndReimport();
            AssetDatabase.ImportAsset(loadPath);
            AssetDatabase.Refresh();

            // 加载此文件下的所有资源
            UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath(loadPath + selectionExt);
            List<Sprite> sprites = new List<Sprite>();

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] is Sprite)
                {
                    textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(objects[i])) as TextureImporter;
                    textureImporter.isReadable = true;
                    sprites.Add(objects[i] as Sprite);
                }
            }

            if (sprites.Count > 0)
            {
                string outPutPath = m_OutPutPath;
                string realOutPutPath = string.Empty;
                System.IO.Directory.CreateDirectory(outPutPath);

                foreach (Sprite sprite in sprites)
                {
                    // 创建单独的纹理
                    Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.format, false);
                    tex.SetPixels(sprite.texture.GetPixels((int)sprite.rect.xMin, (int)sprite.rect.yMin, (int)sprite.rect.width, (int)sprite.rect.height));
                    tex.Apply();

                    // 写入成各种格式文件
                    byte[] bytes = null;

                    if (m_OutPutExtName.Equals(".png")) bytes = tex.EncodeToPNG();
                    else if (m_OutPutExtName.Equals(".jpb")) bytes = tex.EncodeToJPG();

                    realOutPutPath = outPutPath + sprite.name + m_OutPutExtName;
                    System.IO.File.WriteAllBytes(realOutPutPath, bytes);
                }

                AssetDatabase.Refresh();

                foreach (Sprite sprite in sprites)
                {
                    realOutPutPath = outPutPath + sprite.name + m_OutPutExtName;
                    realOutPutPath = realOutPutPath.Substring(realOutPutPath.IndexOf("Assets"));
                    textureImporter = AssetImporter.GetAtPath(realOutPutPath) as TextureImporter;
                    textureImporter.isReadable = true;
                    textureImporter.textureType = m_OutPutType;
                    textureImporter.spriteImportMode = SpriteImportMode.Single;
                    textureImporter.fadeout = false;
                    textureImporter.mipmapEnabled = m_OutPutGenMipmaps;
                    textureImporter.spritePackingTag = m_OutPutPackTag;
                    textureImporter.SaveAndReimport();
                }
            }
        }

        private void OnDestroy()
        {
            m_SelectObject = null;
        }

        private int m_SpriteWidth = 256;
        private int m_SpriteHeight = 256;
        private string m_OutPutExtName = ".png";
        private UnityEngine.Object m_SelectObject = null;

        private string[] m_ExtNames = new string[] { ".png", ".jpg" };
        private string m_OutPutPath = string.Empty;
        private string m_OutPutPackTag = string.Empty;
        private bool m_OutPutGenMipmaps = false;
        private bool m_FoldOut = false;
        private TextureImporterType m_OutPutType = TextureImporterType.Sprite;
    }
}