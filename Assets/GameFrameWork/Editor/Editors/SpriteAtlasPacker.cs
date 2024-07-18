using DG.DemiEditor;
using GameFrameWork.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using FileUtil = GameFrameWork.Utilities.FileUtil;

namespace GameFrameWork.Editor
{
    public class SpriteAtlasPacker : EditorWindow
    {
        class SpriteStatus
        {
            public Sprite sprite;
            public int status;
        }

        public SpriteAtlasPacker()
        {
            titleContent = new GUIContent(this.GetType().Name);

        }

        private void OnEnable()
        {
            position = new Rect(0, 0, 500, 300);

            if(m_ListSprites == null)
            {
                m_ListSprites = new List<Sprite>();
            }

            if (m_ListSpriteStatus == null)
            {
                m_ListSpriteStatus = new List<SpriteStatus>();
            }

            if (m_ListAtlas == null)
            {
                m_ListAtlas = new List<SpriteAtlas>();
                m_ListAtalsNames = new List<string>();
            }

            m_ListSprites.Clear();
            m_ListSpriteStatus.Clear();
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
                        m_ListSprites.Add(AssetDatabase.LoadAssetAtPath<Sprite>(PathUtil.GetAssetPath(assetFiles[i])));
                    }
                }
                else
                {
                    for (int i = 0; i < Selection.objects.Length; i++)
                    {
                        string assetPath = PathUtil.GetAssetFullPath(AssetDatabase.GetAssetPath(Selection.objects[i]));
                        m_ListSprites.Add(AssetDatabase.LoadAssetAtPath<Sprite>(PathUtil.GetAssetPath(assetPath)));
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

            UpdateSpriteStatus();
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
                UpdateSpriteStatus();
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
                for (int i = 0; i < m_ListSpriteStatus.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(13);
                    EditorGUILayout.LabelField(m_ListSpriteStatus[i].sprite.name);

                    string str = string.Empty;
                    Color strColor = Color.clear;

                    if (m_ListSpriteStatus[i].status == 1)
                    {
                        strColor = Color.green;
                        str = "Add";
                    }
                    else if(m_ListSpriteStatus[i].status == 2)
                    {
                        strColor = Color.yellow;
                        str = "Update";
                    }
                    else if(m_ListSpriteStatus[i].status == 3)
                    {
                        strColor = Color.red;
                        str = "Remove";
                    }

                    GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                    labelStyle.normal.textColor = strColor;
                    EditorGUILayout.LabelField(str, labelStyle);
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void UpdateSpriteStatus()
        {
            m_ListSpriteStatus.Clear();

            if (m_CurrAtlasIndex > -1)
            {
                UnityEngine.Object[] atlasSprites = SpriteAtlasExtensions.GetPackables(m_ListAtlas[m_CurrAtlasIndex]);

                for (int i = 0; i < atlasSprites.Length; i++)
                {
                    Sprite tempSprite = atlasSprites[i] as Sprite;
                    Sprite findSprite = null;

                    string tempGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(tempSprite));

                    for (int j = 0; j < m_ListSprites.Count; j++)
                    {
                        string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(m_ListSprites[j]));

                        if (tempGuid.Equals(guid))
                        {
                            findSprite = m_ListSprites[j];
                            break;
                        }
                    }

                    if (findSprite == null)
                    {
                        m_ListSpriteStatus.Add(new SpriteStatus() { sprite = tempSprite, status = 3 });
                    }
                    else
                    {
                        string md51 = StringUtil.MD5(findSprite.texture.GetRawTextureData());
                        string md52 = StringUtil.MD5(tempSprite.texture.GetRawTextureData());

                        if (md51 != md52)
                        {
                            m_ListSpriteStatus.Add(new SpriteStatus() { sprite = findSprite, status = 2 });
                        }
                        else
                        {
                            m_ListSpriteStatus.Add(new SpriteStatus() { sprite = findSprite, status = 0 });
                        }
                    }
                }

                for (int i = 0; i < m_ListSprites.Count; i++)
                {
                    Sprite tempSprite = m_ListSprites[i];
                    Sprite findSprite = null;

                    for (int j = 0; j < atlasSprites.Length; j++)
                    {
                        if (tempSprite.name.Contains(atlasSprites[j].name))
                        {
                            findSprite = atlasSprites[j] as Sprite;
                            break;
                        }
                    }

                    if (findSprite == null)
                    {
                        m_ListSpriteStatus.Add(new SpriteStatus() { sprite = tempSprite, status = 1 });
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_ListSprites.Count; i++)
                {
                    m_ListSpriteStatus.Add(new SpriteStatus() { sprite = m_ListSprites[i], status = 1 });
                }
            }

            m_ListSpriteStatus.Sort((a,b)=> 
            {
                return a.sprite.name.CompareTo(b.sprite.name);
            });
        }

        private void CreateNewAtals()
        {
            string path = UnityEditor.EditorUtility.SaveFilePanelInProject("创建新图集", "New Atlas", "spriteatlas", "Save Atals as...", PathUtil.GetAssetFullPath(m_BuildPath));

            if (!string.IsNullOrEmpty(path))
            {
                SpriteAtlas atlas = new SpriteAtlas();
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
                    textureCompression = TextureImporterCompression.CompressedLQ,
                    format = TextureImporterFormat.Automatic,
                });

                AssetDatabase.CreateAsset(atlas, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                m_ListAtlas.Add(atlas);
                m_ListAtalsNames.Add(atlas.name);
                m_DisplayAtlasNames = m_ListAtalsNames.ToArray();
                m_CurrAtlasIndex = m_ListAtlas.Count - 1;
                UpdateSpriteStatus();
            }
        }

        private void BuildAtals()
        {
            if(m_ListAtlas == null || m_ListAtlas.Count < 1 || m_CurrAtlasIndex < 0 || m_CurrAtlasIndex >= m_ListAtlas.Count)
            {
                CreateNewAtals();
            }

            string atlasPath = PathUtil.GetAssetPath(PathUtil.FormatPath(m_BuildPath, m_ListAtlas[m_CurrAtlasIndex].name));
            Pack(atlasPath);
        }

        private void Pack(string path)
        {
            bool hasChanged = false;

            for (int i = 0; i < m_ListSpriteStatus.Count; i++)
            {
                if (m_ListSpriteStatus[i].status != 0)
                {
                    hasChanged = true;
                    break;
                }
            }

            if(!hasChanged)
            {
                ShowNotification(new GUIContent("图集未发生变化，无需打包图集"));
                return;
            }

            UnityEngine.Object[] packables = SpriteAtlasExtensions.GetPackables(m_ListAtlas[m_CurrAtlasIndex]);
            SpriteAtlasExtensions.Remove(m_ListAtlas[m_CurrAtlasIndex], packables);

            List<Sprite> packList = new List<Sprite>();

            for (int i = 0; i < m_ListSpriteStatus.Count; i++)
            {
                if (m_ListSpriteStatus[i].status != 3)
                {
                    packList.Add(m_ListSpriteStatus[i].sprite);
                }
            }

            SpriteAtlasExtensions.Add(m_ListAtlas[m_CurrAtlasIndex], packList.ToArray());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UpdateSpriteStatus();
            ShowNotification(new GUIContent("图集打包成功"));
            Selection.activeObject = m_ListAtlas[m_CurrAtlasIndex];
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
        private List<SpriteStatus> m_ListSpriteStatus = null;
        private List<string> m_ListAtalsNames = null;
        private string[] m_DisplayAtlasNames = null;
        private List<SpriteAtlas> m_ListAtlas = null;
    }
}