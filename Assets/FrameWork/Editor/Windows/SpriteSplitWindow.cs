using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class SpriteSplitWindow : EditorWindow
    {
        enum SplitType
        {
            [EnumLabel("图片")]
            Texture2D,
            [EnumLabel("路径")]
            Folder,
        }

        public SpriteSplitWindow()
        {
            titleContent = new GUIContent(GetType().Name);
        }

        private void OnGUI()
        {
            MainGUI();
        }

        private void MainGUI()
        {
            SplitType type = m_SplitType;
            m_SplitType = (SplitType)EditorUtility.EnumPopup("切图模式",m_SplitType);

            if(type != m_SplitType)
            {
                m_SelectObject = null;
                m_SelectFolder = string.Empty;
            }

            switch (m_SplitType)
            {
                case SplitType.Texture2D:
                    FileGUI();
                    break;
                case SplitType.Folder:
                    FolderGUI();
                    break;
            }

            m_SpriteWidth = EditorGUILayout.IntField("每张小图的宽度:", m_SpriteWidth);
            m_SpriteHeight = EditorGUILayout.IntField("每张小图的高度:", m_SpriteHeight);
            m_OutPutExtName = EditorGUILayout.TextField("导出图片格式:", m_OutPutExtName);
            m_OutPutType = (TextureImporterType)EditorGUILayout.EnumPopup("导出图片类型:", m_OutPutType);
            m_OutPutPath = EditorGUILayout.TextField("导出图片路径:", m_OutPutPath);
            m_OutPutPackTag = EditorGUILayout.TextField("导出图片图集名称:", m_OutPutPackTag);
            m_OutPutGenMipmaps = EditorGUILayout.Toggle("导出图片生成小图:", m_OutPutGenMipmaps);

            if(m_SelectObject != null)
            {
                EditorGUILayout.LabelField("路径:", AssetDatabase.GetAssetPath(m_SelectObject));
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("切割图片"))
            {
                switch (m_SplitType)
                {
                    case SplitType.Texture2D:
                        FileSplit();
                        break;
                    case SplitType.Folder:
                        FolderSplit();
                        break;
                }
            }
        }

        private void FileGUI()
        {
            UnityEngine.Object temp = m_SelectObject;
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 5));
            m_SelectObject = EditorGUI.ObjectField(rect, "拖入图片", m_SelectObject, typeof(UnityEngine.Texture2D), false);

            if (m_SelectObject == null)
            {
                DefaultSetting();
            }
            else if (m_SelectObject != null && temp != m_SelectObject)
            {
                temp = m_SelectObject;
                SetTextureSprite(m_SelectObject as Texture2D);
                AssetDatabase.Refresh();
            }
        }

        private void FolderGUI()
        {
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 5));
            UnityEngine.Object temp = m_SelectObject;
            m_SelectObject = EditorGUI.ObjectField(rect, "拖入文件夹", m_SelectObject, typeof(UnityEditor.DefaultAsset), false);

            if (m_SelectObject == null)
            {
                DefaultSetting();
            }
            else if (m_SelectObject != null && temp != m_SelectObject)
            {
                m_SelectFolder = AssetDatabase.GetAssetPath(m_SelectObject);
                m_OutPutPath = Application.dataPath + "/SplitSprite" + m_SelectFolder.Substring(m_SelectFolder.LastIndexOf("/")) + "/";
                string path = Application.dataPath + m_SelectFolder.Substring(m_SelectFolder.IndexOf("Assets") + 6);
                string[] files = Directory.GetFiles(path, "*");

                for (int i = 0; i < files.Length; i++)
                {
                    if (Path.GetExtension(files[i]).Equals(".meta")) continue;
                    string objectPath = files[i].Substring(files[i].IndexOf("Assets"));
                    UnityEngine.Object @object = AssetDatabase.LoadAssetAtPath(objectPath, typeof(Texture2D));

                    if(@object != null)
                    {
                        SetTextureSprite(@object as Texture2D);
                    }
                }

                AssetDatabase.Refresh();
            }
        }

        private void DefaultSetting()
        {
            m_SpriteWidth = 256;
            m_SpriteHeight = 256;
            m_OutPutExtName = ".png";
            m_OutPutPath = Application.dataPath + "/SplitSprite/";
            m_OutPutType = TextureImporterType.Sprite;
            m_OutPutPackTag = string.Empty;
            m_OutPutGenMipmaps = false;
        }

        private void FileSplit()
        {
            if (m_SelectObject == null)
            {
                ShowNotification(new GUIContent("先拖入要切割的图片！"));
                return;
            }


            if (CanSplit(m_SelectObject as Texture2D))
            {
                SplitSprite(m_SelectObject as Texture2D);
                ShowNotification(new GUIContent("图片切割成功在" + m_OutPutPath + "下查看"));
            }
        }

        private void FolderSplit()
        {
            if (m_SelectObject == null)
            {
                ShowNotification(new GUIContent("先拖入文件夹！"));
                return;
            }

            string path = Application.dataPath + m_SelectFolder.Substring(m_SelectFolder.IndexOf("Assets") + 6);
            string[] files = Directory.GetFiles(path, "*");

            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetExtension(files[i]).Equals(".meta")) continue;
                string objectPath = files[i].Substring(files[i].IndexOf("Assets"));
                UnityEngine.Object @object = AssetDatabase.LoadAssetAtPath(objectPath, typeof(Texture2D));
                if (@object != null && CanSplit(@object as Texture2D))
                    SplitSprite(@object as Texture2D);
            }
        }

        private bool CanSplit(Texture2D texture)
        {
            bool isInExt = false;

            if (string.IsNullOrEmpty(m_OutPutExtName))
            {
                m_OutPutExtName = ".png";
                isInExt = true;
            }
            else
            {
                foreach (string ext in m_ExtNames)
                {
                    if (m_OutPutExtName.ToLower().Equals(ext))
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
                return false;
            }

            if (m_SpriteWidth > texture.width || m_SpriteHeight > texture.height)
            {
                ShowNotification(new GUIContent("小图宽高不能比主图大，请重新输入正确的数值!"));
                return false;
            }

            return true;
        }

        private void SetTextureSprite(Texture2D texture)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.isReadable = true;
            textureImporter.fadeout = false;
            textureImporter.SaveAndReimport();
            texture.Apply();
        }

        private void SplitSprite(Texture2D texture)
        {
            if (texture == null) return;

            string selectionPath = AssetDatabase.GetAssetPath(texture);
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

            TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
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
                string realOutPutPath = m_OutPutPath + texture.name + "/";

                if (Directory.Exists(realOutPutPath))
                {
                    Directory.Delete(realOutPutPath, true);
                }

                Directory.CreateDirectory(realOutPutPath);

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

                    File.WriteAllBytes(realOutPutPath + sprite.name + m_OutPutExtName, bytes);
                }

                AssetDatabase.Refresh();

                realOutPutPath = realOutPutPath.Substring(realOutPutPath.IndexOf("Assets"));

                foreach (Sprite sprite in sprites)
                {
                    textureImporter = AssetImporter.GetAtPath(realOutPutPath + sprite.name + m_OutPutExtName) as TextureImporter;
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
            m_SelectFolder = string.Empty;
        }

        private SplitType m_SplitType = SplitType.Texture2D;
        private int m_SpriteWidth = 256;
        private int m_SpriteHeight = 256;
        private string m_OutPutExtName = ".png";
        private UnityEngine.Object m_SelectObject = null;
        private string m_SelectFolder = string.Empty;
        private string[] m_ExtNames = new string[] { ".png", ".jpg" };
        private string m_OutPutPath = string.Empty;
        private string m_OutPutPackTag = string.Empty;
        private bool m_OutPutGenMipmaps = false;
        private bool m_FoldOut = false;
        private TextureImporterType m_OutPutType = TextureImporterType.Sprite;
    }
}