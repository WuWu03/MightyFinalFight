using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapEditorWindow : EditorWindow
{
    private void OnEnable()
    {
        MapEditorHelper.InitConfig();
        if (m_ViewAreaPoints == null)
        {
            m_ViewAreaPoints = new Vector3[4];
            for (int i = 0; i < m_ViewAreaPoints.Length; i++)
            {
                m_ViewAreaPoints[i] = Vector3.zero;
            }
        }

        string[] files = Directory.GetFiles(MapEditorHelper.MapPath);

        if (files.Length > 0)
        {
            List<string> listMapFile = new List<string>();
            List<string> listMapName = new List<string>();

            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetExtension(files[i]).Equals(".meta")) continue;
                listMapFile.Add(files[i].Substring(files[i].IndexOf("Assets")));
                listMapName.Add(Path.GetFileNameWithoutExtension(files[i]));
            }

            m_MapNames = listMapName.ToArray();
            m_MapFiles = listMapFile.ToArray();
            MapEditorHelper.LoadTexture(m_MapFiles[0]);
            m_CurrMap = 0;;

            SetMapNames();
            SetWindowSize();
        }
    }

    private void OnDisable()
    {
        MapEditorHelper.Dispose();
    }

    private void OnGUI()
    {
        if(MapEditorHelper.Texture == null)
        {
            return;
        }

        MainGUI();
        DrawElement();
        ConfigPoint(UnityEngine.Event.current);
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void DrawElement()
    {
        Vector2 screenSize = MapEditorHelper.GetScreenSize();

        GUI.DrawTexture(MapEditorHelper.GetTextureRect(), MapEditorHelper.Texture);

        for (int i = 0; i < MapEditorHelper.MoveAreas.Count; i++)
        {
            Rect rect = MapEditorHelper.GetDrawMoveRect(MapEditorHelper.MoveAreas[i].Rect);
            Color color = MapEditorHelper.MoveAreas[i].Color;
            EditorGUI.DrawRect(rect, color);

            if (m_CurrMoveArea == i)
            {
                Vector3 pos0 = new Vector3(rect.x, rect.y, 0);
                Vector3 pos1 = new Vector3(rect.x + rect.width, rect.y, 0);
                Vector3 pos2 = new Vector3(rect.x + rect.width, rect.y + rect.height, 0);
                Vector3 pos3 = new Vector3(rect.x, rect.y + rect.height, 0);

                Handles.color = new Color(color.r, color.g, color.b, 1);
                Handles.DrawLine(pos0, pos1);
                Handles.DrawLine(pos1, pos2);
                Handles.DrawLine(pos2, pos3);
                Handles.DrawLine(pos3, pos0);
            }
        }

        EditorGUI.DrawRect(MapEditorHelper.GetCurrPointRect(), Color.red);
        EditorGUI.DrawRect(MapEditorHelper.GetInitPointRect(), Color.green);

        m_ViewAreaPoints[0].x = Mathf.Min(screenSize.x + m_CameraX, position.width) - screenSize.x;
        m_ViewAreaPoints[0].y = 0;
        m_ViewAreaPoints[1].x = Mathf.Min(screenSize.x + m_CameraX, position.width);
        m_ViewAreaPoints[1].y = 0;
        m_ViewAreaPoints[2].x = Mathf.Min(screenSize.x + m_CameraX, position.width);
        m_ViewAreaPoints[2].y = screenSize.y;
        m_ViewAreaPoints[3].x = Mathf.Min(screenSize.x + m_CameraX, position.width) - screenSize.x;
        m_ViewAreaPoints[3].y = screenSize.y;

        Handles.color = Color.yellow;
        Handles.DrawLine(m_ViewAreaPoints[0], m_ViewAreaPoints[1]);
        Handles.DrawLine(m_ViewAreaPoints[1], m_ViewAreaPoints[2]);
        Handles.DrawLine(m_ViewAreaPoints[2], m_ViewAreaPoints[3]);
        Handles.DrawLine(m_ViewAreaPoints[3], m_ViewAreaPoints[0]);
    }

    private void MainGUI()
    {
        Debug.Log(position.position);
        Vector2 texSize = MapEditorHelper.GetTextureSize();
        Vector2 screenSize = MapEditorHelper.GetScreenSize();

        GUILayout.FlexibleSpace();

        int select = EditorGUILayout.Popup("当前地图", m_CurrMap, m_MapNames);

        if(select != m_CurrMap)
        {
            m_CurrMap = select;
            MapEditorHelper.ScrollX = 0;
            MapEditorHelper.Scale = 1;
            MapEditorHelper.NormalSize = 1;
            MapEditorHelper.LoadTexture(m_MapFiles[m_CurrMap]);
            SetWindowSize();
        }

        MapEditorHelper.Id = EditorGUILayout.IntField("地图Id", MapEditorHelper.Id);
        MapEditorHelper.SceneName = EditorGUILayout.TextField("地图名称", MapEditorHelper.SceneName);
        EditorGUILayout.Vector2Field("当前坐标", MapEditorHelper.CurrPos);
        EditorGUILayout.Vector2Field("出生坐标", MapEditorHelper.InitPos);
        float scale = EditorGUILayout.Slider("地图缩放", MapEditorHelper.Scale, 1, 2);
        float normalSize = EditorGUILayout.Slider("正交尺寸", MapEditorHelper.NormalSize, MapEditorHelper.NormalSizeMinimum, 10);
        m_CameraX = EditorGUILayout.Slider("相机位置", m_CameraX, 0, Mathf.Max(texSize.x - screenSize.x, 0));

        if (scale != MapEditorHelper.Scale || normalSize != MapEditorHelper.NormalSize)
        {
            MapEditorHelper.Scale = scale;
            MapEditorHelper.NormalSize = normalSize;
            SetWindowSize();
        }

        if (screenSize.x + m_CameraX > position.width)
        {
            MapEditorHelper.ScrollX = screenSize.x + m_CameraX - position.width;
        }
        else
        {
            MapEditorHelper.ScrollX = 0;
        }

        if(MapEditorHelper.MoveAreas.Count > 0)
        {
            GUILayout.BeginHorizontal();
            m_CurrMoveArea = EditorGUILayout.Popup("行走区域", m_CurrMoveArea, m_MapAreaNames);

            if (GUILayout.Button("x", GUILayout.Width(20f)))
            {
                MapEditorHelper.MoveAreas.RemoveAt(m_CurrMoveArea);

                if(MapEditorHelper.MoveAreas.Count < 1)
                {
                    m_CurrMoveArea = 0;
                    return;
                }

                if (m_CurrMoveArea >= MapEditorHelper.MoveAreas.Count)
                {
                    m_CurrMoveArea = Mathf.Max(MapEditorHelper.MoveAreas.Count - 1, 0);
                }

                SetMapNames();
            }
            GUILayout.EndHorizontal();

            MapEditorHelper.MoveAreas[m_CurrMoveArea].RealRect = MapEditorHelper.ConvertMoveRect(MapEditorHelper.MoveAreas[m_CurrMoveArea].Rect);
            MapEditorHelper.MoveAreas[m_CurrMoveArea].RealRect = EditorGUILayout.RectField(MapEditorHelper.MoveAreas[m_CurrMoveArea].RealRect);
            MapEditorHelper.MoveAreas[m_CurrMoveArea].Rect = MapEditorHelper.RevertMoveRect(MapEditorHelper.MoveAreas[m_CurrMoveArea].RealRect);
        }

        EditorGUILayout.BeginHorizontal();
        TaskConfigGUI();
        BGMConfigGUI();
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("导出配置"))
        {
            Export();
        }
    }

    private void TaskConfigGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.Label("场景任务配置");
        m_ScollPosTask = GUILayout.BeginScrollView(m_ScollPosTask, GUILayout.Width(position.width / 2), GUILayout.Height(150));

        for (int i = 0; i < MapEditorHelper.ListTaskId.Count; i++)
        {
            GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Label((i + 1) + ".");
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("×"))//删除本条数据
                {
                    if (UnityEditor.EditorUtility.DisplayDialog("提示", "确认移除本条配置吗？", "确认", "取消"))
                    {
                        MapEditorHelper.ListTaskId.RemoveAt(i);
                        return;
                    }
                }

                GUILayout.EndHorizontal();

                MapEditorHelper.ListTaskId[i] = EditorGUILayout.IntField("任务id", MapEditorHelper.ListTaskId[i]);
                GUILayout.EndVertical();
            });
        }

        GUILayout.EndScrollView();

        if (GUILayout.Button("增加任务"))
        {
            MapEditorHelper.ListTaskId.Add(0);
        }

        GUILayout.EndVertical();
    }

    private void BGMConfigGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.Label("场景BGM配置");
        m_ScollPosBGM = GUILayout.BeginScrollView(m_ScollPosBGM, GUILayout.Width(position.width / 2), GUILayout.Height(150));

        for (int i = 0; i < MapEditorHelper.ListBGM.Count; i++)
        {
            GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Label((i + 1) + ".");
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("×"))//删除本条数据
                {
                    if (UnityEditor.EditorUtility.DisplayDialog("提示", "确认移除本条配置吗？", "确认", "取消"))
                    {
                        MapEditorHelper.ListBGM.RemoveAt(i);
                        return;
                    }
                }

                GUILayout.EndHorizontal();
                MapEditorHelper.ListBGM[i].ClipName = EditorGUILayout.TextField("音频资源名称", MapEditorHelper.ListBGM[i].ClipName);
                MapEditorHelper.ListBGM[i].IsLoop = EditorGUILayout.Toggle("是否循环播放", MapEditorHelper.ListBGM[i].IsLoop);
                MapEditorHelper.ListBGM[i].Volume = EditorGUILayout.Slider("音量大小", MapEditorHelper.ListBGM[i].Volume, 0, 1);
                MapEditorHelper.ListBGM[i].LerpTime = EditorGUILayout.FloatField("过渡时间", MapEditorHelper.ListBGM[i].Volume);
                GUILayout.EndVertical();
            });
        }

        GUILayout.EndScrollView();

        if (GUILayout.Button("增加BGM"))
        {
            MapEditorHelper.ListBGM.Add(new StageConfigData.BGM());
        }

        GUILayout.EndVertical();
    }

    private void ConfigPoint(UnityEngine.Event e)
    {
        if (e.type == EventType.MouseDown)
        {
            if (e.button == 1)
            {
                if (!MapEditorHelper.IsPointInTexture(e.mousePosition)) return;
                m_Mouse1Pos = e.mousePosition;
                GenericMenu menu = new GenericMenu();
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("出生点位置"), false, MenuContextCallback, 0);
                menu.AddItem(new GUIContent("增加行走区域"), false, MenuContextCallback, 1);
                //menu.AddItem(new GUIContent("更改名称"), false, LeftMenuContextCallback, 1);
                //menu.AddItem(new GUIContent("更改ID"), false, LeftMenuContextCallback, 2);
                //menu.AddItem(new GUIContent("添加行为树"), false, LeftMenuContextCallback, 3);
                menu.AddSeparator("");
                menu.ShowAsContext();
            }
            else if (e.button == 0)
            {
                m_IsMouse0Down = true;
            }
        }
        else if (e.type == EventType.MouseUp)
        {
            m_IsMouse0Down = false;
        }

        if (m_IsMouse0Down)
        {
            if (!MapEditorHelper.IsPointInTexture(e.mousePosition)) return;
            MapEditorHelper.CurrPos = e.mousePosition;
        }
    }

    private void MenuContextCallback(object args)
    {
        int operation = (int)args;
        if (operation == 0)
        {   
            MapEditorHelper.InitPos = m_Mouse1Pos;
        }
        else if(operation == 1)
        {
            Rect rect = new Rect(m_Mouse1Pos.x, m_Mouse1Pos.y, 100, 100);
            float r = UnityEngine.Random.Range(0f, 1f);
            float g = UnityEngine.Random.Range(0f, 1f);
            float b = UnityEngine.Random.Range(0f, 1f);
            MapEditorHelper.MoveAreas.Add(new MapEditorConfigData.MoveArea()
            {
                Rect = rect,
                Color = new Color(r, g, b, 0.3f)
            });
            SetMapNames();
        }
    }

    private void SetMapNames()
    {
        m_MapAreaNames = new string[MapEditorHelper.MoveAreas.Count];

        for (int i = 0; i < MapEditorHelper.MoveAreas.Count; i++)
        {
            m_MapAreaNames[i] = (i + 1).ToString();
        }
    }

    private void SetWindowSize()
    {
        Vector2 texSize = MapEditorHelper.GetTextureSize(true);
        Vector2 screenSize = MapEditorHelper.GetScreenSize();

        float width = Mathf.Max(screenSize.x, texSize.x);
        float height = Mathf.Max(screenSize.y, texSize.y) + 500;

        minSize = new Vector2(width, height);
        maxSize = minSize;  
    }

    private void Export()
    {
        MapEditorHelper.Export();
    }

    private Vector2 m_ScollPosTask = Vector2.zero;
    private Vector2 m_ScollPosBGM = Vector2.zero;
    private Vector2 m_Mouse1Pos = Vector2.zero;
    private int m_CurrMap = 0;
    private int m_CurrMoveArea = 0;
    private string[] m_MapFiles = null;
    private string[] m_MapNames = null;
    private string[] m_MapAreaNames = null;
    private float m_CameraX = 0;
    private bool m_IsMouse0Down = false;
    private Vector3[] m_ViewAreaPoints = null;
}
