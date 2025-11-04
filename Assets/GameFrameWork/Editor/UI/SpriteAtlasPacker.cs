using GameFrameWork.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using FileUtil = GameFrameWork.Utils.FileUtil;

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

            m_ListSprites ??= new List<Sprite>();
            m_ListSpriteStatus ??= new List<SpriteStatus>();
            m_ListAtlas ??= new List<SpriteAtlas>();
            m_ListAtalsNames ??= new List<string>();

            m_ListSprites.Clear();
            m_ListSpriteStatus.Clear();
            m_ListAtlas.Clear();
            m_ListAtalsNames.Clear();

            if (Selection.objects != null && Selection.objects.Length > 0)
            {
                int defaultAssetIndex = -1;

                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    UnityEngine.Object obj = Selection.objects[i];

                    if (obj is UnityEditor.DefaultAsset)
                    {
                        defaultAssetIndex = i;
                        break;
                    }
                }

                if (defaultAssetIndex > -1)
                {
                    string assetPath = PathUtil.GetAssetFullPath(AssetDatabase.GetAssetPath(Selection.objects[defaultAssetIndex]));
                    string[] assetFiles = FileUtil.GetFiles(assetPath);

                    if (assetFiles != null && assetFiles.Length > 0)
                    {
                        for (int i = 0; i < assetFiles.Length; i++)
                        {
                            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(PathUtil.GetAssetPath(assetFiles[i]));
                            if (sprite != null)
                            {
                                m_ListSprites.Add(sprite);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Selection.objects.Length; i++)
                    {
                        string assetPath = PathUtil.GetAssetFullPath(AssetDatabase.GetAssetPath(Selection.objects[i]));
                        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(PathUtil.GetAssetPath(assetPath));

                        if (sprite != null)
                        {
                            m_ListSprites.Add(sprite);
                        }
                    }
                }
            }

            string atlasPath = EditorPathUtil.GetUIAtlasPath();
            string[] atlasFiles = FileUtil.GetFiles(PathUtil.GetAssetFullPath(atlasPath));

            if (atlasFiles != null && atlasFiles.Length > 0)
            {
                for (int i = 0; i < atlasFiles.Length; i++)
                {
                    m_ListAtlas.Add(AssetDatabase.LoadAssetAtPath<SpriteAtlas>(PathUtil.GetAssetPath(atlasFiles[i])));
                    m_ListAtalsNames.Add(m_ListAtlas[i].name);

                    if (m_ListSprites.Count > 0 && m_CurrAtlasIndex == -1)
                    {
                        UnityEngine.Object[] atalsSprites = m_ListAtlas[i].GetPackables();
                        int count = Math.Max(atalsSprites.Length, m_ListSprites.Count);

                        for (int j = 0; j < count; j++)
                        {
                            if (j >= atalsSprites.Length || j >= m_ListSprites.Count || atalsSprites[j] == null || m_ListSprites[j] == null)
                            {
                                continue;
                            }

                            string atlasSpritePath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(atalsSprites[j])).Replace("\\", "/");
                            string spritePath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(m_ListSprites[j])).Replace("\\", "/");

                            if (atlasSpritePath == spritePath)
                            {
                                m_CurrAtlasIndex = i;
                                break;
                            }
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

            if (m_CurrAtlasIndex != atlasIndex)
            {
                m_CurrAtlasIndex = atlasIndex;
                UpdateSpriteStatus();
            }

            string atlasName = EditorGUILayout.TextField("新图集名称", m_NewAtalsName);

            if (string.IsNullOrEmpty(atlasName))
            {
                atlasName = "NewAtlas";
            }

            if (atlasName != m_NewAtalsName)
            {
                m_NewAtalsName = Path.GetFileNameWithoutExtension(atlasName);
            }

            if (GUILayout.Button("创建新图集"))
            {
                CreateNewAtals();
            }

            if (m_CurrAtlasIndex > -1 && m_ListSprites.Count > 0)
            {
                if (GUILayout.Button("构建所选图集"))
                {
                    BuildAtals();
                }
            }

            pos = EditorGUILayout.BeginScrollView(pos);
            EditorGUILayout.BeginVertical();

            m_FoldOut = EditorGUILayout.Foldout(m_FoldOut, "Sprites");

            if (m_FoldOut)
            {
                for (int i = 0; i < m_ListSpriteStatus.Count; i++)
                {
                    SpriteStatus spriteStatus = m_ListSpriteStatus[i];
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(13);
                    EditorGUILayout.LabelField(spriteStatus.sprite == null ? "空" : spriteStatus.sprite.name);

                    string str = string.Empty;
                    Color strColor = Color.clear;

                    if (spriteStatus.status == 0)
                    {
                        strColor = Color.white;
                        str = "无变化";
                    }
                    else if (spriteStatus.status == 1)
                    {
                        strColor = Color.green;
                        str = "新增";
                    }
                    else if (spriteStatus.status == 2)
                    {
                        strColor = Color.yellow;
                        str = "更新";
                    }
                    else if (spriteStatus.status == 3)
                    {
                        strColor = Color.red;
                        str = "移除";
                    }

                    GUIStyle labelStyle = new(EditorStyles.label);
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
            m_ListSprites.Sort((a, b) =>
            {
                return a.name.CompareTo(b.name);
            });

            if (m_CurrAtlasIndex > -1)
            {
                UnityEngine.Object[] atlasSprites = SpriteAtlasExtensions.GetPackables(m_ListAtlas[m_CurrAtlasIndex]);

                if (m_ListSprites.Count > 0)
                {
                    List<Sprite> listSprites = new(m_ListSprites.ToArray());

                    for (int i = 0; i < atlasSprites.Length; i++)
                    {
                        if (atlasSprites[i] == null)
                        {
                            m_ListSpriteStatus.Add(new SpriteStatus() { sprite = null, status = 3 });
                        }
                        else
                        {
                            Sprite atlasSprite = atlasSprites[i] as Sprite;
                            Sprite tempSprite = null;

                            for (int j = 0; j < listSprites.Count; j++)
                            {
                                if (listSprites[j].name == atlasSprite.name)
                                {
                                    tempSprite = listSprites[j];
                                    listSprites.RemoveAt(j);
                                    break;
                                }
                            }

                            if (tempSprite == null)
                            {
                                m_ListSpriteStatus.Add(new SpriteStatus() { sprite = atlasSprite, status = 3 });
                            }
                            else
                            {
                                string md51 = StringUtil.MD5(atlasSprite.texture.GetRawTextureData());
                                string md52 = StringUtil.MD5(tempSprite.texture.GetRawTextureData());

                                if (md51 != md52)
                                {
                                    m_ListSpriteStatus.Add(new SpriteStatus() { sprite = tempSprite, status = 2 });
                                }
                                else
                                {
                                    m_ListSpriteStatus.Add(new SpriteStatus() { sprite = atlasSprite, status = 0 });
                                }
                            }
                        }

                    }

                    for (int i = 0; i < listSprites.Count; i++)
                    {
                        m_ListSpriteStatus.Add(new SpriteStatus() { sprite = listSprites[i], status = 1 });
                    }
                }
                else
                {
                    for (int i = 0; i < atlasSprites.Length; i++)
                    {
                        m_ListSpriteStatus.Add(new SpriteStatus() { sprite = atlasSprites[i] as Sprite, status = 0 });
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
        }

        private bool CreateNewAtals()
        {
            string atlasPath = EditorMgr.GetGameFrameWorkConfig().uiAtlasPath;
            string atlasCreatePath = PathUtil.FormatPath(atlasPath, m_NewAtalsName + ".spriteatlas");

            if (File.Exists(atlasCreatePath))
            {
                if (!EditorUtility.DisplayDialog("创建图集", "图集 [" + m_NewAtalsName + ".spriteatlas] 已存在，是否进行覆盖？", "确定", "取消"))
                {
                    return false;
                }
            }
            else if (!EditorUtility.DisplayDialog("创建图集", "是否创建图集 [" + m_NewAtalsName + ".spriteatlas] ？", "确定", "取消"))
            {
                return false;
            }

            SpriteAtlas atlas = GetNewSpriteAtlas();
            AssetDatabase.CreateAsset(atlas, atlasCreatePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            m_ListAtlas.Add(atlas);
            m_ListAtalsNames.Add(atlas.name);
            m_DisplayAtlasNames = m_ListAtalsNames.ToArray();
            m_CurrAtlasIndex = m_ListAtlas.Count - 1;
            UpdateSpriteStatus();

            if (EditorUtility.DisplayDialog("创建图集", "创建图集 [" + m_NewAtalsName + "] 成功", "确定"))
            {
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(atlasCreatePath);
            }

            return true;
        }

        private void BuildAtals()
        {
            bool canBuild = true;

            if (m_ListAtlas == null || m_ListAtlas.Count < 1 || m_CurrAtlasIndex < 0 || m_CurrAtlasIndex >= m_ListAtlas.Count)
            {
                canBuild = CreateNewAtals();
            }

            if (!canBuild)
            {
                return;
            }

            string atlasPath = PathUtil.GetAssetPath(PathUtil.FormatPath(EditorMgr.GetGameFrameWorkConfig().uiAtlasPath, m_ListAtlas[m_CurrAtlasIndex].name, ".spriteatlas"));
            Pack(atlasPath);
        }

        private void Pack(string atlasPath)
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

            if (!hasChanged)
            {
                ShowNotification(new GUIContent("图集未发生变化，无需构建图集"));
                return;
            }

            File.Delete(AssetDatabase.GetAssetPath(m_ListAtlas[m_CurrAtlasIndex]));
            List<Sprite> packList = new();

            for (int i = 0; i < m_ListSpriteStatus.Count; i++)
            {
                if (m_ListSpriteStatus[i].status != 3)
                {
                    packList.Add(m_ListSpriteStatus[i].sprite);
                }
            }

            SpriteAtlas atlas = GetNewSpriteAtlas();
            atlas.Add(packList.ToArray());

            AssetDatabase.CreateAsset(atlas, atlasPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ShowNotification(new GUIContent("图集构建成功"));

            m_ListAtlas[m_CurrAtlasIndex] = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
            Selection.activeObject = m_ListAtlas[m_CurrAtlasIndex];
            UpdateSpriteStatus();
        }

        private SpriteAtlas GetNewSpriteAtlas()
        {
            SpriteAtlas atlas = new();
            atlas.SetIncludeInBuild(true);
            atlas.SetPackingSettings(new SpriteAtlasPackingSettings()
            {
                enableAlphaDilation = true,
                enableRotation = true,
                enableTightPacking = false,
                padding = 4,
            });

            SetAltasPlatformSettings(atlas, EditorUserBuildSettings.activeBuildTarget);
            SetAltasPlatformSettings(atlas, BuildTarget.StandaloneWindows);
            SetAltasPlatformSettings(atlas, BuildTarget.StandaloneWindows64);
            SetAltasPlatformSettings(atlas, BuildTarget.Android);

            return atlas;
        }

        private void SetAltasPlatformSettings(SpriteAtlas atlas, BuildTarget buildTarget)
        {
            TextureImporterFormat compressFormat = TextureImporterFormat.Automatic;

            if (buildTarget == BuildTarget.Android ||
                buildTarget == BuildTarget.iOS ||
                buildTarget == BuildTarget.WebGL ||
                buildTarget == BuildTarget.PS5 ||
                buildTarget == BuildTarget.XboxOne ||
                buildTarget == BuildTarget.Switch)
            {
                compressFormat = TextureImporterFormat.ASTC_5x5;
            }
            else if (buildTarget == BuildTarget.StandaloneWindows ||
                    buildTarget == BuildTarget.StandaloneWindows64 ||
                    buildTarget == BuildTarget.StandaloneLinux64 ||
                    buildTarget == BuildTarget.StandaloneOSX)
            {
                compressFormat = TextureImporterFormat.BC7;
            }

            atlas.SetPlatformSettings(new TextureImporterPlatformSettings
            {
                name = buildTarget.ToString(),
                overridden = true,
                maxTextureSize = 2048,
                textureCompression = TextureImporterCompression.Compressed,
                format = compressFormat,
                crunchedCompression = false,
            });
        }

        private bool m_FoldOut = true;
        private int m_CurrAtlasIndex = -1;
        private string m_NewAtalsName = string.Empty;
        private List<Sprite> m_ListSprites = null;
        private List<SpriteStatus> m_ListSpriteStatus = null;
        private List<string> m_ListAtalsNames = null;
        private string[] m_DisplayAtlasNames = null;
        private List<SpriteAtlas> m_ListAtlas = null;
    }
}