using UnityEditor;
using UnityEngine;

public static class UIAtlasMakerTools
{
    static GameObject mPrevious;
    /// <summary>
    /// Fix the import settings for the specified texture, re-importing it if necessary.
    /// </summary>

    public static Texture2D ImportTexture(string path, bool forInput, bool force, bool alphaTransparency)
    {
        if (!string.IsNullOrEmpty(path))
        {
            if (forInput) { if (!MakeTextureReadable(path, force)) return null; }
            else if (!MakeTextureAnAtlas(path, force, alphaTransparency)) return null;
            //return AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;

            Texture2D tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            return tex;
        }
        return null;
    }

    /// <summary>
    /// Change the import settings of the specified texture asset, making it suitable to be used as a texture atlas.
    /// </summary>

    static bool MakeTextureAnAtlas(string path, bool force, bool alphaTransparency)
    {
        if (string.IsNullOrEmpty(path)) return false;
        var ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti == null) return false;

        var settings = new TextureImporterSettings();
        ti.ReadTextureSettings(settings);

        if (force || settings.readable ||
#if UNITY_5_5_OR_NEWER
			ti.maxTextureSize < 4096 ||
			(UIAtlasMakerSettings.trueColorAtlas && ti.textureCompression != TextureImporterCompression.Uncompressed) ||
#else
            settings.maxTextureSize < 4096 ||
#endif
            settings.wrapMode != TextureWrapMode.Clamp ||
            settings.npotScale != TextureImporterNPOTScale.ToNearest)
        {
            settings.readable = false;
#if !UNITY_4_7 && !UNITY_5_3 && !UNITY_5_4
			ti.maxTextureSize = 4096;
#else
            settings.maxTextureSize = 4096;
#endif
            settings.wrapMode = TextureWrapMode.Clamp;
            settings.npotScale = TextureImporterNPOTScale.ToNearest;

            if (UIAtlasMakerSettings.trueColorAtlas)
            {
#if UNITY_5_5_OR_NEWER
				ti.textureCompression = TextureImporterCompression.Uncompressed;
#else
                settings.textureFormat = TextureImporterFormat.ARGB32;
#endif
                settings.filterMode = FilterMode.Trilinear;
            }

            settings.aniso = 4;
            settings.alphaIsTransparency = alphaTransparency;
            ti.SetTextureSettings(settings);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        }
        return true;
    }

    /// <summary>
    /// Change the import settings of the specified texture asset, making it readable.
    /// </summary>

    public static bool MakeTextureReadable(string path, bool force)
    {
        if (string.IsNullOrEmpty(path)) return false;
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti == null) return false;

        TextureImporterSettings settings = new TextureImporterSettings();
        ti.ReadTextureSettings(settings);

        if (force || !settings.readable || settings.npotScale != TextureImporterNPOTScale.None || settings.alphaIsTransparency)
        {
            settings.readable = true;
#if !UNITY_4_7 && !UNITY_5_3 && !UNITY_5_4
			if (UIAtlasMakerSettings.trueColorAtlas)
			{
				var platform = ti.GetDefaultPlatformTextureSettings();
				platform.format = TextureImporterFormat.RGBA32;
			}
#else
            if (UIAtlasMakerSettings.trueColorAtlas) settings.textureFormat = TextureImporterFormat.AutomaticTruecolor;
#endif
            settings.npotScale = TextureImporterNPOTScale.None;
            settings.alphaIsTransparency = false;
            ti.SetTextureSettings(settings);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        }
        return true;
    }

    /// <summary>
    /// Fix the import settings for the specified texture, re-importing it if necessary.
    /// </summary>

    public static Texture2D ImportTexture(Texture tex, bool forInput, bool force, bool alphaTransparency)
    {
        if (tex != null)
        {
            string path = AssetDatabase.GetAssetPath(tex.GetInstanceID());
            return ImportTexture(path, forInput, force, alphaTransparency);
        }
        return null;
    }

    /// <summary>
    /// Figures out the saveable filename for the texture of the specified atlas.
    /// </summary>

    public static string GetSaveableTexturePath(UIAtlas atlas)
    {
        // Path where the texture atlas will be saved
        string path = "";

        // If the atlas already has a texture, overwrite its texture
        if (atlas.texture != null)
        {
            path = AssetDatabase.GetAssetPath(atlas.texture.GetInstanceID());

            if (!string.IsNullOrEmpty(path))
            {
                int dot = path.LastIndexOf('.');
                return path.Substring(0, dot) + ".png";
            }
        }

        // No texture to use -- figure out a name using the atlas
        path = AssetDatabase.GetAssetPath(atlas.GetInstanceID());
        path = string.IsNullOrEmpty(path) ? "Assets/" + atlas.name + ".png" : path.Replace(".prefab", ".png");
        return path;
    }

    /// <summary>
    /// Unity 4.3 changed the way LookLikeControls works.
    /// </summary>

    public static void SetLabelWidth(float width)
    {
        EditorGUIUtility.labelWidth = width;
    }


    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text) { return DrawHeader(text, text, false, UIAtlasMakerSettings.minimalisticLook); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key) { return DrawHeader(text, key, false, UIAtlasMakerSettings.minimalisticLook); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, bool detailed) { return DrawHeader(text, text, detailed, !detailed); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (!minimalistic) GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUI.changed = false;

        if (minimalistic)
        {
            if (state) text = "\u25BC" + (char)0x200a + text;
            else text = "\u25BA" + (char)0x200a + text;

            GUILayout.BeginHorizontal();
            GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
            if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
        else
        {
            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25BC " + text;
            else text = "\u25BA " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
        }

        if (GUI.changed) EditorPrefs.SetBool(key, state);

        if (!minimalistic) GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }


    /// <summary>
    /// Begin drawing the content area.
    /// </summary>

    public static void BeginContents() { BeginContents(UIAtlasMakerSettings.minimalisticLook); }

    static bool mEndHorizontal = false;

    /// <summary>
    /// Begin drawing the content area.
    /// </summary>

    public static void BeginContents(bool minimalistic)
    {
        if (!minimalistic)
        {
            mEndHorizontal = true;
            GUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
        }
        else
        {
            mEndHorizontal = false;
            EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
            GUILayout.Space(10f);
        }
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    /// <summary>
    /// End drawing the content area.
    /// </summary>

    public static void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (mEndHorizontal)
        {
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(3f);
    }

    /// <summary>
    /// Repaints all inspector windows related to sprite drawing.
    /// </summary>

    public static void RepaintSprites()
    {
        if (UIAtlasMaker.instance != null)
            UIAtlasMaker.instance.Repaint();
    }

    /// <summary>
    /// Select the specified sprite within the currently selected atlas.
    /// </summary>

    public static void SelectSprite(string spriteName)
    {
        if (UIAtlasMakerSettings.atlas != null)
        {
            UIAtlasMakerSettings.selectedSprite = spriteName;
            UIAtlasMakerTools.Select(UIAtlasMakerSettings.atlas.gameObject);
            RepaintSprites();
        }
    }

    /// <summary>
    /// Select the specified atlas and sprite.
    /// </summary>

    public static void SelectSprite(UIAtlas atlas, string spriteName)
    {
        if (atlas != null)
        {
            UIAtlasMakerSettings.atlas = atlas;
            UIAtlasMakerSettings.selectedSprite = spriteName;
            UIAtlasMakerTools.Select(atlas.gameObject);
            RepaintSprites();
        }
    }

    /// <summary>
    /// Select the specified game object and remember what was selected before.
    /// </summary>

    public static void Select(GameObject go)
    {
        mPrevious = Selection.activeGameObject;
        Selection.activeGameObject = go;
    }

    static public Object LoadAsset(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        return AssetDatabase.LoadMainAssetAtPath(path);
    }

    /// <summary>
    /// Convenience function to load an asset of specified type, given the full path to it.
    /// </summary>

    static public T LoadAsset<T>(string path) where T : Object
    {
        Object obj = LoadAsset(path);
        if (obj == null) return null;

        T val = obj as T;
        if (val != null) return val;

        if (typeof(T).IsSubclassOf(typeof(Component)))
        {
            if (obj.GetType() == typeof(GameObject))
            {
                GameObject go = obj as GameObject;
                return go.GetComponent(typeof(T)) as T;
            }
        }
        return null;
    }

    /// <summary>
    /// Automatically upgrade all of the UITextures in the scene to Sprites if they can be found within the specified atlas.
    /// </summary>

    static public void UpgradeTexturesToSprites(UIAtlas atlas)
    {
        if (atlas == null) return;
    }

    /// <summary>
    /// Convenience function that marks the specified object as dirty in the Unity Editor.
    /// </summary>

    static public void SetDirty(UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        if (obj)
        {
            //if (obj is Component) Debug.Log(NGUITools.GetHierarchy((obj as Component).gameObject), obj);
            //else if (obj is GameObject) Debug.Log(NGUITools.GetHierarchy(obj as GameObject), obj);
            //else Debug.Log("Hmm... " + obj.GetType(), obj);
            UnityEditor.EditorUtility.SetDirty(obj);
        }
#endif
    }

    /// <summary>
    /// Pre-multiply shaders result in a black outline if this operation is done in the shader. It's better to do it outside.
    /// </summary>

    static public Color ApplyPMA(Color c)
    {
        if (c.a != 1f)
        {
            c.r *= c.a;
            c.g *= c.a;
            c.b *= c.a;
        }
        return c;
    }
}
