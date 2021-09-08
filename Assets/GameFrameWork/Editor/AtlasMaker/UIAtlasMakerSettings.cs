using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UIAtlasMakerSettings
{
    static public int atlasPadding
    {
        get { return GetInt("NGUI Padding", 1); }
        set { SetInt("NGUI Padding", value); }
    }

    static public bool minimalisticLook
    {
        get { return GetBool("NGUI Minimalistic", false); }
        set { SetBool("NGUI Minimalistic", value); }
    }

    static public UIAtlas atlas
    {
        get { return Get<UIAtlas>("NGUI Atlas", null); }
        set { Set("NGUI Atlas", value); }
    }


    static public bool atlasPMA
    {
        get { return GetBool("NGUI PMA", false); }
        set { SetBool("NGUI PMA", value); }
    }

    static public string currentPath
    {
        get { return GetString("NGUI Path", "Assets/"); }
        set { SetString("NGUI Path", value); }
    }

    static public bool unityPacking
    {
        get { return GetBool("NGUI Atlas Packing", false); }
        set { SetBool("NGUI Atlas Packing", value); }
    }


    static public bool trueColorAtlas
    {
        get { return GetBool("NGUI Truecolor", true); }
        set { SetBool("NGUI Truecolor", value); }
    }

    static public bool atlasTrimming
    {
        get { return GetBool("NGUI Trim", true); }
        set { SetBool("NGUI Trim", value); }
    }

    static public bool forceSquareAtlas
    {
        get { return GetBool("NGUI Square", false); }
        set { SetBool("NGUI Square", value); }
    }

    static public bool autoUpgradeSprites
    {
        get { return GetBool("NGUI AutoUpgrade", false); }
        set { SetBool("NGUI AutoUpgrade", value); }
    }

    static public string selectedSprite
    {
        get { return GetString("NGUI Sprite", null); }
        set { SetString("NGUI Sprite", value); }
    }

    static public bool allow4096
    {
        get { return GetBool("NGUI 4096", true); }
        set { SetBool("NGUI 4096", value); }
    }

    /// <summary>
    /// Get a previously saved object from settings.
    /// </summary>

    static public T Get<T>(string name, T defaultValue) where T : Object
    {
        string path = EditorPrefs.GetString(name);
        if (string.IsNullOrEmpty(path)) return null;

        T retVal = UIAtlasMakerTools.LoadAsset<T>(path);

        if (retVal == null)
        {
            int id;
            if (int.TryParse(path, out id))
                return EditorUtility.InstanceIDToObject(id) as T;
        }
        return retVal;
    }

    /// <summary>
    /// Get the previously saved boolean value.
    /// </summary>

    static public bool GetBool(string name, bool defaultValue) { return EditorPrefs.GetBool(name, defaultValue); }
    /// <summary>
    /// Get the previously saved string value.
    /// </summary>

    static public string GetString(string name, string defaultValue) { return EditorPrefs.GetString(name, defaultValue); }

    /// <summary>
    /// Get the previously saved integer value.
    /// </summary>

    static public int GetInt(string name, int defaultValue) { return EditorPrefs.GetInt(name, defaultValue); }

    /// <summary>
    /// Save the specified object in settings.
    /// </summary>

    static public void Set(string name, Object obj)
    {
        if (obj == null)
        {
            EditorPrefs.DeleteKey(name);
        }
        else
        {
            if (obj != null)
            {
                string path = AssetDatabase.GetAssetPath(obj);

                if (!string.IsNullOrEmpty(path))
                {
                    EditorPrefs.SetString(name, path);
                }
                else
                {
                    EditorPrefs.SetString(name, obj.GetInstanceID().ToString());
                }
            }
            else EditorPrefs.DeleteKey(name);
        }
    }

    /// <summary>
    /// Save the specified boolean value in settings.
    /// </summary>

    static public void SetBool(string name, bool val) { EditorPrefs.SetBool(name, val); }

    /// <summary>
    /// Save the specified string value in settings.
    /// </summary>

    static public void SetString(string name, string val) { EditorPrefs.SetString(name, val); }


    /// <summary>
    /// Save the specified integer value in settings.
    /// </summary>

    static public void SetInt(string name, int val) { EditorPrefs.SetInt(name, val); }
}
