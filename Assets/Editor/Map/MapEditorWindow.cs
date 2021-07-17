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
        if (m_ViewAreaPoints == null)
        {
            m_ViewAreaPoints = new Vector3[4];
            for (int i = 0; i < m_ViewAreaPoints.Length; i++)
            {
                m_ViewAreaPoints[i] = Vector3.zero;
            }
        }

        string[] files = Directory.GetFiles(MapEditorUtil.MAP_PATH);

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
            m_CurrMap = 0;

            MapEditorUtil.LoadTexture(m_MapFiles[0]);
            SetWindowSize();
        }
    }

    private void OnGUI()
    {
        if(MapEditorUtil.Texture == null)
        {
            return;
        }

        DrawElement();
        MainGUI();  
        ConfigPoint(UnityEngine.Event.current);
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void DrawElement()
    {
        Vector2 screenSize = MapEditorUtil.GetScreenSize();

        GUI.DrawTexture(MapEditorUtil.GetTextureRect(), MapEditorUtil.Texture);
        EditorGUI.DrawRect(MapEditorUtil.GetCurrPointRect(), Color.red);
        EditorGUI.DrawRect(MapEditorUtil.GetCurrInitPointRect(), Color.green);

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
        Vector2 texSize = MapEditorUtil.GetTextureSize();
        Vector2 screenSize = MapEditorUtil.GetScreenSize();

        GUILayout.FlexibleSpace();

        int select = EditorGUILayout.Popup("当前地图", m_CurrMap, m_MapNames);

        if(select != m_CurrMap)
        {
            m_CurrMap = select;
            MapEditorUtil.LoadTexture(m_MapFiles[m_CurrMap]);
            SetWindowSize();
        }

        EditorGUILayout.Vector2Field("当前坐标", m_CurrMapPos);

        float scale = EditorGUILayout.Slider("地图缩放", MapEditorUtil.Scale, 0, 10);
        MapEditorUtil.NormalSize = EditorGUILayout.Slider("正交尺寸", MapEditorUtil.NormalSize, 0, 10);
        m_CameraX = EditorGUILayout.Slider("相机位置", m_CameraX, 0, Mathf.Max(texSize.x - screenSize.x, 0));

        if (scale != MapEditorUtil.Scale)
        {
            MapEditorUtil.Scale = scale;
            position = new Rect(position.x, position.y, screenSize.x, screenSize.y + 300);
        }

        if (screenSize.x + m_CameraX > position.width)
            MapEditorUtil.ScrollX = screenSize.x + m_CameraX - position.width;
        else
            MapEditorUtil.ScrollX = 0;
    }

    private void ConfigPoint(UnityEngine.Event e)
    {
        if (e.type == EventType.MouseDown)
        {
            if (e.button == 1)
            {
                if (!MapEditorUtil.IsPointInTexture(e.mousePosition)) return;
                m_InitMousePosition = e.mousePosition;
                GenericMenu menu = new GenericMenu();
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("出生点位置"), false, MenuContextCallback, 0);
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
            if (!MapEditorUtil.IsPointInTexture(e.mousePosition)) return;
            MapEditorUtil.CurrPos = e.mousePosition;
            Vector2 texSize = MapEditorUtil.GetTextureSize();
            Vector2 screenSize = MapEditorUtil.GetScreenSize();
            m_CurrMapPos.x = e.mousePosition.x - texSize.x / 2;
            m_CurrMapPos.y = -(e.mousePosition.y - screenSize.y / 2);
            m_CurrMapPos /= MapEditorUtil.Scale;
        }
    }

    private void MenuContextCallback(object args)
    {
        int operation = (int)args;
        if(operation == 0)
        {
            MapEditorUtil.CurrInitPos = m_InitMousePosition;
            Vector2 texSize = MapEditorUtil.GetTextureSize();
            Vector2 screenSize = MapEditorUtil.GetScreenSize();
            m_CurrMapInitPos.x = m_InitMousePosition.x - texSize.x / 2;
            m_CurrMapInitPos.y = -(m_InitMousePosition.y - screenSize.y / 2);
            m_CurrMapInitPos /= MapEditorUtil.Scale;
        }
    }

    private void SetWindowSize()
    {
        Vector2 texSize = MapEditorUtil.GetTextureSize();
        Vector2 screenSize = MapEditorUtil.GetScreenSize();

        position = new Rect(position.x, position.y, texSize.x, screenSize.y + 300);
        minSize = new Vector2(texSize.x, screenSize.y + 300);
    }

    private Vector2 m_InitMousePosition = Vector2.zero;
    private int m_CurrMap = 0;
    private string[] m_MapFiles = null;
    private string[] m_MapNames = null;
    private float m_CameraX = 0;
    private bool m_IsMouse0Down = false;
    private Vector3[] m_ViewAreaPoints = null;
    private Vector2 m_CurrMapPos = Vector2.zero;
    private Vector2 m_CurrMapInitPos = Vector2.zero;
}
