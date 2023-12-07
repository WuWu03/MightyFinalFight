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
                if (Path.GetExtension(files[i]).Equals(".meta"))
                {
                    continue;
                }

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
            int pos1Index = i;
            int pos2Index = i + 1 >= MapEditorHelper.MoveAreas.Count ? 0 : i + 1;

            Vector2 pos1 = MapEditorHelper.RevertPos(MapEditorHelper.MoveAreas[pos1Index].Point);
            Vector2 pos2 = MapEditorHelper.RevertPos(MapEditorHelper.MoveAreas[pos2Index].Point);
            Color color = MapEditorHelper.MoveAreas[i].Color;

            int symbleX = pos1.x > MapEditorHelper.GetTextureSize().x / 2 ? -1 : 0;
            EditorGUI.DrawRect(new Rect(pos1.x + 5f * symbleX, pos1.y - 2.5f, 5f, 5f), color);
            Handles.color = Color.green;   
            Handles.DrawLine(pos1, pos2);
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
        Vector2 texSize = MapEditorHelper.GetTextureSize();
        Vector2 screenSize = MapEditorHelper.GetScreenSize();

        GUILayout.FlexibleSpace();
        int select = EditorGUILayout.Popup("当前地图", m_CurrMap, m_MapNames);

        if(select != m_CurrMap)
        {
            m_CurrMap = select;
            m_CurrMoveArea = 0;
            MapEditorHelper.ScrollX = 0;
            MapEditorHelper.Scale = 1;
            MapEditorHelper.NormalSize = 1;
            MapEditorHelper.LoadTexture(m_MapFiles[m_CurrMap]);
            SetWindowSize();
            SetMapNames();
        }

        MapEditorHelper.Id = EditorGUILayout.IntField("地图Id", MapEditorHelper.Id);
        MapEditorHelper.StageIndex = EditorGUILayout.IntField("关卡索引", MapEditorHelper.StageIndex);
        MapEditorHelper.Level = EditorGUILayout.IntField("小节", MapEditorHelper.Level);
        MapEditorHelper.SceneName = EditorGUILayout.TextField("地图名称", MapEditorHelper.SceneName);
        EditorGUILayout.FloatField("地图宽", MapEditorHelper.Texture.width);
        EditorGUILayout.FloatField("地图高", MapEditorHelper.Texture.height);

        MapEditorHelper.StageColor = EditorGUILayout.TextField("关卡色调", MapEditorHelper.StageColor);
        MapEditorHelper.StageShowColor = EditorGUILayout.IntField("关卡面板色调", MapEditorHelper.StageShowColor);

        Vector2 currPos = EditorGUILayout.Vector2Field("当前坐标", MapEditorHelper.CurrPos);
        Vector2 initPos = EditorGUILayout.Vector2Field("出生坐标", MapEditorHelper.InitPos);
        MapEditorHelper.SetCurrPos(currPos);
        MapEditorHelper.SetInitPos(initPos);

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

        if (MapEditorHelper.MoveAreas.Count > 0)
        {
            GUILayout.BeginHorizontal();
            m_CurrMoveArea = EditorGUILayout.Popup("行走区域", m_CurrMoveArea, m_MapAreaNames);

            if (GUILayout.Button("x", GUILayout.Width(20f)))
            {
                MapEditorHelper.MoveAreas.RemoveAt(m_CurrMoveArea);

                if (MapEditorHelper.MoveAreas.Count < 1)
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

            Vector2 movePoint = EditorGUILayout.Vector2Field("", MapEditorHelper.MoveAreas[m_CurrMoveArea].Point);
            MapEditorHelper.SetMovePoint(m_CurrMoveArea, movePoint);
        }

        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < m_TabNames.Length; i++)
        {
            if (GUILayout.Button(m_TabNames[i], i == m_CurrPage ? MapEditorHelper.SelectButtonOnStyle : MapEditorHelper.SelectButtonStyle))
            {
                m_CurrPage = i;
                break;
            }
        }
        EditorGUILayout.EndHorizontal();

        if(m_CurrPage == 0)
        {
            TaskConfigGUI();
        }
        else if(m_CurrPage == 1)
        {
            BGMConfigGUI();
        }
        else if(m_CurrPage == 2)
        {
            SceneBuildingConfigGUI();
        }

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
        m_ScollPosTask = GUILayout.BeginScrollView(m_ScollPosTask, GUILayout.Width(position.width), GUILayout.Height(200));

        for (int i = 0; i < MapEditorHelper.ListTaskId.Count; i++)
        {
            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
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
        m_ScollPosBGM = GUILayout.BeginScrollView(m_ScollPosBGM, GUILayout.Width(position.width), GUILayout.Height(200));

        for (int i = 0; i < MapEditorHelper.ListBGM.Count; i++)
        {
            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
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
                    }
                }

                GUILayout.EndHorizontal();
                MapEditorHelper.ListBGM[i].ClipName = EditorGUILayout.TextField("音频资源名称", MapEditorHelper.ListBGM[i].ClipName);
                MapEditorHelper.ListBGM[i].IsLoop = EditorGUILayout.Toggle("是否循环播放", MapEditorHelper.ListBGM[i].IsLoop);
                MapEditorHelper.ListBGM[i].Volume = EditorGUILayout.Slider("音量大小", MapEditorHelper.ListBGM[i].Volume, 0, 1);
                MapEditorHelper.ListBGM[i].LerpTime = EditorGUILayout.FloatField("过渡时间", MapEditorHelper.ListBGM[i].LerpTime);
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

    private void SceneBuildingConfigGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Space(5);
        GUILayout.Label("场景物体配置");
        m_ScollPosSceneObject = GUILayout.BeginScrollView(m_ScollPosSceneObject, GUILayout.Width(position.width), GUILayout.Height(200));

        for (int i = 0; i < MapEditorHelper.ListSceneBuilding.Count; i++)
        {
            GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Label((i + 1) + ".");
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("×"))//删除本条数据
                {
                    if (UnityEditor.EditorUtility.DisplayDialog("提示", "确认移除本条配置吗？", "确认", "取消"))
                    {
                        MapEditorHelper.ListSceneBuilding.RemoveAt(i);
                        for (int j = 0; j < MapEditorHelper.ListSceneBuilding.Count; j++)
                        {
                            MapEditorHelper.ListSceneBuilding[j].Id = j + 1;
                        }
                        return;
                    }
                }

                GUILayout.EndHorizontal();

                MapEditorHelper.ListSceneBuilding[i].SceneObjType = (StageConfigData.SceneObjType)EditorGUILayout.EnumPopup("类型", MapEditorHelper.ListSceneBuilding[i].SceneObjType);
                MapEditorHelper.ListSceneBuilding[i].Name = EditorGUILayout.TextField("名称", MapEditorHelper.ListSceneBuilding[i].Name);
                MapEditorHelper.ListSceneBuilding[i].Pos = EditorGUILayout.Vector2IntField("位置", MapEditorHelper.ListSceneBuilding[i].Pos);

                if (MapEditorHelper.ListSceneBuilding[i].SceneObjType == StageConfigData.SceneObjType.Trap)
                {
                    MapEditorHelper.ListSceneBuilding[i].TriggerSize = EditorGUILayout.Vector2Field("触发器尺寸", MapEditorHelper.ListSceneBuilding[i].TriggerSize);
                    MapEditorHelper.ListSceneBuilding[i].TriggerOffest = EditorGUILayout.Vector2Field("触发器偏移", MapEditorHelper.ListSceneBuilding[i].TriggerOffest);
                }
                MapEditorHelper.ListSceneBuilding[i].AssetName = EditorGUILayout.TextField("资源路径", MapEditorHelper.ListSceneBuilding[i].AssetName);
                GUILayout.EndVertical();
            });
        }

        GUILayout.EndScrollView();

        if (GUILayout.Button("增加场景物体配置"))
        {
            StageConfigData.SceneBuilding sceneObj = new StageConfigData.SceneBuilding();
            sceneObj.Id = MapEditorHelper.ListSceneBuilding.Count + 1;
            MapEditorHelper.ListSceneBuilding.Add(sceneObj);
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
            if (!MapEditorHelper.IsPointInTexture(e.mousePosition))
            {
                return;
            }

            MapEditorHelper.SetCurrPos(e.mousePosition, true);
        }
    }

    private void MenuContextCallback(object args)
    {
        int operation = (int)args;
        if (operation == 0)
        {
            MapEditorHelper.SetInitPos(m_Mouse1Pos, true);
        }
        else if(operation == 1)
        {
            if (!MapEditorHelper.IsPointInTexture(m_Mouse1Pos))
            {
                return;
            }

            float r = UnityEngine.Random.Range(0f, 1f);
            float g = UnityEngine.Random.Range(0f, 1f);
            float b = UnityEngine.Random.Range(0f, 1f);
            MapEditorHelper.AddMovePoint(m_Mouse1Pos, new Color(r, g, b, 1));
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
        float height = Mathf.Max(screenSize.y, texSize.y) + 660;

        minSize = new Vector2(width, height);
        maxSize = minSize;  
    }

    private void Export()
    {
        MapEditorHelper.Export();
    }

    private Vector2 m_ScollPosTask = Vector2.zero;
    private Vector2 m_ScollPosBGM = Vector2.zero;
    private Vector2 m_ScollPosSceneObject = Vector2.zero;
    private Vector2 m_Mouse1Pos = Vector2.zero;
    private int m_CurrMap = 0;
    private int m_CurrMoveArea = 0;
    private string[] m_MapFiles = null;
    private string[] m_MapNames = null;
    private string[] m_MapAreaNames = null;
    private float m_CameraX = 0;
    private bool m_IsMouse0Down = false;
    private Vector3[] m_ViewAreaPoints = null;
    private int m_CurrPage = 0;
    private string[] m_TabNames = new string[] { "TaskConfig", "BGMConfig", "SceneBuildingConfig" };
}
