using System.Collections.Generic;
using GameFrameWork.Utils;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [InitializeOnLoad]
    public static class AssetBundleUtility
    {
        enum AssetFindResult
        {
            Failure = 1, //失败
            Success = 2, //成功
            InSubPath = 3, //子目录有资源
        }

        struct AssetInfo
        {
            public int bundleIndex;
            public int assetIndex;
            public AssetBundleData.BundleBuildType bundleBuildType;
        }

        static AssetBundleUtility()
        {
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemGUI;
            m_AssetBundleConfig = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.AssetBundleWindowDataPath);
            m_DicAssetContainer = new();
        }

        public static void RefreshData()
        {
            m_AssetBundleConfig = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(EditorPathUtil.AssetBundleWindowDataPath);
            m_DicAssetContainer.Clear();
        }

        private static void ProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(assetPath) || !assetPath.StartsWith("Assets"))
            {
                return;
            }

            assetPath = PathUtil.FormatPath(assetPath);
            AssetFindResult result = GetAssetBuildMapIndex(assetPath, out AssetInfo assetInfo);

            if (result == AssetFindResult.Failure)
            {
                return;
            }

            string text = string.Empty;
            string styleName = string.Empty;

            if (result == AssetFindResult.Success)
            {
                styleName = GetAssetBuildTypeColor(assetInfo.bundleBuildType);
                text = $"{assetInfo.bundleIndex + 1}_{assetInfo.assetIndex + 1}";
            }
            else if (result == AssetFindResult.InSubPath)
            {
                styleName = "sv_label_0";
                text = "*";
            }

            GUIStyle labelStyle = new(styleName)
            {
                alignment = TextAnchor.MiddleCenter
            };

            float width = 30f;
            float height = selectionRect.height;
            float x = selectionRect.x + selectionRect.width - width;
            float y = selectionRect.y;
            GUI.Label(new Rect(x, y, width, height), text, labelStyle);
        }

        private static string GetAssetBuildTypeColor(AssetBundleData.BundleBuildType bundleBuildType)
        {
            return bundleBuildType switch
            {
                AssetBundleData.BundleBuildType.Single => "sv_label_6",
                AssetBundleData.BundleBuildType.MultiSingle => "sv_label_4",
                AssetBundleData.BundleBuildType.Multi => "sv_label_3",
                _ => "sv_label_0"
            };
        }

        private static AssetFindResult GetAssetBuildMapIndex(string assetPath, out AssetInfo assetInfo)
        {
            if (m_AssetBundleConfig == null)
            {
                assetInfo = new AssetInfo()
                {
                    bundleIndex = -1,
                    assetIndex = -1,
                };
                return AssetFindResult.Failure;
            }

            if (!m_DicAssetContainer.TryGetValue(assetPath, out AssetInfo result))
            {
                for (int i = 0; i < m_AssetBundleConfig.listDatas.Count; i++)
                {
                    if (m_AssetBundleConfig.listDatas[i].assetPaths == null ||
                        m_AssetBundleConfig.listDatas[i].assetPaths.Count < 1)
                    {
                        continue;
                    }

                    int index = m_AssetBundleConfig.listDatas[i].assetPaths.IndexOf(assetPath);
                    if (index >= 0)
                    {
                        assetInfo = new()
                        {
                            bundleIndex = i,
                            assetIndex = index,
                            bundleBuildType = m_AssetBundleConfig.listDatas[i].bundleBuildType
                        };

                        m_DicAssetContainer.Add(assetPath, assetInfo);
                        return AssetFindResult.Success;
                    }

                    for (int j = 0; j < m_AssetBundleConfig.listDatas[i].assetPaths.Count; j++)
                    {
                        string tempAsetPath = m_AssetBundleConfig.listDatas[i].assetPaths[j];

                        if (assetPath.StartsWith(tempAsetPath))
                        {
                            assetInfo = new()
                            {
                                bundleIndex = i,
                                assetIndex = j,
                                bundleBuildType = m_AssetBundleConfig.listDatas[i].bundleBuildType
                            };

                            m_DicAssetContainer.Add(assetPath, assetInfo);
                            return AssetFindResult.Success;
                        }

                        if (tempAsetPath.Contains(assetPath))
                        {
                            assetInfo = new()
                            {
                                bundleIndex = -1,
                                assetIndex = -1,
                            };
                            m_DicAssetContainer.Add(assetPath, assetInfo);
                            return AssetFindResult.InSubPath;
                        }
                    }
                }

                assetInfo = default;
                return AssetFindResult.Failure;
            }

            assetInfo = result;
            return result.bundleIndex == -1 ? AssetFindResult.InSubPath : AssetFindResult.Success;
        }

        private static Dictionary<string, AssetInfo> m_DicAssetContainer = null;
        private static AssetBundleConfig m_AssetBundleConfig = null;
    }
}