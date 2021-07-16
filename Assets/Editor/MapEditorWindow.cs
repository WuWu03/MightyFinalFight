using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapEditorWindow : EditorWindow
{

    private void OnGUI()
    {
        LoadTexture();

        Vector2 texSize = GetTextureSize();
        Vector2 screenSize = GetScreenSize();

        GUI.DrawTexture(GetTextureRect(), m_Textrue);

        m_ViewAreaPoints[0].x = Mathf.Min(screenSize.x + m_CameraX, position.width) - screenSize.x;
        m_ViewAreaPoints[0].y = 0;
        m_ViewAreaPoints[1].x = Mathf.Min(screenSize.x + m_CameraX, position.width);
        m_ViewAreaPoints[1].y = 0;
        m_ViewAreaPoints[2].x = Mathf.Min(screenSize.x + m_CameraX, position.width);
        m_ViewAreaPoints[2].y = screenSize.y;
        m_ViewAreaPoints[3].x = Mathf.Min(screenSize.x + m_CameraX, position.width) - screenSize.x;
        m_ViewAreaPoints[3].y = screenSize.y;

        Handles.DrawLine(m_ViewAreaPoints[0], m_ViewAreaPoints[1]);
        Handles.DrawLine(m_ViewAreaPoints[1], m_ViewAreaPoints[2]);
        Handles.DrawLine(m_ViewAreaPoints[2], m_ViewAreaPoints[3]);
        Handles.DrawLine(m_ViewAreaPoints[3], m_ViewAreaPoints[0]);

        GUILayout.FlexibleSpace();

        float scale = EditorGUILayout.Slider("地图缩放", m_Scale, 0, 10);
        m_NormalSize = EditorGUILayout.Slider("正交尺寸", m_NormalSize, 0, 10);
        m_CameraX = EditorGUILayout.Slider("相机位置", m_CameraX, 0, Mathf.Max(texSize.x - screenSize.x, 0));

        if(scale != m_Scale)
        {
            m_Scale = scale;
            position = new Rect(position.x, position.y, screenSize.x, screenSize.y + 300);
        }

        if (screenSize.x + m_CameraX > position.width)
            m_ScrollX = screenSize.x + m_CameraX - position.width;
        else
            m_ScrollX = 0;

        ConfigPoint(UnityEngine.Event.current);
    }

    private void LoadTexture()
    {
        if (m_Textrue != null)
        {
            return;
        }

        m_Textrue = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ArtResources/Texture/Stage/1001_1.png");

        if (m_ViewAreaPoints == null)
        {
            m_ViewAreaPoints = new Vector3[4];
            for (int i = 0; i < m_ViewAreaPoints.Length; i++)
            {
                m_ViewAreaPoints[i] = Vector3.zero;
            }
        }

        Vector2 texSize = GetTextureSize();
        Vector2 screenSize = GetScreenSize();

        position = new Rect(position.x, position.y, texSize.x, screenSize.y + 300);
        minSize = new Vector2(texSize.x , screenSize.y + 300);
    }

    private void ConfigPoint(UnityEngine.Event e)
    {
        if (e.button == 1 && e.type == EventType.MouseUp)
        {
            if (!IsPointInTexture(e.mousePosition)) return;
            Debug.Log("在地图内");
        }
    }

    private Vector2 GetTextureSize()
    {
        Vector2 texSize = Vector2.zero;

        if (m_Textrue == null)
        {
            return texSize;
        }

        texSize.x = m_Textrue.width;
        texSize.y = m_Textrue.height;
        return texSize * m_Scale;
    }

    private Rect GetTextureRect()
    {
        Vector2 texSize = GetTextureSize();
        Vector2 screenSize = GetScreenSize();
        m_TextureRect.x = 0 - m_ScrollX;
        m_TextureRect.y = (Mathf.Max(screenSize.y - texSize.y, 0)) / 2;
        m_TextureRect.width = texSize.x;
        m_TextureRect.height = texSize.y;

        return m_TextureRect;
    }

    private Vector2 GetScreenSize()
    {
        Vector2 screenSize = Vector2.zero;
        Vector2 viewSize = GetMainGameViewSize();
        screenSize.y = m_NormalSize * 2 * 100f;
        screenSize.x = viewSize.x * screenSize.y / viewSize.y;

        return screenSize * m_Scale;
    }

    private Vector2 GetMainGameViewSize()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
        return (Vector2)Res;
    }

    private bool IsPointInTexture(Vector2 point)
    {
        return GetTextureRect().Contains(point);
    }

    private const float m_YOffest = 30f;
    private float m_ScrollX = 0;
    private float m_CameraX = 0;
    private float m_Scale = 1;
    private float m_NormalSize = 1;
    private Vector3[] m_ViewAreaPoints = null;
    private Rect m_TextureRect = Rect.zero;
    private Texture2D m_Textrue = null;
}
