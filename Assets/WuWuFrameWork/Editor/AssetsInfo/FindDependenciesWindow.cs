using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace WuWuFramework.Editor
{
    public class FindDependenciesWindow : EditorWindow
    {
        private static Dictionary<string, string[]> s_Dependices = new();
        private static HashSet<string> s_DependenceFolders = new();
        private Vector2 m_ScrollPos;
        private List<bool> m_IsFilesToggle = new();

        private void OnEnable()
        {
            minSize = new Vector2(1000, 600);
            m_IsFilesToggle.Clear();
            foreach (var kvp in s_Dependices)
            {
                m_IsFilesToggle.Add(true);
            }
        }

        //可视化使用
        public static void FindAssetDependencies(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("查找依赖", "请选中你要查找的资源", "确定");
                return;
            }
            string[] dependencies = GetAssetDependencies(path);

            if (dependencies == null)
            {
                EditorUtility.DisplayDialog("查找依赖", "你要查找的资源不存在", "确定");
                return;
            }

            s_Dependices.Clear();
            s_Dependices.Add(path, dependencies);
            FindDependenciesWindow window = GetWindow<FindDependenciesWindow>();
            window.Show();
        }

        //可视化使用
        public static void FindFolderDependencies(string path)
        {
            string[] files = WuWuFramework.Utils.FileUtil.GetFiles(path, "*", SearchOption.AllDirectories);
            s_Dependices.Clear();

            foreach (string file in files)
            {
                string assetPath = WuWuFramework.Utils.PathUtil.GetAssetPath(file);
                string[] referenceFiles = GetAssetDependencies(assetPath);
                s_Dependices.Add(assetPath, referenceFiles);
            }

            FindDependenciesWindow window = GetWindow<FindDependenciesWindow>();
            window.Show();
        }

        private static string[] GetAssetDependencies(string assetPath)
        {
            if (!File.Exists(assetPath))
            {
                return null;
            }

            string[] dependencies = AssetDatabase.GetDependencies(assetPath);
            List<string> filterPath = new();
            s_DependenceFolders.Clear();

            for (int i = 0; i < dependencies.Length; i++)
            {
                if (!dependencies[i].Contains(assetPath))
                {
                    filterPath.Add(dependencies[i]);
                    s_DependenceFolders.Add(Path.GetDirectoryName(dependencies[i]).Replace("\\", "/"));
                }
            }

            dependencies = filterPath.ToArray();
            return dependencies;
        }


        private void OnGUI()
        {
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            int index = 0;

            foreach (var kvp in s_Dependices)
            {
                EditorGUILayout.BeginHorizontal();
                Object assetObj = AssetDatabase.LoadAssetAtPath(kvp.Key, typeof(Object));
                GUILayout.Label(kvp.Key, GUILayout.Width(600));
                EditorGUILayout.ObjectField("", assetObj, typeof(Object), true);
                m_IsFilesToggle[index] = EditorGUILayout.Toggle("", m_IsFilesToggle[index]);
                EditorGUILayout.EndHorizontal();

                if (m_IsFilesToggle[index])
                {
                    Rect r = EditorGUILayout.BeginVertical("Button");
                    foreach (var fileName in kvp.Value)
                    {
                        assetObj = AssetDatabase.LoadAssetAtPath(fileName, typeof(Object));
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(fileName, GUILayout.Width(600));
                        EditorGUILayout.ObjectField("", assetObj, typeof(Object), true);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }
                index++;
            }

            GUILayout.Label("引用的文件夹：");
            EditorGUILayout.BeginVertical();
            foreach (var dependenceFolder in s_DependenceFolders)
            {
                Object dirObj = AssetDatabase.LoadAssetAtPath(dependenceFolder, typeof(Object));
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(dependenceFolder, GUILayout.Width(600));
                EditorGUILayout.ObjectField("", dirObj, typeof(Object), true);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}