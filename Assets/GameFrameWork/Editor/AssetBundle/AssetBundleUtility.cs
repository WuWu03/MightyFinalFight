using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [InitializeOnLoad]
    public static class AssetBundleUtility
    {
        static AssetBundleUtility()
        {
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemGUI;
            m_AssetBundleConfig = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.assetBundleDataPath);
            m_DicAssetContainer = new Dictionary<string, int>();
        }

        public static void RefreshData()
        {
            m_AssetBundleConfig = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.assetBundleDataPath);
            m_DicAssetContainer.Clear();
        }

        private static void ProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
  
            if (string.IsNullOrEmpty(assetPath) || !assetPath.Contains("Assets"))
            {
                return;
            }

            if(!string.IsNullOrEmpty(Path.GetExtension(assetPath)))
            {
                assetPath = assetPath.Substring(0, assetPath.LastIndexOf("/") + 1);
            }
            else if(!assetPath.EndsWith("/"))
            {
                assetPath = assetPath + "/";
            }

            int result = GetAssetBuildMapIndex(assetPath);

            if (result != -1)
            {
                GUIStyle labelStyle = new GUIStyle("AssetLabel");
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.normal.textColor = Color.green;
                labelStyle.focused.textColor = Color.green;
                float x = selectionRect.x + selectionRect.width - 40;
                float y = selectionRect.y;
                float width = 40f;
                float height = selectionRect.height;

                if (result >= 0)
                {
                    GUI.Label(new Rect(x, y, width, height), (result + 1).ToString(), labelStyle);
                }
                else
                {
                    GUI.Label(new Rect(x, y, width, height), "*", labelStyle);
                }
            }
        }

        /// <summary>
        /// -1没找到，-2路径有打包资源
        /// </summary>
        private static int GetAssetBuildMapIndex(string assetPath)
        {
            if (m_AssetBundleConfig == null)
            {
                return -1;
            }

            if (!m_DicAssetContainer.TryGetValue(assetPath, out int result))
            {
                for (int i = 0; i < m_AssetBundleConfig.Datas.Count; i++)
                {
                    if (i == 2)
                    {

                    }
                    if (string.IsNullOrEmpty(m_AssetBundleConfig.Datas[i].AssetPath))
                    {
                        continue;
                    }

                    if (m_AssetBundleConfig.Datas[i].AssetPath.Equals(assetPath) || assetPath.StartsWith(m_AssetBundleConfig.Datas[i].AssetPath))
                    {
                        m_DicAssetContainer.Add(assetPath, i);
                        return i;
                    }
                    else if (m_AssetBundleConfig.Datas[i].AssetPath.Contains(assetPath))
                    {
                        m_DicAssetContainer.Add(assetPath, -2);
                        return -2;
                    }
                }

                return -1;
            }

            return result;
        }

        private static Dictionary<string, int> m_DicAssetContainer = null;
        private static AssetBundleConfig m_AssetBundleConfig = null;
    }
}
