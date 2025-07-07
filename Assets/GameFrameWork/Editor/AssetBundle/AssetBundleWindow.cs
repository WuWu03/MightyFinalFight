using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class AssetBundleWindow : EditorWindow
    {
        public AssetBundleWindow()
        {
            titleContent = new GUIContent(this.GetType().Name);
            m_ListData = new List<AssetBundleData>();
            m_ListPatternIndex = new List<int>();
            m_ListBundleExtendIndex = new List<int>();
            m_ListDataHasRemove = new List<bool>();
            m_StackRemovedData = new Stack<int>();
        }

        private void OnDisable()
        {
            AssetBundleUtility.RefreshData();
        }

        private void OnDestroy()
        {
            if (IsConfigChanged())
            {
                if (EditorUtility.DisplayDialog("警告", "配置未保存，是否保存？", "保存", "取消"))
                {
                    SaveConfig();
                }
            }
        }

        private bool IsConfigChanged()
        {
            if (m_StackRemovedData.Count > 0)
            {
                return true;
            }

            if (m_ListData == null)
            {
                return false;
            }

            if (m_ListData.Count != m_AssetBundleConfig.listDatas.Count)
            {
                return true;
            }

            Func<AssetBundleData, AssetBundleData, bool> equals = (a, b) =>
            a.bundleBuildType == b.bundleBuildType &&
            string.Equals(a.bundleName, b.bundleName) &&
            string.Equals(a.bundleExtend, b.bundleExtend) &&
            string.Equals(a.pattern, b.pattern) &&
            string.Equals(a.assetPath, b.assetPath);

            for (int i = 0; i < m_ListData.Count; i++)
            {
                if (!equals(m_ListData[i], m_AssetBundleConfig.listDatas[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnGUI()
        {
            InitConfig();
            MainGUI();
            LockConfigGUI();
            CopyAssetGUI();
            ExtendNameGUI();
            PatternGUI();
            PlatFormSelectGUI();
            ConfigButtonGUI();
            BuildGUI();
        }

        private void InitConfig()
        {
            if (m_AssetBundleConfig != null)
            {
                return;
            }

            if (!Directory.Exists(EditorPathUtil.editorConfigFullPath))
            {
                Directory.CreateDirectory(EditorPathUtil.editorConfigFullPath);
            }

            if (!File.Exists(EditorPathUtil.assetBundleWindowDataFullPath))
            {
                EditorUtil.CreateConfigData<AssetBundleConfig, AssetBundleData>(EditorPathUtil.assetBundleWindowDataName, EditorPathUtil.assetBundleWindowDataExtend, EditorPathUtil.editorConfigPath);
            }

            m_AssetBundleConfig = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.assetBundleWindowDataPath);

            for (int i = 0; i < m_AssetBundleConfig.listDatas.Count; i++)
            {
                AssetBundleData data = m_AssetBundleConfig.listDatas[i].Clone();

                m_ListData.Add(data);

                for (int j = 0; j < m_AssetBundleConfig.listPattern.Count; j++)
                {
                    if(data.pattern.Equals(m_AssetBundleConfig.listPattern[j]))
                    {
                        m_ListPatternIndex.Add(j);
                    }
                }

                for (int j = 0; j < m_AssetBundleConfig.listExtendName.Count; j++)
                {
                    if (data.bundleExtend.Equals(m_AssetBundleConfig.listExtendName[j]))
                    {
                        m_ListBundleExtendIndex.Add(j);
                    }
                }

                if(m_ListPatternIndex.Count < m_ListData.Count)
                {
                    m_ListPatternIndex.Add(0);
                }

                if (m_ListBundleExtendIndex.Count < m_ListData.Count)
                {
                    m_ListBundleExtendIndex.Add(0);
                }
            }

            m_ListDataHasRemove.AddRange(new bool[m_ListData.Count]);
        }

        private void SaveConfig()
        {
            for (int i = m_ListData.Count - 1; i >= 0; i--)
            {
                if (m_ListDataHasRemove[i])
                {
                    m_ListData.RemoveAt(i);
                    m_ListBundleExtendIndex.RemoveAt(i);
                    m_ListPatternIndex.RemoveAt(i);
                }
            }

            m_AssetBundleConfig.listDatas = m_ListData;

            m_StackRemovedData.Clear();
            m_ListDataHasRemove.Clear();
            m_ListDataHasRemove.AddRange(new bool[m_ListData.Count]);
            EditorUtility.SetDirty(m_AssetBundleConfig);
            AssetBundleUtility.RefreshData();
        }

        private void SortConfig()
        {
            m_ListData.Sort();
            m_ListPatternIndex.Clear();
            m_ListBundleExtendIndex.Clear();

            for (int i = 0; i < m_ListData.Count; i++)
            {
                for (int j = 0; j < m_AssetBundleConfig.listPattern.Count; j++)
                {
                    if (m_ListData[i].pattern.Equals(m_AssetBundleConfig.listPattern[j]))
                    {
                        m_ListPatternIndex.Add(j);
                    }
                }

                for (int j = 0; j < m_AssetBundleConfig.listExtendName.Count; j++)
                {
                    if (m_ListData[i].bundleExtend.Equals(m_AssetBundleConfig.listExtendName[j]))
                    {
                        m_ListBundleExtendIndex.Add(j);
                    }
                }
            }
        }

        private void ClearConfig()
        {
            m_ListData.Clear();
            m_ListPatternIndex.Clear();
            m_ListDataHasRemove.Clear();
            m_ListBundleExtendIndex.Clear();
            m_AssetBundleConfig.listDatas = m_ListData;
        }

        Vector2 scrollPosition = Vector2.zero;
    
        private void MainGUI()
        {
            if (m_AssetBundleConfig.listExtendName == null)
            {
                m_AssetBundleConfig.listExtendName = new List<string>();
            }

            if (m_AssetBundleConfig.listPattern == null)
            {
                m_AssetBundleConfig.listPattern = new List<string>();
            }

            GUI.enabled = !m_AssetBundleConfig.lockConfig;
            EditorUtil.GUIBoxScope(() =>
            {
                GUILayout.BeginVertical();
                m_AssetBundleConfig.assetBuildDir = EditorGUILayout.TextField("资源打包路径（相对路径）", m_AssetBundleConfig.assetBuildDir);
 
                if (!string.IsNullOrEmpty(m_AssetBundleConfig.assetBuildDir) && !m_AssetBundleConfig.assetBuildDir.StartsWith("Assets/"))
                {
                    m_AssetBundleConfig.assetBuildDir = "Assets/" + m_AssetBundleConfig.assetBuildDir;
                }

                if (!string.IsNullOrEmpty(m_AssetBundleConfig.assetBuildDir) && !m_AssetBundleConfig.assetBuildDir.EndsWith("/"))
                {
                    m_AssetBundleConfig.assetBuildDir += "/";
                }

                if (m_AssetBundleConfig.isCopyAsset)
                {
                    m_AssetBundleConfig.assetCopyDir = EditorGUILayout.TextField("资源复制路径（绝对路径）", m_AssetBundleConfig.assetCopyDir);

                    if (!string.IsNullOrEmpty(m_AssetBundleConfig.assetCopyDir) && !m_AssetBundleConfig.assetCopyDir.EndsWith("\\"))
                    {
                        m_AssetBundleConfig.assetCopyDir += "\\";
                    }
                }
                GUILayout.EndVertical();
            });

            GUI.enabled = true;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUI.enabled = !m_AssetBundleConfig.lockConfig;
            int index = 0;
            for (int i = 0; i < m_ListData.Count; i++)
            {
                if (m_ListDataHasRemove.Count > 0 && m_ListDataHasRemove[i])
                {
                    continue;
                }

                index++;
                m_ListData[i].id = index;

                EditorUtil.GUIBoxScope((Action)(() => 
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.alignment = TextAnchor.MiddleLeft;
                    style.fontSize = 18;
                    style.fontStyle = FontStyle.Bold;
                    EditorGUILayout.LabelField(index.ToString(), style);
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("×"))//删除本条数据
                    {
                        if (EditorUtility.DisplayDialog("提示", "确认移除本条配置吗？", "确认", "取消"))
                        {
                            m_ListDataHasRemove[i] = true;
                            m_StackRemovedData.Push(i);
                        }
                    }
                    GUILayout.EndHorizontal();

                    m_ListData[i].bundleBuildType = (AssetBundleData.BundleBuildType)EditorGUILayout.EnumPopup("包类型：", (Enum)m_ListData[i].bundleBuildType);
                    m_ListData[i].assetPath = EditorGUILayout.TextField("资源路径：", m_ListData[i].assetPath);

                    if (!string.IsNullOrEmpty(m_ListData[i].assetPath) && !m_ListData[i].assetPath.EndsWith("/"))
                    {
                        m_ListData[i].assetPath += "/";
                    }

                    if (m_ListData[i].bundleBuildType == AssetBundleData.BundleBuildType.Mulity)
                    {
                        m_ListData[i].bundleName = string.Empty;
                        m_ListData[i].bundlePath = EditorGUILayout.TextField("包路径： ", m_ListData[i].bundlePath);

                        if (!string.IsNullOrEmpty(m_ListData[i].bundlePath))
                        {
                            if (!m_ListData[i].bundlePath.EndsWith("/"))
                            {
                                m_ListData[i].bundlePath += "/";
                            }

                            if (m_ListData[i].bundlePath.StartsWith("Assets"))
                            {
                                m_ListData[i].bundlePath = m_ListData[i].bundlePath.Replace("Assets", string.Empty);
                            }

                            if (m_ListData[i].bundlePath.StartsWith("/"))
                            {
                                m_ListData[i].bundlePath = m_ListData[i].bundlePath.Substring(1);
                            }
                        }
                    }
                    else
                    {
                        m_ListData[i].bundleName = EditorGUILayout.TextField("包名称: ", m_ListData[i].bundleName);
                        m_ListData[i].bundlePath = string.Empty;

                        if (!string.IsNullOrEmpty(m_ListData[i].bundleName))
                        {
                            if (m_ListData[i].bundleName.EndsWith("/"))
                            {
                                m_ListData[i].bundleName = m_ListData[i].bundleName.Substring(0, m_ListData[i].bundleName.Length - 1);
                            }

                            if (m_ListData[i].bundleName.StartsWith("Assets"))
                            {
                                m_ListData[i].bundleName = m_ListData[i].bundleName.Replace("Assets", string.Empty);
                            }

                            if (m_ListData[i].bundleName.StartsWith("/"))
                            {
                                m_ListData[i].bundleName = m_ListData[i].bundleName.Substring(1);
                            }
                        }
                    }

                    if (m_AssetBundleConfig.listExtendName.Count > 0)
                    {
                        m_ListBundleExtendIndex[i] = EditorGUILayout.Popup("包扩展名：", m_ListBundleExtendIndex[i], m_AssetBundleConfig.listExtendName.ToArray());
                        m_ListData[i].bundleExtend = m_AssetBundleConfig.listExtendName[m_ListBundleExtendIndex[i]];
                    }

                    if (m_AssetBundleConfig.listPattern.Count > 0)
                    {
                        m_ListPatternIndex[i] = EditorGUILayout.Popup("文件过滤：", m_ListPatternIndex[i], m_AssetBundleConfig.listPattern.ToArray());
                        m_ListData[i].pattern = m_AssetBundleConfig.listPattern[m_ListPatternIndex[i]];
                    }

                    GUI.enabled = true;

                    if (GUILayout.Button("选中该资源/路径"))
                    {
                        string assetPath = m_ListData[i].assetPath.Substring(0, m_ListData[i].assetPath.LastIndexOf("/"));
                        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
                        if (obj != null)
                        {
                            EditorGUIUtility.PingObject(obj);
                            Selection.activeObject = obj;
                        }
                    }

                    GUI.enabled = !m_AssetBundleConfig.lockConfig;
                    GUILayout.EndVertical();
                }));
            }

            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUI.enabled = true;
        }

        private void LockConfigGUI()
        {
            EditorUtil.GUIBoxScope(() =>
            {
                m_AssetBundleConfig.lockConfig = GUILayout.Toggle(m_AssetBundleConfig.lockConfig, "锁定所有配置", GUI.skin.toggle);
            });
        }

        private void CopyAssetGUI()
        {
            GUI.enabled = !m_AssetBundleConfig.lockConfig;
            EditorUtil.GUIBoxScope(() =>
            {
                m_AssetBundleConfig.isCopyAsset = GUILayout.Toggle(m_AssetBundleConfig.isCopyAsset, "打包完成后自动复制资源到指定文件夹", GUI.skin.toggle);
            });
            GUI.enabled = true;
        }

        string extend = string.Empty;
        int removeExtendIndex = 0;
        private void ExtendNameGUI()
        {
            //--------------添加扩展名----------------
            GUI.enabled = !m_AssetBundleConfig.lockConfig;
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

                    if (!m_AssetBundleConfig.listExtendName.Contains(extend))
                    {
                        m_AssetBundleConfig.listExtendName.Add(extend);
                        ShowNotification(new GUIContent("添加成功"));
                    }
                    else
                    {
                        ShowNotification(new GUIContent("已存在相同的扩展名"));
                    }
                }
            }
            extend = EditorGUILayout.TextField(extend);

            //--------------移除扩展名----------------
            if (GUILayout.Button("  移除扩展名  "))
            {
                if(m_AssetBundleConfig.listExtendName.Count < 1)
                {
                    ShowNotification(new GUIContent("没有扩展名，请先添加扩展名"));
                    return;
                }
                if (EditorUtility.DisplayDialog("提示", "确认移除吗？", "确认", "取消"))
                {
                    m_AssetBundleConfig.listExtendName.RemoveAt(removeExtendIndex);
                }
            }
            removeExtendIndex = EditorGUILayout.Popup(removeExtendIndex, m_AssetBundleConfig.listExtendName.ToArray());
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        string pattern = string.Empty;
        int removePatternIndex = 0;
        private void PatternGUI()
        {
            //--------------添加过滤器----------------
            GUI.enabled = !m_AssetBundleConfig.lockConfig;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("添加过滤器名"))
            {
                if (string.IsNullOrEmpty(pattern))
                {
                    pattern = "*";
                }
                else
                {
                    if (pattern.Contains("."))
                    {
                        pattern = "*" + pattern.Substring(pattern.LastIndexOf("."));
                    }

                    if (!pattern.StartsWith("*."))
                    {
                        pattern = "*." + pattern;
                    }
                }

                if (!m_AssetBundleConfig.listPattern.Contains(pattern))
                {
                    m_AssetBundleConfig.listPattern.Add(pattern);
                    ShowNotification(new GUIContent("添加成功"));
                }
                else
                {
                    ShowNotification(new GUIContent("已存在相同的过滤器名"));
                }
            }
            pattern = EditorGUILayout.TextField(pattern);

            //--------------移除过滤器----------------
            if (GUILayout.Button("移除过滤器名"))
            {
                if(m_AssetBundleConfig.listPattern.Count < 1)
                {
                    ShowNotification(new GUIContent("没有过滤器名，请先添加过滤器名"));
                    return;
                }

                if (EditorUtility.DisplayDialog("提示", "确认移除吗？", "确认", "取消"))
                {
                    m_AssetBundleConfig.listPattern.RemoveAt(removePatternIndex);
                }
            }

            removePatternIndex = EditorGUILayout.Popup(removePatternIndex, m_AssetBundleConfig.listPattern.ToArray());
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        private void PlatFormSelectGUI()
        {
            GUI.enabled = !m_AssetBundleConfig.lockConfig;
            GUILayout.BeginHorizontal();
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            style.fixedWidth = 346;
            GUILayout.Label("打包平台", style);
            m_AssetBundleConfig.platFormIndex = EditorGUILayout.Popup(m_AssetBundleConfig.platFormIndex, TARGET_PLATFORM);
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        private void ConfigButtonGUI()
        {
            GUI.enabled = !m_AssetBundleConfig.lockConfig;
            if (GUILayout.Button("添加配置"))
            {
                m_ListData.Add(new AssetBundleData());
                m_ListPatternIndex.Add(0);
                m_ListBundleExtendIndex.Add(0);
                m_ListDataHasRemove.Add(false);
            }

            if (GUILayout.Button("清空配置"))
            {
                if (EditorUtility.DisplayDialog("提示", "确认清空全部配置吗？", "确认", "取消"))
                {
                    ClearConfig();
                }
            }

            if (m_StackRemovedData.Count> 0)
            {
                if (GUILayout.Button("还原已经删除配置"))
                {
                    if (EditorUtility.DisplayDialog("提示", "确认还原吗？", "确认", "取消"))
                    {
                        int dataIndex = m_StackRemovedData.Pop();
                        m_ListDataHasRemove[dataIndex] = false;
                    }
                }
            }

            if (GUILayout.Button("保存配置"))
            {
                SortConfig();
                SaveConfig();
                ShowNotification(new GUIContent("保存成功"));
            }

            //if (GUILayout.Button("重新排序"))
            //{
            //    SortConfig();
            //    SaveConfig();
            //    ShowNotification(new GUIContent("排序成功"));
            //}
            GUI.enabled = true;
        }

        int platFormIndex = 0;
        private void BuildGUI()
        {
            if (GUILayout.Button("打        包"))
            {
                if (EditorUtility.DisplayDialog("提示", "确认开始打包吗？", "确认", "取消"))
                {
                    SaveConfig();

                    if (platFormIndex == 0)
                    {
                        using(AssetBundleBuilder builder = new AssetBundleBuilder())
                        {
                            builder.Build(BuildTarget.StandaloneWindows);
                        }
                    }
                    else if (platFormIndex == 1)
                    {
                        using (AssetBundleBuilder builder = new AssetBundleBuilder())
                        {
                            builder.Build(BuildTarget.Android);
                        }
                    }
                    else
                    {
                        BuildTarget target;
#if UNITY_5_3_OR_NEWER
                        target = BuildTarget.iOS;
#else
                        target = BuildTarget.iPhone;
#endif
                        using (AssetBundleBuilder builder = new AssetBundleBuilder())
                        {
                            builder.Build(target);
                        }
                    }
                }
            }
        }


        private readonly string[] TARGET_PLATFORM = new string[3]
        {
            "PC",
            "Andriod",
            "iOS"
        };

        private Stack<int> m_StackRemovedData = null;
        private List<int> m_ListBundleExtendIndex = null;
        private List<int> m_ListPatternIndex = null;
        private List<bool> m_ListDataHasRemove = null;
        private List<AssetBundleData> m_ListData = null;
        private AssetBundleConfig m_AssetBundleConfig = null;
    }
}