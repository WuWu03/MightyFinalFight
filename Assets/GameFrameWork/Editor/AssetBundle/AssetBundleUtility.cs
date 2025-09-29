using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [InitializeOnLoad]
    public static class AssetBundleUtility
    {
        enum AssetFindResult
        {
            Failure = 1,//失败
            Success = 2,//成功
            InSubPath = 3,//子目录有资源
        }

        static AssetBundleUtility()
        {
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemGUI;
            m_AssetBundleConfig = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.assetBundleWindowDataPath);
            m_DicAssetContainer = new Dictionary<string, string>();
        }

        public static void RefreshData()
        {
            m_AssetBundleConfig = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.assetBundleWindowDataPath);
            m_DicAssetContainer.Clear();
        }

        private static void ProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(assetPath) || !assetPath.Contains("Assets"))
            {
                return;
            }

            if (string.IsNullOrEmpty(Path.GetExtension(assetPath)) && !assetPath.EndsWith("/"))
            {
                assetPath = assetPath + "/";
            }

            AssetFindResult result = GetAssetBuildMapIndex(assetPath, out string assetIndex);

            if (result == AssetFindResult.Failure)
            {
                return;
            }

            GUIStyle labelStyle = new("AssetLabel")
            {
                alignment = TextAnchor.MiddleCenter
            };
            labelStyle.normal.textColor = Color.green;
            labelStyle.focused.textColor = Color.green;
            float width = 40f;
            float height = selectionRect.height;
            float x = selectionRect.x + selectionRect.width - width;
            float y = selectionRect.y;
     
            if (result == AssetFindResult.Success)
            {
                GUI.Label(new Rect(x, y, width, height), assetIndex, labelStyle);
            }
            else if (result == AssetFindResult.InSubPath)
            {
                GUI.Label(new Rect(x, y, width, height), "*", labelStyle);
            }
        }

        private static AssetFindResult GetAssetBuildMapIndex(string assetPath, out string assetIndex)
        {
            if (m_AssetBundleConfig == null)
            {
                assetIndex = string.Empty;
                return AssetFindResult.Failure;
            }

            if (!m_DicAssetContainer.TryGetValue(assetPath, out string result))
            {
                for (int i = 0; i < m_AssetBundleConfig.listDatas.Count; i++)
                {
                    if (m_AssetBundleConfig.listDatas[i].assetPaths == null || m_AssetBundleConfig.listDatas[i].assetPaths.Count < 1)
                    {
                        continue;
                    }

                    int index = m_AssetBundleConfig.listDatas[i].assetPaths.IndexOf(assetPath);
                    if (index >= 0)
                    {
                        assetIndex = (i + 1).ToString() + "_" + (index + 1).ToString();
                        m_DicAssetContainer.Add(assetPath, assetIndex);
                        return AssetFindResult.Success;
                    }
                    else
                    {
                        for (int j = 0; j < m_AssetBundleConfig.listDatas[i].assetPaths.Count; j++)
                        {
                            string tempAsetPath = m_AssetBundleConfig.listDatas[i].assetPaths[j];

                            if (assetPath.StartsWith(tempAsetPath))
                            {
                                assetIndex = (i + 1).ToString() + "_" + (j + 1).ToString();
                                m_DicAssetContainer.Add(assetPath, assetIndex);

                                return AssetFindResult.Success;
                            }
                            else if (tempAsetPath.Contains(assetPath))
                            {
                                assetIndex = string.Empty;
                                m_DicAssetContainer.Add(assetPath, assetIndex);
                                return AssetFindResult.InSubPath;
                            }
                        }
                    }
                }

                assetIndex = string.Empty;
                return AssetFindResult.Failure;
            }

            assetIndex = result;
            return string.IsNullOrEmpty(result) ? AssetFindResult.InSubPath : AssetFindResult.Success;
        }

        private static Dictionary<string, string> m_DicAssetContainer = null;
        private static AssetBundleConfig m_AssetBundleConfig = null;
    }
}
