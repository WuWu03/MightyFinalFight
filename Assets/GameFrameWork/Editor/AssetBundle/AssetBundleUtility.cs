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
            m_DicAssetContainer = new Dictionary<string, int>();
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

            AssetFindResult result = GetAssetBuildMapIndex(assetPath, out int assetIndex);

            if (result == AssetFindResult.Failure)
            {
                return;
            }

            GUIStyle labelStyle = new GUIStyle("AssetLabel");
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = Color.green;
            labelStyle.focused.textColor = Color.green;
            float x = selectionRect.x + selectionRect.width - 40;
            float y = selectionRect.y;
            float width = 40f;
            float height = selectionRect.height;

            if (result == AssetFindResult.Success)
            {
                GUI.Label(new Rect(x, y, width, height), (assetIndex + 1).ToString(), labelStyle);
            }
            else if (result == AssetFindResult.InSubPath)
            {
                GUI.Label(new Rect(x, y, width, height), "*", labelStyle);
            }
        }

        private static AssetFindResult GetAssetBuildMapIndex(string assetPath, out int assetIndex)
        {
            if (m_AssetBundleConfig == null)
            {
                assetIndex = -1;
                return AssetFindResult.Failure;
            }

            if (!m_DicAssetContainer.TryGetValue(assetPath, out int result))
            {
                for (int i = 0; i < m_AssetBundleConfig.listDatas.Count; i++)
                {
                    if (string.IsNullOrEmpty(m_AssetBundleConfig.listDatas[i].assetPath))
                    {
                        continue;
                    }

                    if (m_AssetBundleConfig.listDatas[i].assetPath.Equals(assetPath) || assetPath.StartsWith(m_AssetBundleConfig.listDatas[i].assetPath))
                    {
                        m_DicAssetContainer.Add(assetPath, i);
                        assetIndex = i;
                        return AssetFindResult.Success;
                    }
                    else if (m_AssetBundleConfig.listDatas[i].assetPath.Contains(assetPath))
                    {
                        m_DicAssetContainer.Add(assetPath, -1);
                        assetIndex = -1;
                        return AssetFindResult.InSubPath;
                    }
                }

                assetIndex = -1;
                return AssetFindResult.Failure;
            }

            assetIndex = result;

            if (result == -1)
            {
                return AssetFindResult.InSubPath;
            }

            return AssetFindResult.Success;
        }

        private static Dictionary<string, int> m_DicAssetContainer = null;
        private static AssetBundleConfig m_AssetBundleConfig = null;
    }
}
