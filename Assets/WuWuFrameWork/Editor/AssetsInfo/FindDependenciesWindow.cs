using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using WuWuFileUtil = WuWuFramework.Utils.FileUtil;
using WuWuPathUtil = WuWuFramework.Utils.PathUtil;

namespace WuWuFramework.Editor
{
    public class FindDependenciesWindow : EditorWindow
    {
        private static string s_FindPath = string.Empty;
        private Dictionary<string, string[]> m_Dependices = new();
        private Dictionary<string, UnityObject> m_Objects = new();
        private HashSet<string> m_DependenceFolders = new();
        private Vector2 m_ScrollPos;
        private List<bool> m_IsFilesToggle = new();

        private void OnEnable()
        {
            minSize = new Vector2(1000, 600);
            InitFindInfo();
        }

        public static void FindDependencies(string path)
        {
            s_FindPath = path;
            FindDependenciesWindow window = GetWindow<FindDependenciesWindow>();
            window.Show();
        }

        private void InitFindInfo()
        {
            m_IsFilesToggle.Clear();
            m_Dependices.Clear();
            m_DependenceFolders.Clear();

            if (Directory.Exists(s_FindPath))
            {
                string[] files = WuWuFileUtil.GetFiles(s_FindPath, "*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    string assetPath = WuWuPathUtil.GetAssetPath(file);
                    string[] referenceFiles = GetAssetDependencies(assetPath);
                    m_Dependices.Add(assetPath, referenceFiles);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(s_FindPath))
                {
                    EditorUtility.DisplayDialog("查找依赖", "请选中你要查找的资源", "确定");
                    return;
                }

                string[] dependencies = GetAssetDependencies(s_FindPath);

                if (dependencies == null)
                {
                    EditorUtility.DisplayDialog("查找依赖", "你要查找的资源不存在", "确定");
                    return;
                }


                m_Dependices.Add(s_FindPath, dependencies);
            }

            foreach (var kvp in m_Dependices)
            {
                m_IsFilesToggle.Add(true);
            }
        }

        private string[] GetAssetDependencies(string assetPath)
        {
            if (!File.Exists(assetPath))
            {
                return null;
            }

            string[] dependencies = AssetDatabase.GetDependencies(assetPath.TrimEnd('/'));
            List<string> filterPath = new();

            for (int i = 0; i < dependencies.Length; i++)
            {
                if (!dependencies[i].Contains(assetPath))
                {
                    filterPath.Add(dependencies[i]);
                    m_DependenceFolders.Add(Path.GetDirectoryName(dependencies[i]).Replace("\\", "/"));
                }
            }

            dependencies = filterPath.ToArray();
            return dependencies;
        }


        private void OnGUI()
        {
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            int index = 0;

            foreach (var kvp in m_Dependices)
            {
                EditorGUILayout.BeginHorizontal();

                if (!m_Objects.TryGetValue(kvp.Key, out UnityObject assetObj))
                {
                    assetObj = AssetDatabase.LoadAssetAtPath(kvp.Key, typeof(UnityObject));
                    m_Objects.Add(kvp.Key, assetObj);
                }

                GUILayout.Label(kvp.Key, GUILayout.Width(600));
                EditorGUILayout.ObjectField("", assetObj, typeof(UnityObject), true);
                m_IsFilesToggle[index] = EditorGUILayout.Toggle("", m_IsFilesToggle[index]);
                EditorGUILayout.EndHorizontal();

                if (m_IsFilesToggle[index])
                {
                    Rect r = EditorGUILayout.BeginVertical("Button");
                    foreach (var fileName in kvp.Value)
                    {
                        assetObj = AssetDatabase.LoadAssetAtPath(fileName, typeof(UnityObject));
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(fileName, GUILayout.Width(600));
                        EditorGUILayout.ObjectField("", assetObj, typeof(UnityObject), true);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }
                index++;
            }

            GUILayout.Label("引用的文件夹：");
            EditorGUILayout.BeginVertical();
            foreach (var dependenceFolder in m_DependenceFolders)
            {
                if (!m_Objects.TryGetValue(dependenceFolder, out UnityObject dirObj))
                {
                    dirObj = AssetDatabase.LoadAssetAtPath(dependenceFolder.TrimEnd('/'), typeof(DefaultAsset));
                    m_Objects.Add(dependenceFolder, dirObj);
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(dependenceFolder, GUILayout.Width(600));
                EditorGUILayout.ObjectField("", dirObj, typeof(UnityObject), true);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}