using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MapEditorUtil
{
    public static string MAP_PATH = "Assets/ArtResources/Texture/Stage/";
    public static Texture2D Texture
    {
        get
        {
            return m_Textrue;
        }
    }

    public static float Scale
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

    public static float ScrollX
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

    public static float NormalSize
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

    public static Vector2 CurrPos
    {
        get
        {
            return m_CurrPos;
        }
        set
        {
            m_CurrPos = value;
        }
    }

    public static Vector2 CurrInitPos
    {
        get
        {
            return m_CurrInitPos;
        }
        set
        {
            m_CurrInitPos = value;
        }
    }

    public static void LoadTexture(string path)
    {
        if (m_CurrPath.Equals(path))
        {
            return;
        }

        m_CurrPath = path;
        m_Textrue = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    public static Vector2 GetTextureSize()
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
        m_CurrPointRect.x = m_CurrPos.x;
        m_CurrPointRect.y = m_CurrPos.y;
        m_CurrPointRect.width = 5;
        m_CurrPointRect.height = 5;

        return m_CurrPointRect;
    }

    public static Rect GetCurrInitPointRect()
    {
        m_CurrInitPointRect.x = m_CurrInitPos.x;
        m_CurrInitPointRect.y = m_CurrInitPos.y;
        m_CurrInitPointRect.width = 5;
        m_CurrInitPointRect.height = 5;

        return m_CurrInitPointRect;
    }

    public static Vector2 GetScreenSize()
    {
        Vector2 screenSize = Vector2.zero;
        Vector2 viewSize = GetMainGameViewSize();
        screenSize.y = m_NormalSize * 2 * 100f;
        screenSize.x = viewSize.x * screenSize.y / viewSize.y;

        return screenSize * m_Scale;
    }

    private static Vector2 GetMainGameViewSize()
    {
        if (m_GameViewSize != Vector2.zero)
        {
            return m_GameViewSize;
        }

        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
        m_GameViewSize = (Vector2)Res;
        return m_GameViewSize;
    }

    public static bool IsPointInTexture(Vector2 point)
    {
        return GetTextureRect().Contains(point);
    }

    private static float m_Scale = 1;
    private static float m_ScrollX = 0;
    private static float m_NormalSize = 1;
    private static string m_CurrPath = string.Empty;
    private static Texture2D m_Textrue = null;
    private static Rect m_TextureRect = Rect.zero;
    private static Rect m_CurrPointRect = Rect.zero;
    private static Rect m_CurrInitPointRect = Rect.zero;
    private static Vector2 m_CurrPos = Vector2.zero;
    private static Vector2 m_CurrInitPos = Vector2.zero;
    private static Vector2 m_GameViewSize = Vector2.zero;
}
