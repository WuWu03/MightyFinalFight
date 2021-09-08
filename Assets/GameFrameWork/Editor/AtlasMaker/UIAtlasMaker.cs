using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UIAtlasMaker : EditorWindow
{
    static public UIAtlasMaker instance;

    public class SpriteEntry : AtlasSprite
    {
        // Sprite texture -- original texture or a temporary texture
        public Texture2D tex;

        // Temporary game object -- used to prevent Unity from unloading the texture
        public GameObject tempGO;

        // Temporary material -- same usage as the temporary game object
        public Material tempMat;

        // Whether the texture is temporary and should be deleted
        public bool temporaryTexture = false;

        /// <summary>
        /// HACK: Prevent Unity from unloading temporary textures.
        /// </summary>

        public void SetTexture(Color32[] newPixels, int newWidth, int newHeight)
        {
            Release();

            temporaryTexture = true;

            tex = new Texture2D(newWidth, newHeight);
            tex.name = name;
            tex.SetPixels32(newPixels);
            tex.Apply();

            tempGO = EditorUtility.CreateGameObjectWithHideFlags(name, HideFlags.HideAndDontSave, typeof(MeshRenderer));
            tempGO.GetComponent<MeshRenderer>().sharedMaterial = tempMat;
        }

        /// <summary>
        /// Release temporary resources.
        /// </summary>

        public void Release()
        {
            if (temporaryTexture)
            {
                Object.DestroyImmediate(tempGO);
                Object.DestroyImmediate(tempMat);
                Object.DestroyImmediate(tex);

                tempGO = null;
                tempMat = null;
                tex = null;
                temporaryTexture = false;
            }
        }
    }

    Vector2 mScroll = Vector2.zero;
    List<string> mDelNames = new List<string>();
    UIAtlas mLastAtlas;

    void OnEnable() { instance = this; }
    void OnDisable() { instance = null; }

    /// <summary>
    /// Atlas selection callback.
    /// </summary>

    void OnSelectAtlas(Object obj)
    {
        if (UIAtlasMakerSettings.atlas != obj)
        {
            UIAtlasMakerSettings.atlas = obj as UIAtlas;
            Repaint();
        }
    }

    /// <summary>
    /// Refresh the window on selection.
    /// </summary>

    void OnSelectionChange() { mDelNames.Clear(); Repaint(); }

    /// <summary>
    /// Helper function that retrieves the list of currently selected textures.
    /// </summary>

    List<Texture> GetSelectedTextures()
    {
        List<Texture> textures = new List<Texture>();

        if (Selection.objects != null && Selection.objects.Length > 0)
        {
            Object[] objects = EditorUtility.CollectDependencies(Selection.objects);

            foreach (Object o in objects)
            {
                Texture tex = o as Texture;
                if (tex == null || tex.name == "Font Texture") continue;
                if (UIAtlasMakerSettings.atlas == null || UIAtlasMakerSettings.atlas.texture != tex) textures.Add(tex);
            }
        }
        return textures;
    }

    /// <summary>
    /// Load the specified list of textures as Texture2Ds, fixing their import properties as necessary.
    /// </summary>

    static List<Texture2D> LoadTextures(List<Texture> textures)
    {
        List<Texture2D> list = new List<Texture2D>();

        foreach (Texture tex in textures)
        {
            Texture2D t2 = UIAtlasMakerTools.ImportTexture(tex, true, false, true);
            if (t2 != null) list.Add(t2);
        }
        return list;
    }

    /// <summary>
    /// Used to sort the sprites by pixels used
    /// </summary>

    static int Compare(SpriteEntry a, SpriteEntry b)
    {
        // A is null b is not b is greater so put it at the front of the list
        if (a == null && b != null) return 1;

        // A is not null b is null a is greater so put it at the front of the list
        if (a != null && b == null) return -1;

        // Get the total pixels used for each sprite
        int aPixels = a.width * a.height;
        int bPixels = b.width * b.height;

        if (aPixels > bPixels) return -1;
        else if (aPixels < bPixels) return 1;
        return 0;
    }

    /// <summary>
    /// Pack all of the specified sprites into a single texture, updating the outer and inner rects of the sprites as needed.
    /// </summary>

    static bool PackTextures(Texture2D tex, List<SpriteEntry> sprites)
    {
        Texture2D[] textures = new Texture2D[sprites.Count];
        Rect[] rects;

#if UNITY_3_5 || UNITY_4_0
		int maxSize = 4096;
#else
        int maxSize = SystemInfo.maxTextureSize;
#endif

#if UNITY_ANDROID || UNITY_IPHONE
        maxSize = Mathf.Min(maxSize, UIAtlasMakerSettings.allow4096 ? 4096 : 2048);
#endif
        if (UIAtlasMakerSettings.unityPacking)
        {
            for (int i = 0; i < sprites.Count; ++i) textures[i] = sprites[i].tex;
            rects = tex.PackTextures(textures, UIAtlasMakerSettings.atlasPadding, maxSize);
        }
        else
        {
            sprites.Sort(Compare);
            for (int i = 0; i < sprites.Count; ++i) textures[i] = sprites[i].tex;
            rects = UITexturePacker.PackTextures(tex, textures, 4, 4, UIAtlasMakerSettings.atlasPadding, maxSize);
        }

        for (int i = 0; i < sprites.Count; ++i)
        {
            Rect rect = AltasMath.ConvertToPixels(rects[i], tex.width, tex.height, true);

            // Apparently Unity can take the liberty of destroying temporary textures without any warning
            if (textures[i] == null) return false;

            // Make sure that we don't shrink the textures
            if (Mathf.RoundToInt(rect.width) != textures[i].width) return false;

            SpriteEntry se = sprites[i];
            se.x = Mathf.RoundToInt(rect.x);
            se.y = Mathf.RoundToInt(rect.y);
            se.width = Mathf.RoundToInt(rect.width);
            se.height = Mathf.RoundToInt(rect.height);
        }
        return true;
    }

    /// <summary>
    /// Helper function that creates a single sprite list from both the atlas's sprites as well as selected textures.
    /// Dictionary value meaning:
    /// 0 = No change
    /// 1 = Update
    /// 2 = Add
    /// </summary>

    Dictionary<string, int> GetSpriteList(List<Texture> textures)
    {
        Dictionary<string, int> spriteList = new Dictionary<string, int>();

        // If we have textures to work with, include them as well
        if (textures.Count > 0)
        {
            List<string> texNames = new List<string>();
            foreach (Texture tex in textures) texNames.Add(tex.name);
            texNames.Sort();
            foreach (string tex in texNames) spriteList.Add(tex, 2);
        }

        if (UIAtlasMakerSettings.atlas != null)
        {
            List<string> spriteNames = UIAtlasMakerSettings.atlas.GetListOfSprites();
            foreach (string sp in spriteNames)
            {
                if (spriteList.ContainsKey(sp)) spriteList[sp] = 1;
                else spriteList.Add(sp, 0);
            }
        }
        return spriteList;
    }

    /// <summary>
    /// Add a new sprite to the atlas, given the texture it's coming from and the packed rect within the atlas.
    /// </summary>

    static public AtlasSprite AddSprite(List<AtlasSprite> sprites, SpriteEntry se)
    {
        // See if this sprite already exists
        foreach (AtlasSprite sp in sprites)
        {
            if (sp.name == se.name)
            {
                sp.CopyFrom(se);
                return sp;
            }
        }

        AtlasSprite sprite = new AtlasSprite();
        sprite.CopyFrom(se);
        sprites.Add(sprite);
        return sprite;
    }

    /// <summary>
    /// Create a list of sprites using the specified list of textures.
    /// </summary>

    static public List<SpriteEntry> CreateSprites(List<Texture> textures)
    {
        List<SpriteEntry> list = new List<SpriteEntry>();

        foreach (Texture tex in textures)
        {
            Texture2D oldTex = UIAtlasMakerTools.ImportTexture(tex, true, false, true);
            if (oldTex == null) oldTex = tex as Texture2D;
            if (oldTex == null) continue;

            // If we aren't doing trimming, just use the texture as-is
            if (!UIAtlasMakerSettings.atlasTrimming && !UIAtlasMakerSettings.atlasPMA)
            {
                SpriteEntry sprite = new SpriteEntry();
                sprite.SetRect(0, 0, oldTex.width, oldTex.height);
                sprite.tex = oldTex;
                sprite.name = oldTex.name;
                sprite.temporaryTexture = false;
                list.Add(sprite);
                continue;
            }

            // If we want to trim transparent pixels, there is more work to be done
            Color32[] pixels = oldTex.GetPixels32();

            int xmin = oldTex.width;
            int xmax = 0;
            int ymin = oldTex.height;
            int ymax = 0;
            int oldWidth = oldTex.width;
            int oldHeight = oldTex.height;

            // Find solid pixels
            if (UIAtlasMakerSettings.atlasTrimming)
            {
                for (int y = 0, yw = oldHeight; y < yw; ++y)
                {
                    for (int x = 0, xw = oldWidth; x < xw; ++x)
                    {
                        Color32 c = pixels[y * xw + x];

                        if (c.a != 0)
                        {
                            if (y < ymin) ymin = y;
                            if (y > ymax) ymax = y;
                            if (x < xmin) xmin = x;
                            if (x > xmax) xmax = x;
                        }
                    }
                }
            }
            else
            {
                xmin = 0;
                xmax = oldWidth - 1;
                ymin = 0;
                ymax = oldHeight - 1;
            }

            int newWidth = (xmax - xmin) + 1;
            int newHeight = (ymax - ymin) + 1;

            if (newWidth > 0 && newHeight > 0)
            {
                SpriteEntry sprite = new SpriteEntry();
                sprite.x = 0;
                sprite.y = 0;
                sprite.width = oldTex.width;
                sprite.height = oldTex.height;

                // If the dimensions match, then nothing was actually trimmed
                if (!UIAtlasMakerSettings.atlasPMA && (newWidth == oldWidth && newHeight == oldHeight))
                {
                    sprite.tex = oldTex;
                    sprite.name = oldTex.name;
                    sprite.temporaryTexture = false;
                }
                else
                {
                    // Copy the non-trimmed texture data into a temporary buffer
                    Color32[] newPixels = new Color32[newWidth * newHeight];

                    for (int y = 0; y < newHeight; ++y)
                    {
                        for (int x = 0; x < newWidth; ++x)
                        {
                            int newIndex = y * newWidth + x;
                            int oldIndex = (ymin + y) * oldWidth + (xmin + x);
                            if (UIAtlasMakerSettings.atlasPMA) newPixels[newIndex] = ApplyPMA(pixels[oldIndex]);
                            else newPixels[newIndex] = pixels[oldIndex];
                        }
                    }

                    // Create a new texture
                    sprite.name = oldTex.name;
                    sprite.SetTexture(newPixels, newWidth, newHeight);

                    // Remember the padding offset
                    sprite.SetPadding(xmin, ymin, oldWidth - newWidth - xmin, oldHeight - newHeight - ymin);
                }
                list.Add(sprite);
            }
        }
        return list;
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

    /// <summary>
    /// Release all temporary textures created for the sprites.
    /// </summary>

    static public void ReleaseSprites(List<SpriteEntry> sprites)
    {
        foreach (SpriteEntry se in sprites) se.Release();
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// Replace the sprites within the atlas.
    /// </summary>

    static public void ReplaceSprites(UIAtlas atlas, List<SpriteEntry> sprites)
    {
        // Get the list of sprites we'll be updating
        List<AtlasSprite> spriteList = atlas.spriteList;
        List<AtlasSprite> kept = new List<AtlasSprite>();

        // Run through all the textures we added and add them as sprites to the atlas
        for (int i = 0; i < sprites.Count; ++i)
        {
            SpriteEntry se = sprites[i];
            AtlasSprite sprite = AddSprite(spriteList, se);
            kept.Add(sprite);
        }

        // Remove unused sprites
        for (int i = spriteList.Count; i > 0;)
        {
            AtlasSprite sp = spriteList[--i];
            if (!kept.Contains(sp)) spriteList.RemoveAt(i);
        }

        // Sort the sprites so that they are alphabetical within the atlas
        atlas.SortAlphabetically();
        atlas.MarkAsChanged();
    }

    /// <summary>
    /// Duplicate the specified sprite.
    /// </summary>

    static public SpriteEntry DuplicateSprite(UIAtlas atlas, string spriteName)
    {
        if (atlas == null || atlas.texture == null) return null;
        AtlasSprite sd = atlas.GetSprite(spriteName);
        if (sd == null) return null;

        Texture2D tex = UIAtlasMakerTools.ImportTexture(atlas.texture, true, true, false);
        SpriteEntry se = ExtractSprite(sd, tex);

        if (se != null)
        {
            se.name = se.name + " (Copy)";

            List<UIAtlasMaker.SpriteEntry> sprites = new List<UIAtlasMaker.SpriteEntry>();
            UIAtlasMaker.ExtractSprites(atlas, sprites);
            sprites.Add(se);
            UIAtlasMaker.UpdateAtlas(atlas, sprites);
            se.Release();
        }
        else UIAtlasMakerTools.ImportTexture(atlas.texture, false, false, !atlas.premultipliedAlpha);
        return se;
    }

    /// <summary>
    /// Extract the specified sprite from the atlas.
    /// </summary>

    static public SpriteEntry ExtractSprite(UIAtlas atlas, string spriteName)
    {
        if (atlas.texture == null) return null;
        AtlasSprite sd = atlas.GetSprite(spriteName);
        if (sd == null) return null;

        Texture2D tex = UIAtlasMakerTools.ImportTexture(atlas.texture, true, true, false);
        SpriteEntry se = ExtractSprite(sd, tex);
        UIAtlasMakerTools.ImportTexture(atlas.texture, false, false, !atlas.premultipliedAlpha);
        return se;
    }

    /// <summary>
    /// Extract the specified sprite from the atlas texture.
    /// </summary>

    static SpriteEntry ExtractSprite(AtlasSprite es, Texture2D tex)
    {
        return (tex != null) ? ExtractSprite(es, tex.GetPixels32(), tex.width, tex.height) : null;
    }

    /// <summary>
    /// Extract the specified sprite from the atlas texture.
    /// </summary>

    static SpriteEntry ExtractSprite(AtlasSprite es, Color32[] oldPixels, int oldWidth, int oldHeight)
    {
        int xmin = Mathf.Clamp(es.x, 0, oldWidth);
        int ymin = Mathf.Clamp(es.y, 0, oldHeight);
        int xmax = Mathf.Min(xmin + es.width, oldWidth - 1);
        int ymax = Mathf.Min(ymin + es.height, oldHeight - 1);
        int newWidth = Mathf.Clamp(es.width, 0, oldWidth);
        int newHeight = Mathf.Clamp(es.height, 0, oldHeight);

        if (newWidth == 0 || newHeight == 0) return null;

        Color32[] newPixels = new Color32[newWidth * newHeight];

        for (int y = 0; y < newHeight; ++y)
        {
            int cy = ymin + y;
            if (cy > ymax) cy = ymax;

            for (int x = 0; x < newWidth; ++x)
            {
                int cx = xmin + x;
                if (cx > xmax) cx = xmax;

                int newIndex = (newHeight - 1 - y) * newWidth + x;
                int oldIndex = (oldHeight - 1 - cy) * oldWidth + cx;

                newPixels[newIndex] = oldPixels[oldIndex];
            }
        }

        // Create a new sprite
        SpriteEntry sprite = new SpriteEntry();
        sprite.CopyFrom(es);
        sprite.SetRect(0, 0, newWidth, newHeight);
        sprite.SetTexture(newPixels, newWidth, newHeight);
        return sprite;
    }

    /// <summary>
    /// Extract sprites from the atlas, adding them to the list.
    /// </summary>

    static public void ExtractSprites(UIAtlas atlas, List<SpriteEntry> finalSprites)
    {
        ShowProgress(0f);

        // Make the atlas texture readable
        Texture2D tex = UIAtlasMakerTools.ImportTexture(atlas.texture, true, true, false);

        if (tex != null)
        {
            Color32[] pixels = null;
            int width = tex.width;
            int height = tex.height;
            List<AtlasSprite> sprites = atlas.spriteList;
            float count = sprites.Count;
            int index = 0;

            foreach (AtlasSprite es in sprites)
            {
                ShowProgress((index++) / count);

                bool found = false;

                foreach (SpriteEntry fs in finalSprites)
                {
                    if (es.name == fs.name)
                    {
                        fs.CopyBorderFrom(es);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    if (pixels == null) pixels = tex.GetPixels32();
                    SpriteEntry sprite = ExtractSprite(es, pixels, width, height);
                    if (sprite != null) finalSprites.Add(sprite);
                }
            }
        }

        // The atlas no longer needs to be readable
        UIAtlasMakerTools.ImportTexture(atlas.texture, false, false, !atlas.premultipliedAlpha);
        ShowProgress(1f);
    }

    /// <summary>
    /// Combine all sprites into a single texture and save it to disk.
    /// </summary>

    static public bool UpdateTexture(UIAtlas atlas, List<SpriteEntry> sprites)
    {
        // Get the texture for the atlas
        Texture2D tex = atlas.texture as Texture2D;
        string oldPath = (tex != null) ? AssetDatabase.GetAssetPath(tex.GetInstanceID()) : "";
        string newPath = UIAtlasMakerTools.GetSaveableTexturePath(atlas);

        // Clear the read-only flag in texture file attributes
        if (System.IO.File.Exists(newPath))
        {
            System.IO.FileAttributes newPathAttrs = System.IO.File.GetAttributes(newPath);
            newPathAttrs &= ~System.IO.FileAttributes.ReadOnly;
            System.IO.File.SetAttributes(newPath, newPathAttrs);
        }

        bool newTexture = (tex == null || oldPath != newPath);

        if (newTexture)
        {
            // Create a new texture for the atlas
            tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        }
        else
        {
            // Make the atlas readable so we can save it
            tex = UIAtlasMakerTools.ImportTexture(oldPath, true, false, false);
        }

        // Pack the sprites into this texture
        if (PackTextures(tex, sprites))
        {
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(newPath, bytes);
            bytes = null;

            // Load the texture we just saved as a Texture2D
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            tex = UIAtlasMakerTools.ImportTexture(newPath, false, true, !atlas.premultipliedAlpha);

            // Update the atlas texture
            if (newTexture)
            {
                if (tex == null) Debug.LogError("Failed to load the created atlas saved as " + newPath);

                ReleaseSprites(sprites);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
            return true;
        }
        else
        {
            if (!newTexture) UIAtlasMakerTools.ImportTexture(oldPath, false, true, !atlas.premultipliedAlpha);

            EditorUtility.DisplayDialog("Operation Canceled", "The selected sprites can't fit into the atlas.\n" +
                    "Keep large sprites outside the atlas (use UITexture), and/or use multiple atlases instead", "OK");
            return false;
        }
    }

    /// <summary>
    /// Show a progress bar.
    /// </summary>

    static public void ShowProgress(float val)
    {
        EditorUtility.DisplayProgressBar("Updating", "Updating the atlas, please wait...", val);
    }

    /// <summary>
    /// Add the specified texture to the atlas, or update an existing one.
    /// </summary>

    static public void AddOrUpdate(UIAtlas atlas, Texture2D tex)
    {
        if (atlas != null && tex != null)
        {
            List<Texture> textures = new List<Texture>();
            textures.Add(tex);
            List<SpriteEntry> sprites = CreateSprites(textures);
            ExtractSprites(atlas, sprites);
            UpdateAtlas(atlas, sprites);
        }
    }

    /// <summary>
    /// Add the specified texture to the atlas, or update an existing one.
    /// </summary>

    static public void AddOrUpdate(UIAtlas atlas, SpriteEntry se)
    {
        if (atlas != null && se != null)
        {
            List<SpriteEntry> sprites = new List<SpriteEntry>();
            sprites.Add(se);
            ExtractSprites(atlas, sprites);
            UpdateAtlas(atlas, sprites);
        }
    }

    /// <summary>
    /// Update the sprites within the texture atlas, preserving the sprites that have not been selected.
    /// </summary>

    void UpdateAtlas(List<Texture> textures, bool keepSprites)
    {
        // Create a list of sprites using the collected textures
        List<SpriteEntry> sprites = CreateSprites(textures);

        if (sprites.Count > 0)
        {
            // Extract sprites from the atlas, filling in the missing pieces
            if (keepSprites) ExtractSprites(UIAtlasMakerSettings.atlas, sprites);

            // Update the atlas
            UpdateAtlas(UIAtlasMakerSettings.atlas, sprites);
        }
        else if (!keepSprites)
        {
            UpdateAtlas(UIAtlasMakerSettings.atlas, sprites);
        }
    }

    /// <summary>
    /// Update the sprite atlas, keeping only the sprites that are on the specified list.
    /// </summary>

    static public void UpdateAtlas(UIAtlas atlas, List<SpriteEntry> sprites)
    {
        if (sprites.Count > 0)
        {
            // Combine all sprites into a single texture and save it
            if (UpdateTexture(atlas, sprites))
            {
                // Replace the sprites within the atlas
                ReplaceSprites(atlas, sprites);
            }

            // Release the temporary textures
            ReleaseSprites(sprites);
            EditorUtility.ClearProgressBar();
            return;
        }
        else
        {
            atlas.spriteList.Clear();
            string path = UIAtlasMakerTools.GetSaveableTexturePath(atlas);
            atlas.spriteMaterial.mainTexture = null;
            if (!string.IsNullOrEmpty(path)) AssetDatabase.DeleteAsset(path);
        }

        atlas.MarkAsChanged();
        Selection.activeGameObject = (UIAtlasMakerSettings.atlas != null) ? UIAtlasMakerSettings.atlas.gameObject : null;
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// Draw the UI for this tool.
    /// </summary>

    void OnGUI()
    {
        if (mLastAtlas != UIAtlasMakerSettings.atlas)
            mLastAtlas = UIAtlasMakerSettings.atlas;

        bool update = false;
        bool replace = false;

        UIAtlasMakerTools.SetLabelWidth(84f);
        GUILayout.Space(3f);

        UIAtlasMakerTools.DrawHeader("Input", true);
        UIAtlasMakerTools.BeginContents(false);

        GUILayout.BeginHorizontal();
        {
            EditorGUI.BeginDisabledGroup(UIAtlasMakerSettings.atlas == null);
            if (GUILayout.Button("New", GUILayout.Width(40f)))
                UIAtlasMakerSettings.atlas = null;
            EditorGUI.EndDisabledGroup();
        }
        GUILayout.EndHorizontal();

        List<Texture> textures = GetSelectedTextures();

        if (UIAtlasMakerSettings.atlas != null)
        {
            Texture tex = UIAtlasMakerSettings.atlas.texture;

            // Texture atlas information
            GUILayout.BeginHorizontal();
            {
                if (tex != null)
                {
                    if (GUILayout.Button("Texture", GUILayout.Width(76f))) Selection.activeObject = tex;
                    GUILayout.Label(" " + tex.width + "x" + tex.height);
                }
                else
                {
                    GUI.color = Color.grey;
                    GUILayout.Button("Texture", GUILayout.Width(76f));
                    GUI.color = Color.white;
                    GUILayout.Label(" N/A");
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        UIAtlasMakerSettings.atlasPadding = Mathf.Clamp(EditorGUILayout.IntField("Padding", UIAtlasMakerSettings.atlasPadding, GUILayout.Width(100f)), 0, 8);
        GUILayout.Label((UIAtlasMakerSettings.atlasPadding == 1 ? "pixel" : "pixels") + " between sprites");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        UIAtlasMakerSettings.atlasTrimming = EditorGUILayout.Toggle("Trim Alpha", UIAtlasMakerSettings.atlasTrimming, GUILayout.Width(100f));
        GUILayout.Label("Remove empty space");
        GUILayout.EndHorizontal();


#if !UNITY_5_6
        GUILayout.BeginHorizontal();
        UIAtlasMakerSettings.unityPacking = EditorGUILayout.Toggle("Unity Packer", UIAtlasMakerSettings.unityPacking, GUILayout.Width(100f));
        GUILayout.Label("or custom packer", GUILayout.MinWidth(70f));
        GUILayout.EndHorizontal();
#endif

        GUILayout.BeginHorizontal();
        UIAtlasMakerSettings.trueColorAtlas = EditorGUILayout.Toggle("Truecolor", UIAtlasMakerSettings.trueColorAtlas, GUILayout.Width(100f));
        GUILayout.Label("force ARGB32 textures", GUILayout.MinWidth(70f));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        UIAtlasMakerSettings.autoUpgradeSprites = EditorGUILayout.Toggle("Auto-upgrade", UIAtlasMakerSettings.trueColorAtlas, GUILayout.Width(100f));
        GUILayout.Label("replace textures with sprites", GUILayout.MinWidth(70f));
        GUILayout.EndHorizontal();

#if !UNITY_5_6
        if (!UIAtlasMakerSettings.unityPacking)
        {
            GUILayout.BeginHorizontal();
            UIAtlasMakerSettings.forceSquareAtlas = EditorGUILayout.Toggle("Force Square", UIAtlasMakerSettings.forceSquareAtlas, GUILayout.Width(100f));
            GUILayout.Label("if on, forces a square atlas texture", GUILayout.MinWidth(70f));
            GUILayout.EndHorizontal();
        }
#endif

#if UNITY_IPHONE || UNITY_ANDROID
        GUILayout.BeginHorizontal();
        UIAtlasMakerSettings.allow4096 = EditorGUILayout.Toggle("4096x4096", UIAtlasMakerSettings.allow4096, GUILayout.Width(100f));
        GUILayout.Label("if off, limit atlases to 2048x2048");
        GUILayout.EndHorizontal();
#endif
        UIAtlasMakerTools.EndContents();

        if (UIAtlasMakerSettings.atlas != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);

            if (textures.Count > 0)
            {
                update = GUILayout.Button("Add/Update");
            }
            else if (GUILayout.Button("View Sprites"))
            {
                UIAtlasSelector.ShowSelected();
            }

            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("You can create a new atlas by selecting one or more textures in the Project View window, then clicking \"Create\".", MessageType.Info);

            EditorGUI.BeginDisabledGroup(textures.Count == 0);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            bool create = GUILayout.Button("Create");
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            if (create)
            {
                string path = EditorUtility.SaveFilePanelInProject("Save As",
                    "New Atlas.prefab", "prefab", "Save atlas as...", UIAtlasMakerSettings.currentPath);

                if (!string.IsNullOrEmpty(path))
                {
                    UIAtlasMakerSettings.currentPath = System.IO.Path.GetDirectoryName(path);
                    GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                    string matPath = path.Replace(".prefab", ".mat");
                    replace = true;

                    // Create a new prefab for the atlas
                    Object prefab = (go != null) ? go : PrefabUtility.CreateEmptyPrefab(path);

                    // Create a new game object for the atlas
                    string atlasName = path.Replace(".prefab", "");
                    atlasName = atlasName.Substring(path.LastIndexOfAny(new char[] { '/', '\\' }) + 1);
                    go = new GameObject(atlasName);
                    go.AddComponent<UIAtlas>();
                    // Update the prefab
                    PrefabUtility.ReplacePrefab(go, prefab);
                    DestroyImmediate(go);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                    // Select the atlas
                    go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                    UIAtlasMakerSettings.atlas = go.GetComponent<UIAtlas>();
                    Selection.activeGameObject = go;
                }
            }
        }

        string selection = null;
        Dictionary<string, int> spriteList = GetSpriteList(textures);

        if (spriteList.Count > 0)
        {
            UIAtlasMakerTools.DrawHeader("Sprites", true);
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(3f);
                GUILayout.BeginVertical();

                mScroll = GUILayout.BeginScrollView(mScroll);

                bool delete = false;
                int index = 0;
                foreach (KeyValuePair<string, int> iter in spriteList)
                {
                    ++index;

                    GUILayout.Space(-1f);
                    GUI.backgroundColor = Color.white;
                    GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                    GUI.backgroundColor = Color.white;
                    GUILayout.Label(index.ToString(), GUILayout.Width(24f));

                    if (GUILayout.Button(iter.Key, "OL TextField", GUILayout.Height(20f)))
                        selection = iter.Key;

                    if (iter.Value == 2)
                    {
                        GUI.color = Color.green;
                        GUILayout.Label("Add", GUILayout.Width(27f));
                        GUI.color = Color.white;
                    }
                    else if (iter.Value == 1)
                    {
                        GUI.color = Color.cyan;
                        GUILayout.Label("Update", GUILayout.Width(45f));
                        GUI.color = Color.white;
                    }
                    else
                    {
                        if (mDelNames.Contains(iter.Key))
                        {
                            GUI.backgroundColor = Color.red;

                            if (GUILayout.Button("Delete", GUILayout.Width(60f)))
                            {
                                delete = true;
                            }
                            GUI.backgroundColor = Color.green;
                            if (GUILayout.Button("X", GUILayout.Width(22f)))
                            {
                                mDelNames.Remove(iter.Key);
                                delete = false;
                            }
                            GUI.backgroundColor = Color.white;
                        }
                        else
                        {
                            // If we have not yet selected a sprite for deletion, show a small "X" button
                            if (GUILayout.Button("X", GUILayout.Width(22f))) mDelNames.Add(iter.Key);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.Space(3f);
                GUILayout.EndHorizontal();

                // If this sprite was marked for deletion, remove it from the atlas
                if (delete)
                {
                    List<SpriteEntry> sprites = new List<SpriteEntry>();
                    ExtractSprites(UIAtlasMakerSettings.atlas, sprites);

                    for (int i = sprites.Count; i > 0;)
                    {
                        SpriteEntry ent = sprites[--i];
                        if (mDelNames.Contains(ent.name))
                            sprites.RemoveAt(i);
                    }
                    UpdateAtlas(UIAtlasMakerSettings.atlas, sprites);
                    mDelNames.Clear();
                    UIAtlasMakerTools.RepaintSprites();
                }
                else if (update) UpdateAtlas(textures, true);
                else if (replace) UpdateAtlas(textures, false);

                if (UIAtlasMakerSettings.atlas != null && !string.IsNullOrEmpty(selection))
                {
                    UIAtlasMakerTools.SelectSprite(selection);
                }
                else if (UIAtlasMakerSettings.autoUpgradeSprites && (update || replace))
                {
                    UIAtlasMakerTools.UpgradeTexturesToSprites(UIAtlasMakerSettings.atlas);
                    UIAtlasMakerTools.RepaintSprites();
                }
            }
        }

        if (UIAtlasMakerSettings.atlas != null && textures.Count == 0)
            EditorGUILayout.HelpBox("You can reveal more options by selecting one or more textures in the Project View window.", MessageType.Info);
    }
}
