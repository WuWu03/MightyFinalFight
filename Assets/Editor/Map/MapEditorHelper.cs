using GameFrameWork.Editor;
using GameFrameWork.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class MapEditorHelper
{
    public static GUIStyle SelectButtonOnStyle
    {
        get
        {
            return m_SelectButtonOnStyle;
        }
    }

    public static GUIStyle SelectButtonStyle
    {
        get
        {
            return m_SelectButtonStyle;
        }
    }

    public static Texture2D Texture
    {
        get
        {
            return m_Textrue;
        }
    }

    public static List<MapEditorConfigData.MoveArea> MoveAreas
    {
        get
        {
            return m_CurrData.ListMovePoints;
        }
    }

    public static int Id
    {
        get
        {
            return m_CurrData.Id;
        }
        set
        {
            m_CurrData.Id = value;
        }
    }

    public static string SceneName
    {
        get
        {
            return m_CurrData.SceneName;
        }
        set
        {
            m_CurrData.SceneName = value;
        }
    }

    public static int StageIndex
    {
        get
        {
            return m_CurrData.StageIndex;
        }
        set
        {
            m_CurrData.StageIndex = value;
        }
    }

    public static int Level
    {
        get
        {
            return m_CurrData.Level;
        }
        set
        {
            m_CurrData.Level = value;
        }
    }

    public static string StageColor
    {
        get
        {
            return m_CurrData.StageColor;
        }
        set
        {
            m_CurrData.StageColor = value;
        }
    }

    public static int StageShowColor
    {
        get
        {
            return m_CurrData.StageShowColor;
        }
        set
        {
            m_CurrData.StageShowColor = value;
        }
    }
    public static List<int> ListTaskId
    {
        get
        {
            return m_CurrData.ListTaskId;
        }
    }

    public static List<StageConfigData.BGM> ListBGM
    {
        get
        {
            return m_CurrData.ListBGM;
        }
    }

    public static List<StageConfigData.SceneBuilding> listSceneBuilding
    {
        get
        {
            return m_CurrData.ListSceneBuilding;
        }
    }

    public static string mapPath
    {
        get
        {
            return m_MapEditorConfig.MapPath;
        }
    }

    public static float scale
    {
        get
        {
            return m_Scale;
        }
        set
        {
            m_Scale = value;
        }
    }

    public static float scrollX
    {
        get
        {
            return m_ScrollX;
        }
        set
        {
            m_ScrollX = value;
        }
    }

    public static float normalSize
    {
        get
        {
            return m_NormalSize;
        }
        set
        {
            m_NormalSize = value;
        }
    }

    public static float normalSizeMinimum
    {
        get
        {
            return m_NormalSizeMinimum;
        }
    }

    public static Vector2 currPos
    {
        get
        {
            return m_CurrData.CurrPos;
        }
    }

    public static Vector2 initPos
    {
        get
        {
            return m_CurrData.InitPos;
        }
    }

    public static void InitConfig()
    {
        string fileName = "MapEditorConfig";
        string ext = ".asset";
        string path = Application.dataPath + "/Editor/Config/";

        if (!File.Exists(path + fileName + ext))
        {
            GameFrameWork.Editor.EditorUtil.CreateConfigData<MapEditorConfig, MapEditorConfigData>(fileName, ext, path);
        }

        if (m_MapEditorConfig == null)
        {
            m_MapEditorConfig = AssetDatabase.LoadAssetAtPath<MapEditorConfig>("Assets/Editor/Config/" + fileName + ext);

            if (!Directory.Exists(m_MapEditorConfig.MapPath))
            {
                m_MapEditorConfig.MapPath = string.Empty;
            }

            m_MapEditorConfig.MapPath = "Assets/ArtResources/Textures/Stage/";
        }

        for (int i = 0; i < m_MapEditorConfig.Datas.Count; i++)
        {
            string mapTextureName = Path.GetFileName(m_MapEditorConfig.Datas[i].MapPath);
            string mapTexturePath = Path.GetDirectoryName(m_MapEditorConfig.Datas[i].MapPath).Replace("\\", "/") + "/";

            if(mapTexturePath != m_MapEditorConfig.MapPath)
            {
                m_MapEditorConfig.Datas[i].MapPath = mapTexturePath + mapTextureName;
            }
        }

        m_SelectButtonOnStyle = new GUIStyle("flow node 1");
        m_SelectButtonOnStyle.stretchWidth = true;
        m_SelectButtonOnStyle.alignment = TextAnchor.MiddleCenter;
        m_SelectButtonOnStyle.contentOffset = new Vector2(0, -15f);
        m_SelectButtonOnStyle.fixedHeight = 15f;

        m_SelectButtonStyle = new GUIStyle("flow node 0");
        m_SelectButtonStyle.stretchWidth = true;
        m_SelectButtonStyle.alignment = TextAnchor.MiddleCenter;
        m_SelectButtonStyle.contentOffset = new Vector2(0, -15f);
        m_SelectButtonStyle.fixedHeight = 15f;
    }

    public static void LoadTexture(string path)
    {
        if (m_CurrData != null && m_CurrData.MapPath.Equals(path))
        {
            return;
        }

        m_Textrue = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        m_CurrData = m_MapEditorConfig.GetData(path);

        if (m_CurrData == null)
        {
            m_CurrData = new MapEditorConfigData();
            m_CurrData.MapPath = path;
            m_CurrData.ListMovePoints = new List<MapEditorConfigData.MoveArea>();
            m_CurrData.ListTaskId = new List<int>();
            m_CurrData.ListBGM = new List<StageConfigData.BGM>();
            m_CurrData.ListSceneBuilding = new List<StageConfigData.SceneBuilding>();
            m_MapEditorConfig.AddData(m_CurrData);
        }

        m_NormalSizeMinimum = (float)m_Textrue.height / 100 / 2;
        m_CurrData.Width = m_Textrue.width;
        m_CurrData.Height = m_Textrue.height;
    }

    public static void SetInitPos(Vector2 pos, bool convert = false)
    {
        m_CurrData.InitPos = convert ? ConvertPos(pos) : pos;
    }

    public static void SetCurrPos(Vector2 pos, bool convert = false)
    {
        m_CurrData.CurrPos = convert ? ConvertPos(pos) : pos;
    }

    public static void AddMovePoint(Vector2 pos, Color color)
    {
        Vector2 realPos = ConvertPos(pos);
        MapEditorConfigData.MoveArea area = new MapEditorConfigData.MoveArea();
        area.Point = realPos;
        area.Color = color;
        m_CurrData.ListMovePoints.Add(area);
    }

    public static void SetMovePoint(int index, Vector2 pos)
    {
        m_CurrData.ListMovePoints[index].Point = pos;
    }

    public static Vector2 GetTextureSize(bool ignoreScale = false)
    {
        Vector2 texSize = Vector2.zero;

        if (m_Textrue == null)
        {
            return texSize;
        }

        texSize.x = m_Textrue.width;
        texSize.y = m_Textrue.height;
        return texSize * (ignoreScale ? 1f : m_Scale);
    }

    public static Vector2 GetScreenSize(bool ignoreScale = false)
    {
        Vector2 screenSize = Vector2.zero;
        Vector2 viewSize = GetMainGameViewSize();
        screenSize.y = m_NormalSize * 2 * 100f;
        screenSize.x = viewSize.x * screenSize.y / viewSize.y;
        return screenSize * (ignoreScale ? 1 : m_Scale);
    }

    public static Rect GetTextureRect()
    {
        Vector2 texSize = GetTextureSize();
        Vector2 screenSize = GetScreenSize();
        m_TextureRect.x = 0 - m_ScrollX;
        m_TextureRect.y = (Mathf.Max(screenSize.y - texSize.y, 0)) / 2;
        m_TextureRect.width = texSize.x;
        m_TextureRect.height = texSize.y;

        return m_TextureRect;
    }

    public static Rect GetCurrPointRect()
    {
        Vector2 currPos = RevertPos(m_CurrData.CurrPos);

        m_CurrPointRect.x = currPos.x;
        m_CurrPointRect.y = currPos.y;
        m_CurrPointRect.width = 5;
        m_CurrPointRect.height = 5;

        return m_CurrPointRect;
    }

    public static Rect GetInitPointRect()
    {
        Vector2 initPos = RevertPos(m_CurrData.InitPos);

        m_InitPointRect.x = initPos.x;
        m_InitPointRect.y = initPos.y;
        m_InitPointRect.width = 5;
        m_InitPointRect.height = 5;

        return m_InitPointRect;
    }

    public static Rect ConvertMoveRect(Rect rect)
    {
        Vector2 pos = Vector2.zero;
        Vector2 texSize = GetTextureSize(true);

        pos.x = rect.x - texSize.x / 2;
        pos.y = -(rect.y - texSize.y / 2);

        return new Rect(pos.x, pos.y, rect.width, rect.height);
    }

    public static Rect RevertMoveRect(Rect rect)
    {
        Vector2 pos = Vector2.zero;
        Vector2 texSize = GetTextureSize(true);

        pos.x = rect.x + texSize.x / 2;
        pos.y = -rect.y + texSize.y / 2;

        return new Rect(pos.x, pos.y, rect.width, rect.height);
    }

    public static Rect GetDrawMoveRect(Rect rect)
    {
        Vector2 texSize = GetTextureSize();
        Vector2 screenSize = GetScreenSize();

        float offestY = (screenSize.y - texSize.y) / 2;
        float x = rect.x * m_Scale - m_ScrollX;
        float y = rect.y * m_Scale  + offestY;
        float width = rect.width * m_Scale;
        float height = rect.height * m_Scale;

        return new Rect(x, y, width, height);
    }

    public static Vector2 ConvertPos(Vector2 pos)
    {
        Vector2 ret = Vector2.zero;
        Vector2 texSize = GetTextureSize();
        Vector2 screenSize = GetScreenSize();
        ret.x = pos.x - texSize.x / 2;
        ret.y = -(pos.y - screenSize.y / 2);
        ret /= m_Scale;

        return ret;
    }

    public static Vector2 RevertPos(Vector2 pos)
    {
        Vector2 ret = pos * m_Scale;
        Vector2 texSize = GetTextureSize();
        Vector2 screenSize = GetScreenSize();
        ret.x = texSize.x / 2 + ret.x - m_ScrollX;
        ret.y = screenSize.y / 2 - ret.y;

        return ret;
    }

    public static bool HasData()
    {
        if(m_MapEditorConfig == null || m_MapEditorConfig.Datas == null || m_MapEditorConfig.Datas.Count < 1)
        {
            return false;
        }

        return true;
    }
    public static void Export()
    {
        if (!File.Exists(EditorPathUtil.configDataFullPath + "StageConfigData.asset"))
        {
            GameFrameWork.Editor.EditorUtil.CreateConfigData<StageConfig, StageConfigData>("StageConfigData", ".asset", EditorPathUtil.configDataPath);
        }

        StageConfig stageConfig = AssetDatabase.LoadAssetAtPath<StageConfig>(EditorPathUtil.configDataPath + "StageConfigData.asset");

        if(stageConfig.Datas == null)
        {
            stageConfig.Datas = new List<StageConfigData>();
        }
        else
        {
            stageConfig.Datas.Clear();
        }

        m_MapEditorConfig.Datas.Sort();

        for (int i = 0; i < m_MapEditorConfig.Datas.Count; i++)
        {
            MapEditorConfigData configData = m_MapEditorConfig.Datas[i];
            StageConfigData data = new StageConfigData();
            data.Id = configData.Id;
            data.Name = configData.SceneName;
            data.SceneName = configData.SceneName;
            data.StageIndex = configData.StageIndex;
            data.Level = configData.Level;
            data.Width = configData.Width;
            data.Height = configData.Height;
            data.InitPos = new Vector2Int((int)configData.InitPos.x, (int)configData.InitPos.y);
            data.TaskIDs = configData.ListTaskId.ToArray();
            data.BGMs = configData.ListBGM.ToArray();
            data.SceneBuildings = configData.ListSceneBuilding.ToArray();
            data.MovePoints = new Vector2Int[configData.ListMovePoints.Count];
            data.StageColor = configData.StageColor;
            data.StageShowColor = configData.StageShowColor;

            for (int j = 0; j < data.MovePoints.Length; j++)
            {
                int x = (int)configData.ListMovePoints[j].Point.x;
                int y = (int)configData.ListMovePoints[j].Point.y;
                data.MovePoints[j] = new Vector2Int(x, y);
            }

            stageConfig.Datas.Add(data);
        }

        UnityEditor.EditorUtility.SetDirty(stageConfig);
        EditorWindow.focusedWindow.ShowNotification(new GUIContent("导出配置成功!"));
    }

    private static Vector2 GetMainGameViewSize()
    {
        if (m_GameViewSize != Vector2.zero)
        {
            return m_GameViewSize;
        }

        System.Type t = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = t.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object pos = GetSizeOfMainGameView.Invoke(null, null);
        m_GameViewSize = (Vector2)pos;
        return m_GameViewSize;
    }

    public static bool IsPointInTexture(Vector2 point)
    {
        return GetTextureRect().Contains(point);
    }

    public static void Dispose()
    {
        UnityEditor.EditorUtility.SetDirty(m_MapEditorConfig);
        m_Textrue = null;
        m_MapEditorConfig = null;
        m_CurrData = null;
    }

    private static GUIStyle m_SelectButtonOnStyle = null;
    private static GUIStyle m_SelectButtonStyle = null;
    private static MapEditorConfig m_MapEditorConfig = null;
    private static MapEditorConfigData m_CurrData = null;
    private static Texture2D m_Textrue = null;
    private static float m_Scale = 1;
    private static float m_ScrollX = 0;
    private static float m_NormalSize = 1;
    private static float m_NormalSizeMinimum = 0f;
    private static Rect m_TextureRect = Rect.zero;
    private static Rect m_CurrPointRect = Rect.zero;
    private static Rect m_InitPointRect = Rect.zero;
    private static Vector2 m_GameViewSize = Vector2.zero;
}