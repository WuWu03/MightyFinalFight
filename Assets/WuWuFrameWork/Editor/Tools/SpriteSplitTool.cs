using System;
using System.Collections.Generic;
using System.IO;
using WuWuFramework.Event;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEditorInternal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace WuWuFramework.Editor
{
    public class SpriteSplitTool : EditorWindow
    {
        private readonly string[] m_ExtNames = { ".png", ".jpg" };
        private SplitType m_SplitType = SplitType.Texture2D;
        private int m_SpriteWidth = 256;
        private int m_SpriteHeight = 256;
        private string m_OutPutExtName = ".png";
        private Dictionary<string, WuWuFrameworkFunc<Texture2D, byte[]>> m_DicEncodes;
        private UnityObject m_SelectObject;
        private string m_SelectFolder = string.Empty;
        private string m_OutPutPath = string.Empty;
        private bool m_OutPutGenMipmaps;
        private bool m_IsUseAuto;
        private TextureImporterType m_OutPutType = TextureImporterType.Sprite;
        
        enum SplitType
        {
            [EnumLabel("图片")]
            Texture2D,
            [EnumLabel("路径")]
            Folder,
        }

        public SpriteSplitTool()
        {
            titleContent = new GUIContent(GetType().Name);
        }

        private void OnEnable()
        {
            m_DicEncodes = new()
            {
                { ".png", (tex) => tex.EncodeToPNG() },
                { ".jpg", (tex) => tex.EncodeToJPG() }
            };
        }

        private void OnDisable()
        {
            m_DicEncodes.Clear();
            m_DicEncodes = null;
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
            UnityObject temp = m_SelectObject;
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 5));
            m_SelectObject = EditorGUI.ObjectField(rect, "拖入图片", m_SelectObject, typeof(Texture2D), false);

            if (m_SelectObject == null)
            {
                DefaultSetting();
            }
            else if (m_SelectObject != null && temp != m_SelectObject)
            {
                SetTextureSprite(m_SelectObject as Texture2D);
                AssetDatabase.Refresh();
            }
        }

        private void FolderGUI()
        {
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 5));
            UnityObject temp = m_SelectObject;
            m_SelectObject = EditorGUI.ObjectField(rect, "拖入文件夹", m_SelectObject, typeof(DefaultAsset), false);

            if (m_SelectObject == null)
            {
                DefaultSetting();
            }
            else if (m_SelectObject != null && temp != m_SelectObject)
            {
                m_SelectFolder = AssetDatabase.GetAssetPath(m_SelectObject);
                m_OutPutPath = Application.dataPath + "/SplitSprite" + m_SelectFolder.Substring(m_SelectFolder.LastIndexOf("/", StringComparison.Ordinal)) + "/";
                string path = Application.dataPath + m_SelectFolder.Substring(m_SelectFolder.IndexOf("Assets", StringComparison.Ordinal) + 6);
                string[] files = Directory.GetFiles(path, "*");

                foreach (var file in files)
                {
                    if (Path.GetExtension(file).Equals(".meta")) continue;
                    string objectPath = file.Substring(file.IndexOf("Assets", StringComparison.Ordinal));
                    UnityObject @object = AssetDatabase.LoadAssetAtPath(objectPath, typeof(Texture2D));

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

            string path = Application.dataPath + m_SelectFolder.Substring(m_SelectFolder.IndexOf("Assets", StringComparison.Ordinal) + 6);
            string[] files = Directory.GetFiles(path, "*");

            foreach (var file in files)
            {
                if (Path.GetExtension(file).Equals(".meta")) continue;
                string objectPath = file.Substring(file.IndexOf("Assets", StringComparison.Ordinal));
                UnityObject @object = AssetDatabase.LoadAssetAtPath(objectPath, typeof(Texture2D));

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
                throw new WuWuFrameworkException(error);
            }

            if (!m_IsUseAuto && (m_SpriteWidth > texture.width || m_SpriteHeight > texture.height))
            {
                throw new WuWuFrameworkException("小图宽高不能比主图大，请重新输入正确的数值!");
            }

            return true;
        }

        private void SetTextureSprite(Texture2D texture)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));

            if (assetImporter is not TextureImporter textureImporter)
            {
                throw new WuWuFrameworkException("格式错误");
            }

            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.isReadable = true;
            textureImporter.fadeout = false;
            textureImporter.SaveAndReimport();
            texture.Apply();
        }

        private void SplitSprite(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }

            if (!m_DicEncodes.ContainsKey(m_OutPutExtName))
            {
                throw new WuWuFrameworkException("输出文件格式错误");
            }

            string selectionPath = AssetDatabase.GetAssetPath(texture);
            SpriteRect[] spriteRects;

            if (!m_IsUseAuto)
            {
                int row = Mathf.CeilToInt((float)texture.height / m_SpriteHeight);
                int column = Mathf.CeilToInt((float)texture.width / m_SpriteWidth);
                spriteRects = new SpriteRect[row * column];

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        SpriteRect tmp = new();
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
                List<Rect> frames = new(InternalSpriteUtility.GenerateAutomaticSpriteRectangles((Texture2D)m_SelectObject, 1, 0));
                frames = SortRects(frames, (m_SelectObject as Texture2D).width);
                spriteRects = new SpriteRect[frames.Count];

                for (int i = 0; i < frames.Count; i++)
                {
                    SpriteRect tmp = new()
                    {
                        name = (i + 1).ToString(),
                        pivot = new Vector2(0.5f, 0.5f),
                        rect = frames[i]
                    };
                    spriteRects[i] = tmp;
                }
            }

            AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));

            if (assetImporter is not TextureImporter textureImporter)
            {
                this.ShowNotification(new GUIContent("资源路径错误"));
                throw new WuWuFrameworkException("资源路径错误");
            }
            
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.isReadable = true;
            textureImporter.fadeout = false;

            SpriteDataProviderFactories factories = new();
            factories.Init();
            ISpriteEditorDataProvider dataProvider = factories.GetSpriteEditorDataProviderFromObject(textureImporter);
            dataProvider.InitSpriteEditorDataProvider();
            dataProvider.SetSpriteRects(spriteRects);
            dataProvider.Apply();

            textureImporter.SaveAndReimport();
            AssetDatabase.ImportAsset(selectionPath);
            AssetDatabase.Refresh();

            // 加载此文件下的所有资源
            UnityObject[] selectionObjects = AssetDatabase.LoadAllAssetsAtPath(selectionPath);
            List<Sprite> sprites = new();

            foreach (var selectionObject in selectionObjects)
            {
                if (selectionObject is Sprite sprite)
                {
                    textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite)) as TextureImporter;
                    if (textureImporter != null)
                    {
                        textureImporter.isReadable = true;
                    }
                    sprites.Add(sprite);
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
                    Texture2D tex = new((int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.format, false);
                    tex.SetPixels(sprite.texture.GetPixels((int)sprite.rect.xMin, (int)sprite.rect.yMin, (int)sprite.rect.width, (int)sprite.rect.height));
                    tex.Apply();

                    // 写入成各种格式文件
                    byte[] bytes = m_DicEncodes[m_OutPutExtName]?.Invoke(tex);
                    if (bytes == null)
                    {
                        
                        continue;
                    }

                    File.WriteAllBytes(realOutPutPath + sprite.name + m_OutPutExtName, bytes);
                }

                AssetDatabase.Refresh();

                realOutPutPath = realOutPutPath.Substring(realOutPutPath.IndexOf("Assets", StringComparison.Ordinal));

                foreach (Sprite sprite in sprites)
                {
                    textureImporter = AssetImporter.GetAtPath(realOutPutPath + sprite.name + m_OutPutExtName) as TextureImporter;
                    if (textureImporter != null)
                    {
                        textureImporter.isReadable = true;
                        textureImporter.textureType = m_OutPutType;
                        textureImporter.spriteImportMode = SpriteImportMode.Single;
                        textureImporter.fadeout = false;
                        textureImporter.mipmapEnabled = m_OutPutGenMipmaps;
                        textureImporter.SaveAndReimport();
                    }
                }
            }
        }

        private List<Rect> SortRects(List<Rect> rects, int textureActualWidth)
        {
            List<Rect> result = new();

            while (rects.Count > 0)
            {
                // Because the slicing algorithm works from bottom-up, the topmost rect is the last one in the array
                Rect r = rects[^1];
                Rect sweepRect = new(0, r.yMin, textureActualWidth, r.height);

                List<Rect> rowRects = RectSweep(rects, sweepRect);

                if (rowRects.Count > 0)
                {
                    result.AddRange(rowRects);
                }
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
            {
                return new List<Rect>();
            }

            List<Rect> containedRects = new();

            foreach (Rect rect in rects)
            {
                if (rect.Overlaps(sweepRect))
                {
                    containedRects.Add(rect);
                }
            }

            // Remove found rects from original list
            foreach (Rect rect in containedRects)
            {
                rects.Remove(rect);
            }

            // Sort found rects by x position
            containedRects.Sort((a, b) => a.x.CompareTo(b.x));
            return containedRects;
        }
        
        private void OnDestroy()
        {
            m_SelectObject = null;
            m_SelectFolder = string.Empty;
        }
    }
}