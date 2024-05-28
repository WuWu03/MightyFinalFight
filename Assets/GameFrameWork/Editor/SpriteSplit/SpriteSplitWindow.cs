using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEditorInternal;
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
            EditorGUILayout.BeginVertical();
            m_SplitType = (SplitType)EditorUtil.EnumPopup("切图模式",m_SplitType);

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

            m_IsUseAuto = EditorGUILayout.Toggle("使用自动切图尺寸", m_IsUseAuto);

            if (!m_IsUseAuto)
            {
                m_SpriteWidth = EditorGUILayout.IntField("每张小图的宽度:", m_SpriteWidth);
                m_SpriteHeight = EditorGUILayout.IntField("每张小图的高度:", m_SpriteHeight);
            }
            m_OutPutExtName = EditorGUILayout.TextField("导出图片格式:", m_OutPutExtName);
            m_OutPutType = (TextureImporterType)EditorGUILayout.EnumPopup("导出图片类型:", m_OutPutType);
            m_OutPutPath = EditorGUILayout.TextField("导出图片路径:", m_OutPutPath);
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
            EditorGUILayout.EndVertical();
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
                {
                    SplitSprite(@object as Texture2D);
                }
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

            if (!m_IsUseAuto && (m_SpriteWidth > texture.width || m_SpriteHeight > texture.height))
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
            SpriteRect[] spriteRects = null;

            if (!m_IsUseAuto)
            {
                int row = Mathf.CeilToInt((float)texture.height / m_SpriteHeight);
                int column = Mathf.CeilToInt((float)texture.width / m_SpriteWidth);
                spriteRects = new SpriteRect[row * column];

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        SpriteRect tmp = new SpriteRect();
                        int id = i * column + j;
                        float x = j * m_SpriteWidth;
                        float y = i * m_SpriteHeight;

                        float width = (x + m_SpriteWidth) <= texture.width ? m_SpriteWidth : texture.width - x;
                        float height = (y + m_SpriteHeight) <= texture.height ? m_SpriteHeight : texture.height - y;

                        tmp.name = m_SelectObject.name + "_" + i * m_SpriteWidth + "_" + j * m_SpriteHeight;
                        tmp.pivot = new Vector2(0.5f, 0.5f);
                        tmp.rect = new Rect(x, y, width, height);
                        spriteRects[id] = tmp;
                    }
                }
            }
            else
            {
                List<Rect> frames = new List<Rect>(InternalSpriteUtility.GenerateAutomaticSpriteRectangles((Texture2D)m_SelectObject, 1, 0));
                frames = SortRects(frames, (m_SelectObject as Texture2D).width);
                spriteRects = new SpriteRect[frames.Count];

                for (int i = 0; i < frames.Count; i++)
                {
                    SpriteRect tmp = new SpriteRect();
                    tmp.name = (i + 1).ToString();
                    tmp.pivot = new Vector2(0.5f, 0.5f);
                    tmp.rect = frames[i];
                    spriteRects[i] = tmp;
                }
            }

            TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.isReadable = true;
            textureImporter.fadeout = false;

            SpriteDataProviderFactories factories = new SpriteDataProviderFactories();
            factories.Init();
            ISpriteEditorDataProvider dataProvider = factories.GetSpriteEditorDataProviderFromObject(textureImporter);
            dataProvider.InitSpriteEditorDataProvider();
            dataProvider.SetSpriteRects(spriteRects);
            dataProvider.Apply();

            textureImporter.SaveAndReimport();
            AssetDatabase.ImportAsset(selectionPath);
            AssetDatabase.Refresh();

            // 加载此文件下的所有资源
            UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath(selectionPath);
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

                    if (m_OutPutExtName.Equals(".png"))
                    {
                        bytes = tex.EncodeToPNG();
                    }
                    else if (m_OutPutExtName.Equals(".jpb"))
                    {
                        bytes = tex.EncodeToJPG();
                    }

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
                    textureImporter.SaveAndReimport();
                }
            }
        }

        private List<Rect> SortRects(List<Rect> rects, int textureActualWidth)
        {
            List<Rect> result = new List<Rect>();

            while (rects.Count > 0)
            {
                // Because the slicing algorithm works from bottom-up, the topmost rect is the last one in the array
                Rect r = rects[rects.Count - 1];
                Rect sweepRect = new Rect(0, r.yMin, textureActualWidth, r.height);

                List<Rect> rowRects = RectSweep(rects, sweepRect);

                if (rowRects.Count > 0)
                    result.AddRange(rowRects);
                else
                {
                    // We didn't find any rects, just dump the remaining rects and continue
                    result.AddRange(rects);
                    break;
                }
            }
            return result;
        }

        private List<Rect> RectSweep(List<Rect> rects, Rect sweepRect)
        {
            if (rects == null || rects.Count == 0)
                return new List<Rect>();

            List<Rect> containedRects = new List<Rect>();

            foreach (Rect rect in rects)
            {
                if (rect.Overlaps(sweepRect))
                    containedRects.Add(rect);
            }

            // Remove found rects from original list
            foreach (Rect rect in containedRects)
                rects.Remove(rect);

            // Sort found rects by x position
            containedRects.Sort((a, b) => a.x.CompareTo(b.x));

            return containedRects;
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
        private bool m_OutPutGenMipmaps = false;
        //private bool m_FoldOut = false;
        private bool m_IsUseAuto = false;
        private TextureImporterType m_OutPutType = TextureImporterType.Sprite;
    }
}