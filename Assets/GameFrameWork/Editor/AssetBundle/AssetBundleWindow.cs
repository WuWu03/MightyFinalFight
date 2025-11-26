using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class AssetBundleWindow : EditorWindow
    {
        public AssetBundleWindow()
        {
            titleContent = new(this.GetType().Name);
            m_AssetBundleDatas = new();
            m_BundlePatternIndexs = new();
            m_BundleExtendIndexs = new();
            m_RemoveDatas = new();
            m_RemovedAssetPaths = new();
        }

        private void OnEnable()
        {

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

            if (!Directory.Exists(EditorPathUtil.EditorConfigFullPath))
            {
                Directory.CreateDirectory(EditorPathUtil.EditorConfigFullPath);
            }

            if (!File.Exists(EditorPathUtil.AassetBundleWindowDataFullPath))
            {
                EditorUtil.CreateConfigData<AssetBundleConfig, AssetBundleData>(EditorPathUtil.AssetBundleWindowDataName, EditorPathUtil.AssetBundleWindowDataExtend, EditorPathUtil.EditorConfigPath);
            }

            m_AssetBundleConfig = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.AssetBundleWindowDataPath);

            for (int i = 0; i < m_AssetBundleConfig.listDatas.Count; i++)
            {
                AssetBundleData data = m_AssetBundleConfig.listDatas[i].Clone();
                m_AssetBundleDatas.Add(data);

                for (int j = 0; j < m_AssetBundleConfig.listPattern.Count; j++)
                {
                    if (data.pattern.Equals(m_AssetBundleConfig.listPattern[j]))
                    {
                        m_BundlePatternIndexs.Add(j);
                    }
                }

                for (int j = 0; j < m_AssetBundleConfig.listExtendName.Count; j++)
                {
                    if (data.bundleExtend.Equals(m_AssetBundleConfig.listExtendName[j]))
                    {
                        m_BundleExtendIndexs.Add(j);
                    }
                }

                if (m_BundlePatternIndexs.Count < m_AssetBundleDatas.Count)
                {
                    m_BundlePatternIndexs.Add(0);
                }

                if (m_BundleExtendIndexs.Count < m_AssetBundleDatas.Count)
                {
                    m_BundleExtendIndexs.Add(0);
                }
            }

            Array buildTargetArray = Enum.GetValues(typeof(BuildTarget));
            m_BuildTargetDisplayNames = new string[buildTargetArray.Length];
            m_BuildTargets = new BuildTarget[buildTargetArray.Length];

            for (int i = 0; i < buildTargetArray.Length; i++)
            {
                m_BuildTargetDisplayNames[i] = buildTargetArray.GetValue(i).ToString();
                m_BuildTargets[i] = (BuildTarget)buildTargetArray.GetValue(i);

                if (m_BuildTargets[i] == EditorUserBuildSettings.activeBuildTarget)
                {
                    m_AssetBundleConfig.platFormIndex = i;
                }
            }
        }

        private void SaveConfig()
        {
            for (int i = m_AssetBundleDatas.Count - 1; i >= 0; i--)
            {
                if (m_RemoveDatas.Contains(m_AssetBundleDatas[i]))
                {
                    m_AssetBundleDatas.RemoveAt(i);
                    m_BundleExtendIndexs.RemoveAt(i);
                    m_BundlePatternIndexs.RemoveAt(i);
                }
            }

            m_AssetBundleConfig.listDatas.Clear();

            for (int i = 0; i < m_AssetBundleDatas.Count; i++)
            {
                m_AssetBundleConfig.listDatas.Add(m_AssetBundleDatas[i].Clone());
            }

            m_RemoveDatas.Clear();
            EditorUtility.SetDirty(m_AssetBundleConfig);
            AssetBundleUtility.RefreshData();
        }

        private void SortConfig()
        {
            m_AssetBundleDatas.Sort();
            foreach (AssetBundleData data in m_AssetBundleDatas) 
            {
                data.assetPaths.Sort();
            }

            m_BundlePatternIndexs.Clear();
            m_BundleExtendIndexs.Clear();

            for (int i = 0; i < m_AssetBundleDatas.Count; i++)
            {
                for (int j = 0; j < m_AssetBundleConfig.listPattern.Count; j++)
                {
                    if (m_AssetBundleDatas[i].pattern.Equals(m_AssetBundleConfig.listPattern[j]))
                    {
                        m_BundlePatternIndexs.Add(j);
                    }
                }

                for (int j = 0; j < m_AssetBundleConfig.listExtendName.Count; j++)
                {
                    if (m_AssetBundleDatas[i].bundleExtend.Equals(m_AssetBundleConfig.listExtendName[j]))
                    {
                        m_BundleExtendIndexs.Add(j);
                    }
                }
            }
        }

        private bool IsConfigChanged()
        {
            if (m_RemoveDatas.Count > 0)
            {
                return true;
            }

            if (m_AssetBundleDatas == null)
            {
                return false;
            }

            if (m_AssetBundleDatas.Count != m_AssetBundleConfig.listDatas.Count)
            {
                return true;
            }

            Func<AssetBundleData, AssetBundleData, bool> equals = (a, b) =>
            a.bundleBuildType == b.bundleBuildType &&
            a.bundleName == b.bundleName &&
            a.pattern == b.pattern &&
            a.assetPaths.SequenceEqual(b.assetPaths);

            for (int i = 0; i < m_AssetBundleDatas.Count; i++)
            {
                if (!equals(m_AssetBundleDatas[i], m_AssetBundleConfig.listDatas[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private void ClearConfig()
        {
            m_AssetBundleDatas.Clear();
            m_BundlePatternIndexs.Clear();
            m_RemoveDatas.Clear();
            m_BundleExtendIndexs.Clear();
            m_AssetBundleConfig.listDatas = m_AssetBundleDatas;
        }

        private int HasSameAsset(UnityEngine.Object asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            for (int i = 0; i < m_AssetBundleDatas.Count; i++)
            {
                if (m_RemoveDatas.Contains(m_AssetBundleDatas[i]))
                {
                    continue;
                }

                AssetBundleData assetBundleData = m_AssetBundleDatas[i];
                if (assetBundleData.assetPaths != null && assetBundleData.assetPaths.Contains(assetPath))
                {
                    return i;
                }

                for (int j = 0; j < assetBundleData.assetPaths.Count; j++)
                {
                    bool isFile = File.Exists(assetBundleData.assetPaths[j]);
                    if (!isFile)
                    {
                        List<string> files = new();
                        List<string> paths = new();
                        GameFrameWork.Utils.FileUtil.Recursive(assetBundleData.assetPaths[j], "*", files, paths);

                        if (files.Contains(assetPath) || paths.Contains(assetPath))
                        {
                            return i;
                        }
                    }
                }
            }

            return -1;
        }

        Vector2 scrollPosition = Vector2.zero;

        private void MainGUI()
        {
            m_AssetBundleConfig.listExtendName ??= new();
            m_AssetBundleConfig.listPattern ??= new();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUI.enabled = !m_AssetBundleConfig.lockConfig;
            int index = 0;
            for (int i = 0; i < m_AssetBundleDatas.Count; i++)
            {
                AssetBundleData assetBundleData = m_AssetBundleDatas[i];
                assetBundleData.assetPaths ??= new();

                if (m_RemoveDatas.Count > 0 && m_RemoveDatas.Contains(assetBundleData))
                {
                    continue;
                }

                index++;
                assetBundleData.id = index;

                EditorUtil.GUIBoxScope((Action)(() =>
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUIStyle style = new(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        fontSize = 18,
                        fontStyle = FontStyle.Bold
                    };
                    EditorGUILayout.LabelField(index.ToString(), style);
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("×"))//删除本条数据
                    {
                        if (EditorUtility.DisplayDialog("提示", "确认移除本条配置吗？", "确认", "取消"))
                        {
                            m_RemoveDatas.Add(assetBundleData);
                        }
                    }
                    GUILayout.EndHorizontal();
                    assetBundleData.bundleBuildType = (AssetBundleData.BundleBuildType)EditorGUILayout.EnumPopup("打包方式：", (Enum)assetBundleData.bundleBuildType);

                    if (assetBundleData.bundleBuildType == AssetBundleData.BundleBuildType.Single)
                    {
                        assetBundleData.bundleName = EditorGUILayout.TextField("包名称: ", assetBundleData.bundleName);

                        if (!string.IsNullOrEmpty(assetBundleData.bundleName))
                        {
                            if (assetBundleData.bundleName.StartsWith("Assets/"))
                            {
                                assetBundleData.bundleName = assetBundleData.bundleName.Replace("Assets/", string.Empty);
                            }
                        }
                    }
                    else
                    {
                        assetBundleData.bundleName = string.Empty;
                    }

                    if (m_AssetBundleConfig.listExtendName.Count > 0)
                    {
                        m_BundleExtendIndexs[i] = EditorGUILayout.Popup("包扩展名：", m_BundleExtendIndexs[i], m_AssetBundleConfig.listExtendName.ToArray());
                        assetBundleData.bundleExtend = m_AssetBundleConfig.listExtendName[m_BundleExtendIndexs[i]];
                    }

                    if (m_AssetBundleConfig.listPattern.Count > 0)
                    {
                        m_BundlePatternIndexs[i] = EditorGUILayout.Popup("文件过滤：", m_BundlePatternIndexs[i], m_AssetBundleConfig.listPattern.ToArray());
                        m_AssetBundleDatas[i].pattern = m_AssetBundleConfig.listPattern[m_BundlePatternIndexs[i]];
                    }

                    for (int j = 0; j < assetBundleData.assetPaths.Count; j++)
                    {
                        string assetPath = assetBundleData.assetPaths[j];
                        EditorGUILayout.BeginHorizontal();
                        UnityEngine.Object currAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                        UnityEngine.Object temp = EditorGUILayout.ObjectField("资源" + (j + 1), currAsset, typeof(UnityEngine.Object), false);

                        if (temp != currAsset)
                        {
                            int sameIndex = HasSameAsset(temp);
                            if (sameIndex > -1)
                            {
                                EditorUtility.DisplayDialog("警告", "该资源已经存在于包体" + (sameIndex + 1) + "中", "确定");
                            }
                            else
                            {
                                assetBundleData.assetPaths[j] = AssetDatabase.GetAssetPath(temp);
                                AssetBundleUtility.RefreshData();
                            }
                        }

                        if (GUILayout.Button("×", GUILayout.Width(20)))
                        {
                            if (EditorUtility.DisplayDialog("提示", "确认移除本条配置吗？", "确认", "取消"))
                            {
                                m_RemovedAssetPaths.Add(assetPath);
                            }
                        }

                        GUILayout.EndHorizontal();
                    }

                    if (GUILayout.Button("添加资源"))
                    {
                        assetBundleData.assetPaths.Add(string.Empty);
                    }

                    if (m_RemovedAssetPaths.Count > 0)
                    {
                        foreach (string removedAssetPath in m_RemovedAssetPaths) 
                        {
                            assetBundleData.assetPaths.Remove(removedAssetPath);
                        }

                        m_RemovedAssetPaths.Clear();
                    }

                    GUILayout.EndVertical();
                }));
            }

            GUI.enabled = true;
            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
        }

        private void LockConfigGUI()
        {
            EditorUtil.GUIBoxScope(() =>
            {
                m_AssetBundleConfig.lockConfig = GUILayout.Toggle(m_AssetBundleConfig.lockConfig, "锁定所有配置", GUI.skin.toggle);
                if(m_AssetBundleConfig.lockConfig && IsConfigChanged())
                {
                    m_AssetBundleConfig.lockConfig = EditorUtility.DisplayDialog("警告", "配置未保存，是否保存？", "保存", "取消");
                    if (m_AssetBundleConfig.lockConfig)
                    {
                        SaveConfig();
                    }
                }
            });
        }

        private void CopyAssetGUI()
        {
            GUI.enabled = !m_AssetBundleConfig.lockConfig;
            EditorUtil.GUIBoxScope(() =>
            {
                m_AssetBundleConfig.isCopyAsset = GUILayout.Toggle(m_AssetBundleConfig.isCopyAsset, "打包完成后自动复制资源到指定文件夹", GUI.skin.toggle);
            });

            if (m_AssetBundleConfig.isCopyAsset)
            {
                EditorUtil.GUIBoxScope(() =>
                {
                    m_AssetBundleConfig.assetCopyDir = EditorGUILayout.TextField("资源复制路径（绝对路径）", m_AssetBundleConfig.assetCopyDir);
                });

                if (!string.IsNullOrEmpty(m_AssetBundleConfig.assetCopyDir) && !m_AssetBundleConfig.assetCopyDir.EndsWith("\\"))
                {
                    m_AssetBundleConfig.assetCopyDir += "\\";
                }
            }

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
                if (m_AssetBundleConfig.listExtendName.Count < 1)
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
                if (m_AssetBundleConfig.listPattern.Count < 1)
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
            GUIStyle style = new(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                fixedWidth = 346
            };
            GUILayout.Label("打包平台", style);
            m_AssetBundleConfig.platFormIndex = EditorGUILayout.Popup(m_AssetBundleConfig.platFormIndex, m_BuildTargetDisplayNames);
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        private void ConfigButtonGUI()
        {
            GUI.enabled = !m_AssetBundleConfig.lockConfig;
            if (GUILayout.Button("添加配置"))
            {
                m_AssetBundleDatas.Add(new AssetBundleData());
                m_BundlePatternIndexs.Add(0);
                m_BundleExtendIndexs.Add(0);
            }

            if (GUILayout.Button("清空配置"))
            {
                if (EditorUtility.DisplayDialog("提示", "确认清空全部配置吗？", "确认", "取消"))
                {
                    ClearConfig();
                }
            }

            if (m_RemoveDatas.Count > 0)
            {
                if (GUILayout.Button("还原已经删除配置"))
                {
                    if (EditorUtility.DisplayDialog("提示", "确认还原吗？", "确认", "取消"))
                    {
                        m_RemoveDatas.Clear();
                    }
                }
            }

            if (GUILayout.Button("保存配置"))
            {
                SortConfig();
                SaveConfig();
                ShowNotification(new GUIContent("保存成功"));
            }

            GUI.enabled = true;
        }

        private void BuildGUI()
        {
            if (GUILayout.Button("打        包"))
            {
                if (EditorUtility.DisplayDialog("提示", "确认开始打包吗？", "确认", "取消"))
                {
                    SaveConfig();
                    using AssetBundleBuilder builder = new();
                    builder.Build(m_BuildTargets[m_AssetBundleConfig.platFormIndex]);
                }
            }
        }

        private string[] m_BuildTargetDisplayNames = null;
        private BuildTarget[] m_BuildTargets = null;
        private List<int> m_BundleExtendIndexs = null;
        private List<int> m_BundlePatternIndexs = null;
        private List<AssetBundleData> m_RemoveDatas = null;
        private List<string> m_RemovedAssetPaths = null;
        private List<AssetBundleData> m_AssetBundleDatas = null;
        private AssetBundleConfig m_AssetBundleConfig = null;
    }
}