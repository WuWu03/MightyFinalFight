using DG.DemiEditor;
using GameFrameWork.Editor.Config;
using GameFrameWork.Utilities;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.U2D;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;
using FileUtil = GameFrameWork.Utilities.FileUtil;

namespace GameFrameWork.Editor
{
    public class SpriteAtlasPacker : EditorWindow
    {
        public SpriteAtlasPacker()
        {
            titleContent = new GUIContent(this.GetType().Name);

        }

        private void OnEnable()
        {
            position = new Rect(0, 0, 500, 300);

            if (m_ListSprites == null)
            {
                m_ListSprites = new List<Sprite>();
            }

            if (m_ListAtlas == null)
            {
                m_ListAtlas = new List<SpriteAtlas>();
                m_ListAtalsNames = new List<string>();
            }

            m_ListSprites.Clear();
            m_ListAtlas.Clear();
            m_ListAtalsNames.Clear();

            if (Selection.objects != null && Selection.objects.Length > 0)
            {
                if (Selection.objects[0] is UnityEditor.DefaultAsset)
                {
                    string assetPath = PathUtil.GetAssetFullPath(AssetDatabase.GetAssetPath(Selection.objects[0]));
                    string[] assetFiles = FileUtil.GetFiles(assetPath);

                    if (assetFiles == null || assetFiles.Length == 0)
                    {
                        return;
                    }

                    for (int i = 0; i < assetFiles.Length; i++)
                    {
                        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(PathUtil.GetAssetPath(assetFiles[i]));

                        if(sprite != null)
                        {
                            m_ListSprites.Add(sprite);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Selection.objects.Length; i++)
                    {
                        if (Selection.objects[i].GetType() != typeof(Texture2D))
                        {
                            continue;
                        }

                        Texture2D t2 = Selection.objects[i] as Texture2D;
                        m_ListSprites.Add(Sprite.Create(t2, new Rect(0, 0, t2.width, t2.height), Vector2.zero));
                    }
                }
            }

            string atlasPath = PathUtil.GetUIAtlasPath();
            string[] atlasFiles = FileUtil.GetFiles(PathUtil.GetAssetFullPath(atlasPath));

            if (atlasFiles != null && atlasFiles.Length > 0)
            {
                for (int i = 0; i < atlasFiles.Length; i++)
                {
                    m_ListAtlas.Add(AssetDatabase.LoadAssetAtPath<SpriteAtlas>(PathUtil.GetAssetPath(atlasFiles[i])));
                    m_ListAtalsNames.Add(m_ListAtlas[i].name);

                    if(m_ListSprites.Count > 0 && m_CurrAtlasIndex == -1)
                    {
                        string objPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(m_ListSprites[0])).Replace("\\", "/");

                        if (objPath.Contains(m_ListAtlas[i].name))
                        {
                            m_CurrAtlasIndex = i;
                        }
                    }
                }
            }

            m_DisplayAtlasNames = m_ListAtalsNames.ToArray();
        }

        Vector2 pos;
        private void OnGUI()
        {
            int atlasIndex = EditorGUILayout.Popup("当前图集", m_CurrAtlasIndex, m_DisplayAtlasNames);
            m_BuildPath = EditorGUILayout.TextField("图集路径", m_BuildPath);

            if (string.IsNullOrEmpty(m_BuildPath))
            {
                m_BuildPath = PathUtil.GetUIAtlasPath() + "/";
            }

            if(m_CurrAtlasIndex != atlasIndex)
            {
                m_CurrAtlasIndex = atlasIndex;
            }

            if (GUILayout.Button("Create New Atlas"))
            {
                CreateNewAtals();
            }

            if (GUILayout.Button("Build Atlas"))
            {
                BuildAtals();
            }

            pos = EditorGUILayout.BeginScrollView(pos);
            EditorGUILayout.BeginVertical();

            m_FoldOut = EditorGUILayout.Foldout(m_FoldOut, "Sprites");

            if (m_FoldOut)
            {
                for (int i = 0; i < m_ListSprites.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(13);
                    EditorGUILayout.LabelField(m_ListSprites[i].name);

                    int status = SpriteStatus(m_ListSprites[i]);
                    
                    if (status == 1)
                    {
                        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                        labelStyle.normal.textColor = Color.green;
                        EditorGUILayout.LabelField("Add", labelStyle);
                    }
                    else if(status == 2)
                    {
                        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                        labelStyle.normal.textColor = Color.red;
                        EditorGUILayout.LabelField("Update", labelStyle);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }


            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private int SpriteStatus(Sprite sprite, SpriteAtlas atlas = null)
        {
            Sprite spriteOld = GetSprite(sprite.name, atlas);

            if (spriteOld == null)
            {
                return 1;//新增
            }
            else
            {
                string md51 = StringUtil.MD5(sprite.texture.GetRawTextureData());
                string md52 = StringUtil.MD5(spriteOld.texture.GetRawTextureData());

                if (md51 != md52)
                {
                    return 2;//更新
                }
            }

            return 0;//无变化
        }

        private Sprite GetSprite(string spriteName, SpriteAtlas atlas)
        {
            if (atlas != null)
            {
                return atlas.GetSprite(spriteName);

            }
            if (m_ListAtlas == null || m_ListAtlas.Count < 1 || m_CurrAtlasIndex < 0 || m_CurrAtlasIndex >= m_ListAtlas.Count)
            {
                return null;
            }

            return m_ListAtlas[m_CurrAtlasIndex].GetSprite(spriteName);
        }

        private void CreateNewAtals()
        {
            string path = UnityEditor.EditorUtility.SaveFilePanelInProject("创建新图集", "New Atlas", "spriteatlas", "Save Atals as...", PathUtil.GetAssetFullPath(m_BuildPath));

            if (!string.IsNullOrEmpty(path))
            {
                Pack(null, null, path);
            }
        }

        private void BuildAtals()
        {
            if(m_ListAtlas == null || m_ListAtlas.Count < 1 || m_CurrAtlasIndex < 0 || m_CurrAtlasIndex >= m_ListAtlas.Count)
            {
                CreateNewAtals();
            }

            string atlasPath = PathUtil.GetAssetPath(PathUtil.FormatPath(m_BuildPath, m_ListAtlas[m_CurrAtlasIndex].name));
            Pack(m_ListSprites.ToArray(), m_ListAtlas[m_CurrAtlasIndex], atlasPath + ".spriteatlas");
        }

        private void Pack(Sprite[] sprites, SpriteAtlas atlas, string path)
        {
            bool isCreate = false;

            if(atlas == null)
            {
                atlas = new SpriteAtlas();
                isCreate = true;
            }

            List<Sprite> packSpries = new List<Sprite>();

            if (sprites != null && sprites.Length > 0)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    int spriteStatus = SpriteStatus(sprites[i], atlas);

                    if (spriteStatus == 1 || spriteStatus == 2)
                    {
                        packSpries.Add(sprites[i]);
      
                    }
                }
            }

            if(!isCreate && packSpries.Count > 0)
            {
                atlas.Add(packSpries.ToArray());
            }

            atlas.SetIncludeInBuild(true);
            atlas.SetPackingSettings(new SpriteAtlasPackingSettings()
            {
                enableAlphaDilation = true,
                enableRotation = true,
                enableTightPacking = false,
                padding = 4,
            });
            atlas.GetPlatformSettings(GetPlatformName(EditorUserBuildSettings.activeBuildTarget));
            atlas.SetPlatformSettings(new TextureImporterPlatformSettings
            {
                maxTextureSize = 2048,
                textureCompression = TextureImporterCompression.CompressedHQ,
                format = TextureImporterFormat.Automatic,  
            });

            if (isCreate)
            {
                AssetDatabase.CreateAsset(atlas, path);
                m_ListAtlas.Add(atlas);
                m_ListAtalsNames.Add(atlas.name);
                m_DisplayAtlasNames = m_ListAtalsNames.ToArray();
                m_CurrAtlasIndex = m_ListAtlas.Count - 1;
            }
            else
            {

            }

            AssetDatabase.Refresh();
        }

        private static string GetPlatformName(BuildTarget target)
        {
            string platformName = "";
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                    platformName = "StandaloneWindows";
                    break;
                case BuildTarget.Android:
                    platformName = "Android";
                    break;
                case BuildTarget.iOS:
                    platformName = "iOS";
                    break;
            }
            return platformName;
        }

        private string m_BuildPath = string.Empty;
        private bool m_FoldOut = true;
        private int m_CurrAtlasIndex = -1;
        private List<Sprite> m_ListSprites = null;
        private List<string> m_ListAtalsNames = null;
        private string[] m_DisplayAtlasNames = null;
        private List<SpriteAtlas> m_ListAtlas = null;
    }
}