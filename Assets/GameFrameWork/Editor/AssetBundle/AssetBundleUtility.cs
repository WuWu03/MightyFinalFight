using GameFrameWork.Serialize;
using GameFrameWork.Utilities;
using System;
using System.Collections;
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
            m_DicAssetContainer = new Dictionary<string, bool>();
        }

        private static void ProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (!assetPath.Contains("Assets"))
            {
                return;
            }

            string path = assetPath;

            if(!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = assetPath.Substring(0, assetPath.LastIndexOf("/"));
            }

            if (IsAssetInBuildMap(path))
            {
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);

                labelStyle.normal.textColor = Color.red;

                float x = selectionRect.x + selectionRect.width - 15;
                float y = selectionRect.y;
                float width = 15f;
                float height = selectionRect.height;

                GUI.Label(new Rect(x, y, width, height), "¡ù", labelStyle);
            }
        }

        private static bool IsAssetInBuildMap(string assetPath)
        {
            bool result = false;

            if (m_AssetBundleConfig == null)
            {
                return result;
            }

            if (!m_DicAssetContainer.TryGetValue(assetPath, out result))
            {
                for (int i = 0; i < m_AssetBundleConfig.Datas.Count; i++)
                {
                    if(m_AssetBundleConfig.Datas[i].AssetPath.Contains(assetPath))
                    {
                        result = true;
                        m_DicAssetContainer.Add(assetPath, true);
                        break;
                    }
                }
            }

            return result;
        }


        private static Dictionary<string, bool> m_DicAssetContainer = null;
        private static AssetBundleConfig m_AssetBundleConfig = null;
    }
}
