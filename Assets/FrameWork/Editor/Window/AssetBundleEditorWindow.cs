using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameFrameWork.Utils;
using System;
using System.IO;

namespace GameFrameWork.Editor
{
    public class AssetBundleEditorWindow : EditorWindow
    {
        public AssetBundleEditorWindow()
        {
            titleContent = new GUIContent(this.GetType().Name);
            m_ListData = new List<AssetBundleData>();
            m_ListPatternIndex = new List<int>();
            m_ListBundleExtendIndex = new List<int>();
            m_ListDataHasRemove = new List<bool>();
        }

        private void OnDisable()
        {
            Debug.Log("保存数据");
            SaveConfig();
        }

        private void OnGUI()
        {
            InitConfig();

            MainGUI();
            ExtendNameGUI();
            PatternGUI();
            PlatFormSelectGUI();
            ConfigButtonGUI();
            BuildGUI();
        }

        private void InitConfig()
        {
            if (m_AssetBundleConfig != null) return;
            string configPath = Application.dataPath + PathUtil.AssetBundleDataPath.Substring(PathUtil.AssetBundleDataPath.IndexOf("Assets") + "Assets".Length);
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }

            if (!File.Exists(configPath + PathUtil.AssetBundleConfig.Substring(PathUtil.AssetBundleConfig.IndexOf("Assets") + "Assets".Length)))
            {
                Utility.CreateConfigData<AssetBundleConfig, AssetBundleData>(PathUtil.AssetBundleDataName, PathUtil.AssetBundleDataExtend, configPath);
            }

            m_AssetBundleConfig = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(PathUtil.AssetBundleConfig);
            m_ListData.AddRange(m_AssetBundleConfig.Datas);
            m_ListPatternIndex.AddRange(new int[m_ListData.Count]);
            m_ListBundleExtendIndex.AddRange(new int[m_ListData.Count]);
            m_ListDataHasRemove.AddRange(new bool[m_ListData.Count]);
        }

        private void SaveConfig()
        {
            List<AssetBundleData> list = new List<AssetBundleData>();
            for (int i = 0; i < m_ListData.Count; i++)
            {
                if (!m_ListDataHasRemove[i]) list.Add(m_ListData[i]);
            }

            m_AssetBundleConfig.Datas = list.ToArray();
            m_ListData.Clear();
            m_ListPatternIndex.Clear();
            m_ListDataHasRemove.Clear();
            m_ListBundleExtendIndex.Clear();
            m_ListData.AddRange(m_AssetBundleConfig.Datas);
            m_ListPatternIndex.AddRange(new int[m_ListData.Count]);
            m_ListBundleExtendIndex.AddRange(new int[m_ListData.Count]);
            m_ListDataHasRemove.AddRange(new bool[m_ListData.Count]);
        }

        Vector2 scrollPosition = Vector2.zero;
    
        private void MainGUI()
        {
            if (m_AssetBundleConfig.ListExtendName == null) m_AssetBundleConfig.ListExtendName = new List<string>();
            if (m_AssetBundleConfig.ListPattern == null) m_AssetBundleConfig.ListPattern = new List<string>();

            PathUtil.AssetsDirectory = EditorGUILayout.TextField("资源打包路径", PathUtil.AssetsDirectory);
            if (!PathUtil.AssetsDirectory.StartsWith("Assets/"))
            {
                PathUtil.AssetsDirectory = "Assets/" + PathUtil.AssetsDirectory;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            int index = 0;
            for (int i = 0; i < m_ListData.Count; i++)
            {
                if (m_ListDataHasRemove[i]) continue;
                index++;
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUIStyle style = new GUIStyle();
                style.alignment = TextAnchor.MiddleLeft;
                style.fontSize = 18;
                style.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField(index.ToString(), style);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("x"))
                {
                    m_ListDataHasRemove[i] = true;
                }
                GUILayout.EndHorizontal();

                m_ListData[i].BundleType = (AssetBundleData.AssetType)EditorGUILayout.EnumPopup("包类型：", m_ListData[i].BundleType);
                m_ListData[i].AssetPath = EditorGUILayout.TextField("资源路径：", m_ListData[i].AssetPath);

                if (m_ListData[i].BundleType == AssetBundleData.AssetType.MapSingle)
                {
                    m_ListData[i].AssetBundlePath = EditorGUILayout.TextField("包路径： ", m_ListData[i].AssetBundlePath);
                }
                else
                {
                    m_ListData[i].BundleName = EditorGUILayout.TextField("包名称: ", m_ListData[i].BundleName);
                }
  
                if (m_AssetBundleConfig.ListExtendName.Count > 0)
                {
                    m_ListBundleExtendIndex[i] = EditorGUILayout.Popup("包扩展名：", m_ListBundleExtendIndex[i], m_AssetBundleConfig.ListExtendName.ToArray());
                    m_ListData[i].BundleExtend = m_AssetBundleConfig.ListExtendName[m_ListBundleExtendIndex[i]];
                }

                if (m_AssetBundleConfig.ListPattern.Count > 0)
                {
                    m_ListPatternIndex[i] = EditorGUILayout.Popup("文件过滤：", m_ListPatternIndex[i], m_AssetBundleConfig.ListPattern.ToArray());
                    m_ListData[i].Pattern = m_AssetBundleConfig.ListPattern[m_ListPatternIndex[i]];
                }

                GUILayout.EndVertical();
                if (i < m_ListData.Count - 1)
                    GUILayout.Space(30);
            }

            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
        }

        string extend = string.Empty;
        int removeExtendIndex = 0;
        private void ExtendNameGUI()
        {
            //--------------添加扩展名----------------
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("  添加扩展名  "))
            {
                if (string.IsNullOrEmpty(extend))
                {
                    ShowNotification(new GUIContent("扩展名不能为空"));
                }
                else
                {
                    if (extend.Contains("."))
                    {
                        extend = extend.Substring(extend.LastIndexOf("."));
                    }

                    if (!extend.StartsWith("."))
                    {
                        extend = "." + extend;
                    }

                    if (!m_AssetBundleConfig.ListExtendName.Contains(extend))
                    {
                        m_AssetBundleConfig.ListExtendName.Add(extend);
                        ShowNotification(new GUIContent("添加成功"));
                    }
                    else
                    {
                        ShowNotification(new GUIContent("已存在相同的扩展名"));
                    }
                }
            }
            extend = EditorGUILayout.TextField(extend);
            GUILayout.EndHorizontal();

            //--------------移除扩展名----------------
            if (m_AssetBundleConfig.ListExtendName.Count > 0)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("  移除扩展名  "))
                {
                    if (UnityEditor.EditorUtility.DisplayDialog("提示", "确认移除吗？", "确认", "取消"))
                    {
                        m_AssetBundleConfig.ListExtendName.RemoveAt(removeExtendIndex);
                    }
                }

                removeExtendIndex = EditorGUILayout.Popup(removeExtendIndex, m_AssetBundleConfig.ListExtendName.ToArray());
                GUILayout.EndHorizontal();
            }
        }

        string pattern = string.Empty;
        int removePatternIndex = 0;
        private void PatternGUI()
        {
            //--------------添加过滤器----------------
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("添加过滤器名"))
            {
                if (string.IsNullOrEmpty(pattern))
                {
                    ShowNotification(new GUIContent("过滤器名不能为空"));
                }
                else
                {
                    if(pattern.Contains("."))
                    {
                        pattern = "*" + pattern.Substring(pattern.LastIndexOf("."));
                    }

                    if (!pattern.StartsWith("*."))
                    {
                        pattern = "*." + pattern;
                    }

                    if (!m_AssetBundleConfig.ListPattern.Contains(pattern))
                    {
                        m_AssetBundleConfig.ListPattern.Add(pattern);
                        ShowNotification(new GUIContent("添加成功"));
                    }
                    else
                    {
                        ShowNotification(new GUIContent("已存在相同的过滤器名"));
                    }
                }
            }
            pattern = EditorGUILayout.TextField(pattern);
            GUILayout.EndHorizontal();

            //--------------移除过滤器----------------
            if (m_AssetBundleConfig.ListPattern.Count > 0)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("移除过滤器名"))
                {
                    if (UnityEditor.EditorUtility.DisplayDialog("提示", "确认移除吗？", "确认", "取消"))
                    {
                        m_AssetBundleConfig.ListPattern.RemoveAt(removePatternIndex);
                    }
                }

                removePatternIndex = EditorGUILayout.Popup(removePatternIndex, m_AssetBundleConfig.ListPattern.ToArray());
                GUILayout.EndHorizontal();
            }
        }

        private void PlatFormSelectGUI()
        {
            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            GUILayout.Button("    打包平台    ");
            GUI.enabled = true;
            m_AssetBundleConfig.PlatFormIndex = EditorGUILayout.Popup(m_AssetBundleConfig.PlatFormIndex, TARGET_PLATFORM);
            GUILayout.EndHorizontal();
        }

        private void ConfigButtonGUI()
        {
            if (GUILayout.Button("添加配置"))
            {
                m_ListData.Add(new AssetBundleData());
                m_ListPatternIndex.Add(0);
                m_ListBundleExtendIndex.Add(0);
                m_ListDataHasRemove.Add(false);
            }

            if (GUILayout.Button("保存配置"))
            {
                SaveConfig();
                ShowNotification(new GUIContent("保存成功"));
            }
        }

        int platFormIndex = 0;
        private void BuildGUI()
        {
            if (GUILayout.Button("   打     包   "))
            {
                if (EditorUtility.DisplayDialog("提示", "确认开始打包吗？", "确认", "取消"))
                {
                    SaveConfig();

                    if (platFormIndex == 0)
                    {
                        Packager.Build(BuildTarget.StandaloneWindows);
                    }
                    else if (platFormIndex == 1)
                    {
                        Packager.Build(BuildTarget.Android);
                    }
                    else
                    {
                        BuildTarget target;
#if UNITY_5_3_OR_NEWER
                        target = BuildTarget.iOS;
#else
                        target = BuildTarget.iPhone;
#endif
                        Packager.Build(target);
                    }
                }
            }
        }

        private string[] TARGET_PLATFORM = new string[]
        {
            "PC",
            "Andriod",
            "iOS"
        };

        private List<int> m_ListBundleExtendIndex = null;
        private List<int> m_ListPatternIndex = null;
        private List<bool> m_ListDataHasRemove = null;
        private List<AssetBundleData> m_ListData = null;
        private AssetBundleConfig m_AssetBundleConfig = null;
    }
}