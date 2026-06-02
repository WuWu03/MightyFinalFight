using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using WuWuFramework.Event;
using UnityColor = UnityEngine.Color;
using UnityObject = UnityEngine.Object;

namespace WuWuFramework.Editor
{
    public class AssetsInfoWindow : EditorWindow
    {
        //文件夹数据
        class FolderData
        {
            public List<FolderData> children = new();
            public bool isRoot = false;
            public int indent = 0;
            public GUIContent content;
            public string assetPath;
            public long fileSize = 0;  //文件在硬盘大小
            public long fileMemorySize = 0;//文件在内存大小
            public bool isIllegalImage = false;
            public bool isExpand = false;
            public bool isCreateContent = false;
            public bool isSearch = true;
        }

        class ThreadData
        {
            public List<FolderData> folders = new();
            public List<string> paths = new();
        }

        private FolderData m_CurrFolderData;
        private string m_SearchKey = string.Empty;
        private Vector2 m_ScrollPos;
        private WuWuFrameworkFunc<ThreadData, long>[] m_ThreadFuncs;
        private IAsyncResult[] m_ThreadFuncResults;
        private bool m_IsInit = false;
        private const int ThreadCount = 4;

        void OnEnable()
        {
            InitFolderData();
            minSize = new Vector2(700, 400);
        }

        void OnGUI()
        {
            GUILayout.Label("提示1:标红说明图片大小超过256*256");
            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            string searchKey = EditorGUILayout.TextField("", m_SearchKey);
            bool isResetSearch = !string.Equals(searchKey, m_SearchKey) && string.IsNullOrEmpty(searchKey);
            m_SearchKey = searchKey;

            if (GUILayout.Button("搜索", GUILayout.Width(100)) || isResetSearch)
            {
                SearchDataPath(m_CurrFolderData, m_SearchKey);
            }
            if (GUILayout.Button("刷新", GUILayout.Width(100)))
            {
                InitFolderData();
            }
            GUILayout.Label("硬盘占用", GUILayout.Width(60));
            GUILayout.Label("内存占用", GUILayout.Width(60));
            GUILayout.Space(120);
            EditorGUILayout.EndHorizontal();

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            OnFolderGUI();
            EditorGUILayout.EndScrollView();
        }

        private long InitFolderData()
        {
            m_CurrFolderData ??= new();
            m_CurrFolderData.isSearch = false;
            m_CurrFolderData.children.Clear();
            m_CurrFolderData.isRoot = true;
            m_ThreadFuncs ??= new WuWuFrameworkFunc<ThreadData, long>[ThreadCount];
            m_ThreadFuncResults ??= new IAsyncResult[ThreadCount];
            m_IsInit = false;

            string assetPath = "Assets/ArtResources";
            int indent = 0;
            long dataSize = 0;
            GUIContent content = GetGUIContent(assetPath);

            if (content != null)
            {
                m_CurrFolderData.indent = indent;
                m_CurrFolderData.content = content;
                m_CurrFolderData.assetPath = assetPath;
                m_CurrFolderData.isIllegalImage = ValidateImage(assetPath);
                m_CurrFolderData.fileMemorySize = GetFileMemorySize(assetPath);
            }

            foreach (string file in WuWuFramework.Utils.FileUtil.GetFiles(assetPath))
            {
                content = GetGUIContent(file);

                if (content != null)
                {
                    FolderData child = new()
                    {
                        indent = indent + 1,
                        content = content,
                        assetPath = file,
                        isIllegalImage = ValidateImage(file),
                        fileMemorySize = GetFileMemorySize(file),
                        fileSize = GetFileSize(file)
                    };
                    dataSize += child.fileSize;
                    m_CurrFolderData.children.Add(child);
                }
            }

            m_CurrFolderData.fileSize = dataSize;
            ThreadData[] threadData = new ThreadData[ThreadCount];

            for (int index = 0; index < ThreadCount; index++)
            {
                threadData[index] = new ThreadData();
            }

            int assetCount = 0;
            foreach (string path in Directory.GetDirectories(assetPath))
            {
                int index = assetCount % ThreadCount;
                FolderData child = new();
                threadData[index].folders.Add(child);
                threadData[index].paths.Add(path);
                m_CurrFolderData.children.Add(child);
                assetCount++;
            }

            for (int i = 0; i < ThreadCount; i++)
            {
                m_ThreadFuncs[i] = CacFolderFilesSize;
                m_ThreadFuncResults[i] = m_ThreadFuncs[i].BeginInvoke(threadData[i], null, null);
            }

            EditorApplication.update += FindAssets;
            return dataSize;
        }



        private void FindAssets()
        {
            if (m_IsInit)
            {
                return;
            }

            int finishedCount = 0;

            for (int i = 0; i < ThreadCount; i++)
            {
                if (m_ThreadFuncResults[i].IsCompleted)
                {
                    finishedCount++;
                }
            }

            if (finishedCount >= ThreadCount)
            {
                m_IsInit = true;
                m_CurrFolderData.isSearch = true;
                EditorUtility.ClearProgressBar();
                EditorApplication.update -= FindAssets;

                for (int i = 0; i < ThreadCount; i++)
                {
                    long fileSize = m_ThreadFuncs[i].EndInvoke(m_ThreadFuncResults[i]);
                    m_CurrFolderData.fileSize += fileSize;
                }
            }
            else
            {
                EditorUtility.DisplayProgressBar("匹配资源中", string.Format("进度：{0}", finishedCount), finishedCount * 1f / ThreadCount);
            }
        }

        private long CacFolderFilesSize(ThreadData threadData)
        {
            long fileSize = 0;

            if (threadData != null)
            {
                for (int i = 0; i < threadData.folders.Count; i++)
                {
                    fileSize += CacFolderFilesSize(threadData.folders[i], threadData.paths[i], 1);
                }
            }

            return fileSize;
        }

        private long CacFolderFilesSize(FolderData folderData, string currentPath, int indent = 0)
        {
            long fileSize = 0;
            folderData.indent = indent;
            folderData.assetPath = currentPath;

            foreach (string file in WuWuFramework.Utils.FileUtil.GetFiles(currentPath))
            {
                FolderData child = new()
                {
                    indent = indent + 1,
                    assetPath = file,
                    fileSize = GetFileSize(file),
                };
                fileSize += child.fileSize;
                folderData.children.Add(child);
            }

            foreach (string directory in WuWuFramework.Utils.FileUtil.GetDirectories(currentPath))
            {
                FolderData child = new();
                folderData.children.Add(child);
                fileSize += CacFolderFilesSize(child, directory, indent + 1);
            }

            folderData.fileSize = fileSize;
            return fileSize;
        }

        private void OnFolderGUI()
        {
            if (m_CurrFolderData == null)
            {
                return;
            }
            GUI.enabled = true;
            EditorGUIUtility.SetIconSize(Vector2.one * 16);
            DrawFolderData(m_CurrFolderData);
        }

        private void DrawFolderData(FolderData data)
        {
            if (!data.isSearch)
            {
                return;
            }

            if (!data.isCreateContent)
            {
                data.isCreateContent = true;
                data.content = GetGUIContent(data.assetPath);
                data.isIllegalImage = ValidateImage(data.assetPath);
                data.fileMemorySize = GetFileMemorySize(data.assetPath);
            }

            if (data.content != null)
            {
                EditorGUI.indentLevel = data.indent;
                DrawFile(data);
            }

            for (int i = 0; i < data.children.Count; i++)
            {
                FolderData child = data.children[i];

                if (!child.isCreateContent)
                {
                    child.isCreateContent = true;
                    child.content = GetGUIContent(child.assetPath);
                    child.isIllegalImage = ValidateImage(child.assetPath);
                    child.fileMemorySize = GetFileMemorySize(child.assetPath);
                }

                if (child.content != null)
                {
                    EditorGUI.indentLevel = child.indent;
                    if (child.children.Count > 0 && child.isExpand)
                    {
                        DrawFolderData(child);
                    }
                    else
                    {
                        DrawFile(child);
                    }
                }
            }
        }

        private void DrawFile(FolderData folderData)
        {
            if (!folderData.isSearch)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            if (!folderData.isCreateContent)
            {
                folderData.isCreateContent = true;
                folderData.content = GetGUIContent(folderData.assetPath);
                folderData.isIllegalImage = ValidateImage(folderData.assetPath);
                folderData.fileMemorySize = GetFileMemorySize(folderData.assetPath);
            }

            if (folderData.children.Count > 0)
            {
                if (!folderData.isRoot)
                {
                    folderData.isExpand = EditorGUILayout.Foldout(folderData.isExpand, folderData.content);
                }
                else
                {
                    EditorGUILayout.Foldout(true, folderData.content);
                }
            }
            else
            {
                Rect rect = GUILayoutUtility.GetRect(folderData.content, "Label");
                int xOffest = 16 * EditorGUI.indentLevel;
                rect.x += xOffest;

                if (folderData.isIllegalImage)
                {
                    UnityColor rectColor = UnityColor.red;
                    rectColor.a = 0.5f;
                    EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width - xOffest, rect.height), rectColor);
                }

                GUI.Label(rect, folderData.content, new("Label"));
            }

            GUILayout.Label(EditorUtility.FormatBytes(folderData.fileSize), GUILayout.Width(60));
            GUILayout.Label(EditorUtility.FormatBytes(folderData.fileMemorySize), GUILayout.Width(60));

            if (GUILayout.Button("引用", GUILayout.Width(60)))
            {
                FindFileReferences(folderData.assetPath);
            }

            if (GUILayout.Button("被引用", GUILayout.Width(60)))
            {
                FindFileUsed(folderData.assetPath);
            }

            EditorGUILayout.EndHorizontal();
        }


        //查找引用了哪些资源
        private static void FindFileReferences(string assetPath)
        {
            if (Directory.Exists(assetPath))
            {
                FindDependenciesWindow.FindFolderDependencies(assetPath);
            }
            else
            {
                FindDependenciesWindow.FindAssetDependencies(assetPath);
            }
        }


        //查找本资源被哪些资源引用
        private void FindFileUsed(string assetPath)
        {
            FindReferencesWindow.FindThread(assetPath);
        }


        //硬盘占用
        private long GetFileSize(string path)
        {
            return new FileInfo(path).Length;
        }

        //内存占用
        private long GetFileMemorySize(string assetPath)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(assetPath)) || !File.Exists(assetPath))
            {
                return 0;
            }

            UnityObject assetObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityObject));
            return Profiler.GetRuntimeMemorySizeLong(assetObj);
        }

        private bool ValidateImage(string assetPath)
        {
            string uiSpritesPath = WuWuFramework.Editor.EditorMgr.GetWuWuFrameworkConfig().uiSpritesPath;

            if (assetPath.Contains(uiSpritesPath))
            {
                if (!AssetDatabase.AssetPathExists(assetPath) || string.IsNullOrEmpty(Path.GetExtension(assetPath)))
                {
                    return false;
                }

                using FileStream fileStream = new(assetPath, FileMode.Open, FileAccess.Read);
                using Image image = Image.FromStream(fileStream);
                return image.Width > 256 || image.Height > 256;
            }

            return false;
        }

        private GUIContent GetGUIContent(string path)
        {
            if (AssetDatabase.AssetPathExists(path))
            {
                return new GUIContent(Path.GetFileName(path), AssetDatabase.GetCachedIcon(path));
            }

            return null;
        }

        private bool SearchDataPath(FolderData searchData, string searchKey)
        {
            bool isSearch = string.IsNullOrEmpty(searchKey) || searchData.assetPath.Contains(searchKey);

            foreach (FolderData child in searchData.children)
            {
                bool hasSearch = SearchDataPath(child, searchKey);

                if (hasSearch)
                {
                    isSearch = hasSearch;
                }
            }

            searchData.isSearch = isSearch;
            return isSearch;
        }
    }
}