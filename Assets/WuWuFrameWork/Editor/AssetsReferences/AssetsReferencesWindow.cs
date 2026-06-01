//art tools 可视化
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using WuWuFramework.Event;
using UnityObject = UnityEngine.Object;

public class AssetsReferencesWindow : EditorWindow
{
    //文件夹数据
    class FolderData
    {
        public bool isSelected = false;
        public int indent = 0;
        public GUIContent content;
        public string assetPath;
        public List<FolderData> children = new();
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
    private FolderData m_SelectFolderData;
    private string m_SearchKey = string.Empty;
    private Vector2 m_ScrollPos;
    private EditorApplication.CallbackFunction m_UpdateDelegate;
    private delegate long ThreadRun(ThreadData threadData);
    private WuWuFrameworkFunc<ThreadData, long>[] m_ThreadFuncs;
    private IAsyncResult[] m_ThreadFuncResults;
    private bool m_IsInit = false;
    private const int ThreadCount = 4;

    void OnEnable()
    {
        InitFolderData();
    }

    void OnGUI()
    {
        GUILayout.Label("提示1:标红说明图片大小超过256*256");
        GUILayout.Label("提示2:第一个是资源在硬盘大小");
        GUILayout.Label("提示3:第二个是资源在内存大小");
        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();
        m_SearchKey = EditorGUILayout.TextField("", m_SearchKey, GUILayout.Width(200));
        if (GUILayout.Button("搜索", GUILayout.Width(100)))
        {
            SearchDataPath(m_CurrFolderData, m_SearchKey);
        }

        GUILayout.Space(20);

        if (GUILayout.Button("刷新", GUILayout.Width(100)))
        {
            InitFolderData();
        }
        EditorGUILayout.EndHorizontal();

        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height - 100));
        OnFolderGUI();
        EditorGUILayout.EndScrollView();
    }

    private long InitFolderData()
    {
        m_CurrFolderData ??= new();
        m_CurrFolderData.isSearch = false;
        m_CurrFolderData.children.Clear();
        m_ThreadFuncs ??= new WuWuFrameworkFunc<ThreadData, long>[ThreadCount];
        m_ThreadFuncResults ??= new IAsyncResult[ThreadCount];
        m_IsInit = false;

        string assetPath = Path.Combine("Assets", "ArtResources");
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

        foreach (string filePath in Directory.GetFiles(assetPath))
        {
            content = GetGUIContent(filePath);

            if (content != null)
            {
                FolderData child = new()
                {
                    indent = indent + 1,
                    content = content,
                    assetPath = filePath,
                    isIllegalImage = ValidateImage(filePath),
                    fileMemorySize = GetFileMemorySize(filePath),
                    fileSize = GetFileSize(filePath)
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
            FolderData childDir = new();
            threadData[index].folders.Add(childDir);
            threadData[index].paths.Add(path);
            m_CurrFolderData.children.Add(childDir);
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
            EditorApplication.update -= m_UpdateDelegate;

            for (int i = 0; i < ThreadCount; i++)
            {
                m_CurrFolderData.fileSize += m_ThreadFuncs[i].EndInvoke(m_ThreadFuncResults[i]);
            }
        }
        else
        {
            EditorUtility.DisplayProgressBar("匹配资源中", string.Format("进度：{0}", finishedCount), finishedCount * 1f / ThreadCount);
        }
    }

    private long CacFolderFilesSize(ThreadData threadData)
    {
        long folderFilesSize = 0;

        if (threadData != null)
        {
            for (int i = 0; i < threadData.folders.Count; i++)
            {
                folderFilesSize += CacFolderFilesSize(threadData.folders[i], threadData.paths[i], 1);
            }
        }

        return folderFilesSize;
    }


    private long CacFolderFilesSize(FolderData data, string currentPath, int indent = 0)
    {
        if (currentPath.EndsWith(".meta"))
        {
            return 0;
        }

        long dataSize = 0;
        data.indent = indent;
        data.assetPath = currentPath;

        foreach (var path in Directory.GetFiles(currentPath))
        {
            if (path.EndsWith(".meta"))
            {
                continue;
            }

            FolderData child = new()
            {
                indent = indent + 1,
                assetPath = path,
                fileSize = GetFileSize(path)
            };
            dataSize += child.fileSize;
            data.children.Add(child);
        }

        foreach (var path in Directory.GetDirectories(currentPath))
        {
            FolderData childDir = new();
            data.children.Add(childDir);
            dataSize += CacFolderFilesSize(childDir, path, indent + 1);
        }

        data.fileSize = dataSize;
        return dataSize;
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

    private void DrawFile(FolderData data)
    {
        if (!data.isSearch)
        {
            return;
        }

        EditorGUILayout.BeginHorizontal();
        if (!data.isCreateContent)
        {
            data.isCreateContent = true;
            data.content = GetGUIContent(data.assetPath);
            data.isIllegalImage = ValidateImage(data.assetPath);
            data.fileMemorySize = GetFileMemorySize(data.assetPath);
        }

        GUIStyle style = "Label";
        Rect rt = GUILayoutUtility.GetRect(data.content, style);

        if (data.isSelected)
        {
            EditorGUI.DrawRect(rt, Color.gray);
        }

        if (data.isIllegalImage)
        {
            EditorGUI.DrawRect(rt, Color.red);
        }

        rt.x += (16 * EditorGUI.indentLevel);

        if (GUI.Button(rt, data.content, style))
        {
            if (m_SelectFolderData != null)
            {
                m_SelectFolderData.isSelected = false;
            }
            data.isSelected = true;
            m_SelectFolderData = data;
            data.isExpand = !data.isExpand;
        }

        GUILayout.Label(EditorUtility.FormatBytes(data.fileSize), GUILayout.Width(60));
        GUILayout.Label(EditorUtility.FormatBytes(data.fileMemorySize), GUILayout.Width(60));

        if (GUILayout.Button("引用", GUILayout.Width(60)))
        {
            FindFileReferences(data.assetPath);
        }

        if (GUILayout.Button("被引用", GUILayout.Width(60)))
        {
            FindFileUsed(data.assetPath);
        }

        EditorGUILayout.EndHorizontal();
    }


    //查找引用了哪些资源
    private static void FindFileReferences(string assetPath)
    {
        if (Directory.Exists(assetPath))
        {
            Finddependent.FindFolderDependentByArtToolsWindow(assetPath);
        }
        else
        {
            Finddependent.FindAssetDependentByArtToolsWindow(assetPath);
        }
    }


    //查找本资源被哪些资源引用
    private void FindFileUsed(string assetPath)
    {
        FindReferences01.FindThread(assetPath);
    }


    //硬盘占用
    private long GetFileSize(string path)
    {
        FileInfo fi = new(path);
        return fi.Length;
    }

    //内存占用
    private long GetFileMemorySize(string assetPath)
    {
        UnityObject assetObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityObject));
        return Profiler.GetRuntimeMemorySizeLong(assetObj);
    }

    private bool ValidateImage(string assetPath)
    {
        //if (path.Contains(SpriteAtlasTool.AtlasName))
        //{
        //    Texture assetObj = (Texture)AssetDatabase.LoadAssetAtPath(path, typeof(Texture));
        //    if(assetObj == null)
        //    {
        //        return false;
        //    }
        //    if(assetObj.width > 256 || assetObj.height > 256)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //else
        //{
        //    return false;
        //}
        return false;
    }

    private GUIContent GetGUIContent(string path)
    {
        UnityObject asset = AssetDatabase.LoadAssetAtPath(path, typeof(UnityObject));

        if (asset)
        {
            return new GUIContent(asset.name, AssetDatabase.GetCachedIcon(path));
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