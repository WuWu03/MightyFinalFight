using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using WuWuFramework.Event;
using UnityObject = UnityEngine.Object;
using UnityColor = UnityEngine.Color;

namespace WuWuFramework.Editor
{
    public class FindReferencesWindow : EditorWindow
    {
        class ThreadData
        {
            public List<string> checkAssets = new();
            public List<string> checkCSharpScripts = new();
            public List<string> assetsGuids = new();
            public List<string> assetsNames = new();
            public List<string> checkWhiteList = new();
        }

        private static string m_FindPath = string.Empty;
        private Dictionary<string, List<string>> m_References;
        private WuWuFrameworkFunc<ThreadData, Dictionary<string, List<string>>>[] m_ThreadFuncs;
        private IAsyncResult[] m_ThreadFuncResults;
        private bool m_IsInit = false;
        private bool m_ShowNoUsed = true;
        private bool m_ShowUsed = true;
        private List<bool> m_UsedHasShow = new();
        private Vector2 m_ScrollPos;
        private const int ThreadCount = 4;

        public static void FindThread(string path, bool outExcel = false)
        {
            m_FindPath = path;
            FindReferencesWindow window = GetWindow<FindReferencesWindow>();
            window.Show();
        }

        private void OnEnable()
        {
            minSize = new Vector2(1100, 600);
            InitFindData();
        }

        private void OnDisable()
        {
            EditorUtility.ClearProgressBar();
        }

        private void InitFindData()
        {
            m_References ??= new Dictionary<string, List<string>>();
            m_References.Clear();
            m_UsedHasShow.Clear();
            m_IsInit = false;
            EditorSettings.serializationMode = SerializationMode.ForceText;
            AssetDatabase.Refresh();

            if (string.IsNullOrEmpty(m_FindPath))
            {
                return;
            }

            List<string> assetsGuids = new();
            List<string> assetsNames = new();

            if (Directory.Exists(m_FindPath))
            {
                string[] allFiles = WuWuFramework.Utils.FileUtil.GetFiles(m_FindPath);

                foreach (string file in allFiles)
                {
                    string assetPath = WuWuFramework.Utils.PathUtil.GetAssetPath(file);
                    assetsNames.Add(assetPath);
                    assetsGuids.Add(AssetDatabase.AssetPathToGUID(assetPath));
                }
            }
            else
            {
                assetsGuids.Add(AssetDatabase.AssetPathToGUID(m_FindPath));
                assetsNames.Add(m_FindPath);
            }

            ThreadData[] threadData = new ThreadData[ThreadCount];

            for (int i = 0; i < ThreadCount; i++)//添加查找的udid
            {
                threadData[i] = new()
                {
                    assetsGuids = assetsGuids,
                    assetsNames = assetsNames
                };
            }

            string assetsFullPath = WuWuFramework.Utils.PathUtil.GetAssetFullPath("ArtResources");
            string[] withoutExtensions = new string[] { ".prefab", ".unity", ".mat", ".asset" };
            string[] findFiles = WuWuFramework.Utils.FileUtil.GetFiles(assetsFullPath, "*", SearchOption.AllDirectories).Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();

            for (int i = 0; i < findFiles.Length; i++)//添加要查找的资源文件
            {
                int index = i % ThreadCount;
                threadData[index].checkAssets.Add(findFiles[i]);
            }

            string[] cshaprFiles = WuWuFramework.Utils.FileUtil.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories).Where(s => !s.ToLower().Contains("/editor/")).ToArray();

            for (int i = 0; i < cshaprFiles.Length; i++)//添加要查找的CS文件
            {
                int index = i % ThreadCount;
                threadData[index].checkCSharpScripts.Add(cshaprFiles[i]);
            }

            m_ThreadFuncs ??= new WuWuFrameworkFunc<ThreadData, Dictionary<string, List<string>>>[ThreadCount];
            m_ThreadFuncResults ??= new IAsyncResult[ThreadCount];

            for (int i = 0; i < ThreadCount; i++)
            {
                m_ThreadFuncs[i] = ThreadFind;
                m_ThreadFuncResults[i] = m_ThreadFuncs[i].BeginInvoke(threadData[i], null, null);
            }

            EditorApplication.update += FindReferences;
        }

        private void FindReferences()
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

                for (int i = 0; i < ThreadCount; i++)
                {
                    Dictionary<string, List<string>> referencesInfo = m_ThreadFuncs[i].EndInvoke(m_ThreadFuncResults[i]);

                    foreach (var keyValue in referencesInfo)
                    {
                        var key = keyValue.Key;

                        if (key.Contains("/") || key.Contains("\\"))
                        {
                            key = AssetDatabase.AssetPathToGUID(key);
                        }

                        if (!m_References.TryGetValue(key, out List<string> list))
                        {
                            list = new List<string>();
                            m_References.Add(key, list);
                        }

                        list.AddRange(keyValue.Value);
                    }
                }

                EditorUtility.ClearProgressBar();
                EditorApplication.update -= FindReferences;
            }
            else
            {
                EditorUtility.DisplayProgressBar("匹配资源中", string.Format("进度：{0}", finishedCount), finishedCount * 1f / ThreadCount);
            }
        }

        private Dictionary<string, List<string>> ThreadFind(ThreadData threadData)
        {
            Dictionary<string, List<string>> result = new();

            if (threadData == null)
            {
                return result;
            }

            foreach (string file in threadData.checkAssets)
            {
                string fileContent = File.ReadAllText(file);

                foreach (string assetGuid in threadData.assetsGuids)
                {
                    if (!result.TryGetValue(assetGuid, out List<string> list))
                    {
                        list = new List<string>();
                        result.Add(assetGuid, list);
                    }
                    if (Regex.IsMatch(fileContent, assetGuid))
                    {
                        list.Add(file);
                    }
                }
            }

            foreach (var file in threadData.checkCSharpScripts)
            {
                string fileContent = File.ReadAllText(file + ".meta");

                foreach (string assetGuid in threadData.assetsGuids)
                {
                    if (!result.TryGetValue(assetGuid, out List<string> list))
                    {
                        list = new List<string>();
                        result.Add(assetGuid, list);
                    }

                    if (Regex.IsMatch(fileContent, assetGuid))
                    {
                        list.Add(file);
                    }
                }
            }

            return result;
        }

        private void OnGUI()
        {
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            EditorGUILayout.BeginHorizontal();
            m_ShowNoUsed = GUILayout.Toggle(m_ShowNoUsed, string.Empty, GUILayout.Width(20));
            GUILayout.Label("无引用的资源", GUILayout.Width(600));
            EditorGUILayout.EndHorizontal();

            if (m_ShowNoUsed)
            {
                Rect rect = EditorGUILayout.BeginVertical("Button");
                foreach (var item in m_References)
                {
                    if (item.Value.Count == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        var assetPath = AssetDatabase.GUIDToAssetPath(item.Key);
                        UnityObject assetObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityObject));
                        GUILayout.Label(assetPath, GUILayout.Width(600));
                        EditorGUILayout.ObjectField("", assetObj, typeof(UnityObject), true);
                        EditorGUILayout.LabelField("内存占用：" + EditorUtility.FormatBytes(Profiler.GetRuntimeMemorySizeLong(assetObj)));

                        if (ValidateImage(assetPath))
                        {
                            GUIStyle fontStyle = new GUIStyle();
                            fontStyle.normal.textColor = new UnityColor(1, 0, 0);   //设置字体颜色  
                            fontStyle.fixedWidth = 100;
                            GUILayout.Label("bigImage", fontStyle);
                        }

                        EditorGUILayout.EndHorizontal();

                    }
                }
                EditorGUILayout.EndVertical();
            }


            EditorGUILayout.BeginHorizontal();
            m_ShowUsed = GUILayout.Toggle(m_ShowUsed, string.Empty, GUILayout.Width(20));
            GUILayout.Label("有引用的资源", GUILayout.Width(600));
            EditorGUILayout.EndHorizontal();

            if (m_ShowUsed)
            {
                var index = 0;
                foreach (var item in m_References)
                {
                    if (item.Value.Count > 0)
                    {
                        if (m_UsedHasShow.Count <= index)
                        {
                            m_UsedHasShow.Add(true);
                        }
                        var assetPath = AssetDatabase.GUIDToAssetPath(item.Key);
                        UnityObject assetObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityObject));

                        //有引用的资源
                        EditorGUILayout.BeginHorizontal();
                        m_UsedHasShow[index] = GUILayout.Toggle(m_UsedHasShow[index], string.Empty, GUILayout.Width(20));
                        GUILayout.Label(assetPath, GUILayout.Width(600));
                        EditorGUILayout.ObjectField("", assetObj, typeof(UnityObject), true, GUILayout.Width(300));
                        GUILayout.Label("内存占用：" + EditorUtility.FormatBytes(Profiler.GetRuntimeMemorySizeLong(assetObj)), GUILayout.Width(100));
                        if (ValidateImage(assetPath))
                        {
                            GUIStyle fontStyle = new GUIStyle();
                            fontStyle.normal.textColor = new UnityColor(1, 0, 0);   //设置字体颜色  
                            fontStyle.fixedWidth = 100;
                            GUILayout.Label("bigImage", fontStyle);
                        }
                        EditorGUILayout.EndHorizontal();

                        if (m_UsedHasShow[index])
                        {
                            Rect r = EditorGUILayout.BeginVertical("Button");
                            foreach (var fileName in item.Value)
                            {
                                assetPath = WuWuFramework.Utils.PathUtil.GetAssetPath(fileName);
                                assetObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityObject));
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Label(assetPath, GUILayout.Width(600));
                                EditorGUILayout.ObjectField("", assetObj, typeof(UnityObject), true, GUILayout.Width(300));
                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndVertical();
                        }
                        index++;
                    }
                }
            }

            EditorGUILayout.EndScrollView();
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

        static private void OutputToExcel()
        {
            //输出excel


            //指定默认路径
            //string path = Application.dataPath + "/" + "UIPrefabExcel.xlsx";
            //自己选择一个路径
            System.DateTime time = System.DateTime.Now;
            string timeStr = time.Year.ToString("D4") + "_" + time.Month.ToString("D2") + time.Day.ToString("D2") + "_" + time.Hour.ToString("D2") + time.Minute.ToString("D2");
            string path = EditorUtility.SaveFilePanel("Save Excel File", "", timeStr + ".xlsx", "xlsx");
            FileInfo newFile = new FileInfo(path);
            if (newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(path);
            }

            //通过ExcelPackage打开文件
            //using (ExcelPackage package = new ExcelPackage(newFile))
            //{
            //    //添加sheet
            //    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");
            //    //添加列名
            //    worksheet.Cells[1, 1].Value = "资源名字";
            //    worksheet.Cells[1, 2].Value = "被引用数量";
            //    worksheet.Cells[1, 3].Value = "内存占用";
            //    worksheet.Cells[1, 4].Value = "Atlas超过256*256";

            //    var index = 2;

            //    foreach (var item in refDic)
            //    {
            //        //if (item.Value.Count == 0)
            //        //{
            //        var assetPath = AssetDatabase.GUIDToAssetPath(item.Key);
            //        worksheet.Cells["A" + index].Value = assetPath;
            //        worksheet.Cells["B" + index].Value = item.Value.Count;
            //        Object assetObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            //        worksheet.Cells["C" + index].Value = EditorUtility.FormatBytes(Profiler.GetRuntimeMemorySizeLong(assetObj));
            //        worksheet.Cells["D" + index].Value = FindReferences.Rule01(assetPath) ? "是" : "否";
            //        index++;
            //        //}
            //    }

            //    //foreach (var item in refDic)
            //    //{
            //    //    if (item.Value.Count > 0)
            //    //    {
            //    //        var assetPath = AssetDatabase.GUIDToAssetPath(item.Key);
            //    //        worksheet.Cells["A"+ index].Value = assetPath;
            //    //        worksheet.Cells["B"+ index].Value = item.Value.Count;
            //    //        Object assetObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            //    //        worksheet.Cells["C" + index].Value = EditorUtility.FormatBytes(Profiler.GetRuntimeMemorySizeLong(assetObj));
            //    //        index++;
            //    //    }
            //    //}

            //    package.Save();
            //}
        }

    }
}