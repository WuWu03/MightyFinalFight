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
        }

        private void OnDisable()
        {
            AssetBundleUtility.RefreshData();
            //SaveConfig();
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
            if(m_ListData == null)
            {
                return false;
            }

            if(m_ListData.Count != m_AssetBundleConfig.Datas.Count)
            {
                return true;
            }

            Func<AssetBundleData, AssetBundleData, bool> equals = (a, b) =>
            a.BundleType == b.BundleType &&
            string.Equals(a.BundleName, b.BundleName) &&
            string.Equals(a.BundleExtend, b.BundleExtend) &&
            string.Equals(a.Pattern, b.Pattern) &&
            string.Equals(a.AssetPath, b.AssetPath);

            for (int i = 0; i < m_ListData.Count; i++)
            {
                if (!equals(m_ListData[i], m_AssetBundleConfig.Datas[i]))
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

            if (!Directory.Exists(EditorPathUtil.assetBundleConfigFullPath))
            {
                Directory.CreateDirectory(EditorPathUtil.assetBundleConfigFullPath);
            }

            if (!File.Exists(EditorPathUtil.assetBundleDataFullPath))
            {
                EditorUtil.CreateConfigData<AssetBundleConfig, AssetBundleData>(EditorPathUtil.assetBundleDataName, EditorPathUtil.assetBundleDataExtend, EditorPathUtil.ediorConfigPath);
            }

            m_AssetBundleConfig = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.assetBundleDataPath);

            for (int i = 0; i < m_AssetBundleConfig.Datas.Count; i++)
            {
                AssetBundleData data = m_AssetBundleConfig.Datas[i].Clone();

                m_ListData.Add(data);

                for (int j = 0; j < m_AssetBundleConfig.ListPattern.Count; j++)
                {
                    if(data.Pattern.Equals(m_AssetBundleConfig.ListPattern[j]))
                    {
                        m_ListPatternIndex.Add(j);
                    }
                }

                for (int j = 0; j < m_AssetBundleConfig.ListExtendName.Count; j++)
                {
                    if (data.BundleExtend.Equals(m_AssetBundleConfig.ListExtendName[j]))
                    {
                        m_ListBundleExtendIndex.Add(j);
                    }
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

            m_AssetBundleConfig.Datas = m_ListData;
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
                for (int j = 0; j < m_AssetBundleConfig.ListPattern.Count; j++)
                {
                    if (m_ListData[i].Pattern.Equals(m_AssetBundleConfig.ListPattern[j]))
                    {
                        m_ListPatternIndex.Add(j);
                    }
                }

                for (int j = 0; j < m_AssetBundleConfig.ListExtendName.Count; j++)
                {
                    if (m_ListData[i].BundleExtend.Equals(m_AssetBundleConfig.ListExtendName[j]))
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
            m_AssetBundleConfig.Datas = m_ListData;
        }

        Vector2 scrollPosition = Vector2.zero;
    
        private void MainGUI()
        {
            if (m_AssetBundleConfig.ListExtendName == null)
            {
                m_AssetBundleConfig.ListExtendName = new List<string>();
            }

            if (m_AssetBundleConfig.ListPattern == null)
            {
                m_AssetBundleConfig.ListPattern = new List<string>();
            }

            GUI.enabled = !m_AssetBundleConfig.LockConfig;
            EditorUtil.GUIBoxScope(() =>
            {
                GUILayout.BeginVertical();
                m_AssetBundleConfig.AssetBuildDir = EditorGUILayout.TextField("资源打包路径（相对路径）", m_AssetBundleConfig.AssetBuildDir);
 
                if (!string.IsNullOrEmpty(m_AssetBundleConfig.AssetBuildDir) && !m_AssetBundleConfig.AssetBuildDir.StartsWith("Assets/"))
                {
                    m_AssetBundleConfig.AssetBuildDir = "Assets/" + m_AssetBundleConfig.AssetBuildDir;
                }

                if (!string.IsNullOrEmpty(m_AssetBundleConfig.AssetBuildDir) && !m_AssetBundleConfig.AssetBuildDir.EndsWith("/"))
                {
                    m_AssetBundleConfig.AssetBuildDir += "/";
                }

                if (m_AssetBundleConfig.IsCopyAsset)
                {
                    m_AssetBundleConfig.AssetCopyDir = EditorGUILayout.TextField("资源复制路径（绝对路径）", m_AssetBundleConfig.AssetCopyDir);

                    if (!string.IsNullOrEmpty(m_AssetBundleConfig.AssetCopyDir) && !m_AssetBundleConfig.AssetCopyDir.EndsWith("\\"))
                    {
                        m_AssetBundleConfig.AssetCopyDir += "\\";
                    }
                }
                GUILayout.EndVertical();
            });

            GUI.enabled = true;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUI.enabled = !m_AssetBundleConfig.LockConfig;
            int index = 0;
            for (int i = 0; i < m_ListData.Count; i++)
            {
                if (m_ListDataHasRemove.Count > 0 && m_ListDataHasRemove[i])
                {
                    continue;
                }

                index++;
                m_ListData[i].Id = index;

                EditorUtil.GUIBoxScope(() => 
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
                        }
                    }
                    GUILayout.EndHorizontal();

                    m_ListData[i].BundleType = (AssetBundleData.AssetType)EditorGUILayout.EnumPopup("包类型：", m_ListData[i].BundleType);
                    m_ListData[i].AssetPath = EditorGUILayout.TextField("资源路径：", m_ListData[i].AssetPath);

                    if (!string.IsNullOrEmpty(m_ListData[i].AssetPath) && !m_ListData[i].AssetPath.EndsWith("/"))
                    {
                        m_ListData[i].AssetPath += "/";
                    }

                    if (m_ListData[i].BundleType == AssetBundleData.AssetType.MapSingle)
                    {
                        m_ListData[i].AssetBundlePath = EditorGUILayout.TextField("包路径： ", m_ListData[i].AssetBundlePath);

                        if (!string.IsNullOrEmpty(m_ListData[i].AssetBundlePath) && !m_ListData[i].AssetBundlePath.EndsWith("/"))
                        {
                            m_ListData[i].AssetBundlePath += "/";
                        }
                    }
                    else
                    {
                        m_ListData[i].BundleName = EditorGUILayout.TextField("包名称: ", m_ListData[i].BundleName);

                        if (!string.IsNullOrEmpty(m_ListData[i].BundleName) && m_ListData[i].BundleName.EndsWith("/"))
                        {
                            m_ListData[i].BundleName = m_ListData[i].BundleName.Substring(0, m_ListData[i].BundleName.Length - 1);
                        }
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

                    GUI.enabled = true;

                    if (GUILayout.Button("选中该资源/路径"))
                    {
                        string assetPath = m_ListData[i].AssetPath.Substring(0, m_ListData[i].AssetPath.LastIndexOf("/"));
                        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
                        if (obj != null)
                        {
                            EditorGUIUtility.PingObject(obj);
                            Selection.activeObject = obj;
                        }
                    }

                    GUI.enabled = !m_AssetBundleConfig.LockConfig;
                    GUILayout.EndVertical();
                });
            }

            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUI.enabled = true;
        }

        private void LockConfigGUI()
        {
            EditorUtil.GUIBoxScope(() =>
            {
                m_AssetBundleConfig.LockConfig = GUILayout.Toggle(m_AssetBundleConfig.LockConfig, "锁定所有配置", GUI.skin.toggle);
            });
        }

        private void CopyAssetGUI()
        {
            GUI.enabled = !m_AssetBundleConfig.LockConfig;
            EditorUtil.GUIBoxScope(() =>
            {
                m_AssetBundleConfig.IsCopyAsset = GUILayout.Toggle(m_AssetBundleConfig.IsCopyAsset, "打包完成后自动复制资源到指定文件夹", GUI.skin.toggle);
            });
            GUI.enabled = true;
        }

        string extend = string.Empty;
        int removeExtendIndex = 0;
        private void ExtendNameGUI()
        {
            //--------------添加扩展名----------------
            GUI.enabled = !m_AssetBundleConfig.LockConfig;
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

            //--------------移除扩展名----------------
            if (GUILayout.Button("  移除扩展名  "))
            {
                if(m_AssetBundleConfig.ListExtendName.Count < 1)
                {
                    ShowNotification(new GUIContent("没有扩展名，请先添加扩展名"));
                    return;
                }
                if (EditorUtility.DisplayDialog("提示", "确认移除吗？", "确认", "取消"))
                {
                    m_AssetBundleConfig.ListExtendName.RemoveAt(removeExtendIndex);
                }
            }
            removeExtendIndex = EditorGUILayout.Popup(removeExtendIndex, m_AssetBundleConfig.ListExtendName.ToArray());
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        string pattern = string.Empty;
        int removePatternIndex = 0;
        private void PatternGUI()
        {
            //--------------添加过滤器----------------
            GUI.enabled = !m_AssetBundleConfig.LockConfig;
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
            pattern = EditorGUILayout.TextField(pattern);

            //--------------移除过滤器----------------
            if (GUILayout.Button("移除过滤器名"))
            {
                if(m_AssetBundleConfig.ListPattern.Count < 1)
                {
                    ShowNotification(new GUIContent("没有过滤器名，请先添加过滤器名"));
                    return;
                }

                if (EditorUtility.DisplayDialog("提示", "确认移除吗？", "确认", "取消"))
                {
                    m_AssetBundleConfig.ListPattern.RemoveAt(removePatternIndex);
                }
            }

            removePatternIndex = EditorGUILayout.Popup(removePatternIndex, m_AssetBundleConfig.ListPattern.ToArray());
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        private void PlatFormSelectGUI()
        {
            GUI.enabled = !m_AssetBundleConfig.LockConfig;
            GUILayout.BeginHorizontal();
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            style.fixedWidth = 346;
            GUILayout.Label("打包平台", style);
            m_AssetBundleConfig.PlatFormIndex = EditorGUILayout.Popup(m_AssetBundleConfig.PlatFormIndex, TARGET_PLATFORM);
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        private void ConfigButtonGUI()
        {
            GUI.enabled = !m_AssetBundleConfig.LockConfig;
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

        private List<int> m_ListBundleExtendIndex = null;
        private List<int> m_ListPatternIndex = null;
        private List<bool> m_ListDataHasRemove = null;
        private List<AssetBundleData> m_ListData = null;
        private AssetBundleConfig m_AssetBundleConfig = null;
    }
}