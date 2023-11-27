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
            m_DicAssetContainer = new Dictionary<string, int>();
        }

        private static void ProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (!assetPath.Contains("Assets") || assetPath.Substring(assetPath.IndexOf("Assets")).Equals("Assets"))
            {
                return;
            }

            string path = assetPath;

            if(!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = assetPath.Substring(0, assetPath.LastIndexOf("/"));
            }

            int result = IsAssetInBuildMap(path);

            if (result != 0)
            {
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = result == 1 ? Color.red : Color.green;

                float x = selectionRect.x + selectionRect.width - 40; 
                float y = selectionRect.y;
                float width = 40f;
                float height = selectionRect.height;
                string label = result == 1 ? "Map" : "Single";

                GUI.Label(new Rect(x, y, width, height), label, labelStyle);
            }
        }

        private static int IsAssetInBuildMap(string assetPath)
        {
            int result = 0;

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
                        result = m_AssetBundleConfig.Datas[i].BundleType == AssetBundleData.AssetType.Map ? 1 : 2;
                        m_DicAssetContainer.Add(assetPath, result);
                        break;
                    }
                }
            }
            else
            {
                bool hasFind = false;

                for (int i = 0; i < m_AssetBundleConfig.Datas.Count; i++)
                {
                    if (m_AssetBundleConfig.Datas[i].AssetPath.Contains(assetPath))
                    {
                        hasFind = true;
                        break;
                    }
                }

                if (!hasFind)
                {
                    m_DicAssetContainer.Remove(assetPath);
                    result = 0;
                }
            }

            return result;
        }


        private static Dictionary<string, int> m_DicAssetContainer = null;
        private static AssetBundleConfig m_AssetBundleConfig = null;
    }
}
