//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// UI Atlas contains a collection of sprites inside one large texture atlas.
/// </summary>

[AddComponentMenu("NGUI/UI/Atlas")]
public class UIAtlas : MonoBehaviour
{
    // Legacy functionality, removed in 3.0. Do not use.
    [System.Serializable]
    class Sprite
    {
        public string name = "Unity Bug";
        public Rect outer = new Rect(0f, 0f, 1f, 1f);
        public Rect inner = new Rect(0f, 0f, 1f, 1f);
        public bool rotated = false;

        // Padding is needed for trimmed sprites and is relative to sprite width and height
        public float paddingLeft = 0f;
        public float paddingRight = 0f;
        public float paddingTop = 0f;
        public float paddingBottom = 0f;

        public bool hasPadding { get { return paddingLeft != 0f || paddingRight != 0f || paddingTop != 0f || paddingBottom != 0f; } }
    }

    /// <summary>
    /// Legacy functionality, removed in 3.0. Do not use.
    /// </summary>

    enum Coordinates
    {
        Pixels,
        TexCoords,
    }

    /// <summary>
    /// List of sprites within the atlas.
    /// </summary>

    public List<AtlasSprite> spriteList
    {
        get
        {
            if (mReplacement != null) return mReplacement.spriteList;
            if (mSprites.Count == 0) Upgrade();
            return mSprites;
        }
        set
        {
            if (mReplacement != null)
            {
                mReplacement.spriteList = value;
            }
            else
            {
                mSprites = value;
            }
        }
    }

    /// <summary>
    /// Whether the atlas is using a premultiplied alpha material.
    /// </summary>
    int mPMA = -1;
    public bool premultipliedAlpha
    {
        get
        {
            if (mReplacement != null) return mReplacement.premultipliedAlpha;

            if (mPMA == -1)
            {
                Material mat = spriteMaterial;
                mPMA = (mat != null && mat.shader != null && mat.shader.name.Contains("Premultiplied")) ? 1 : 0;
            }
            return (mPMA == 1);
        }
    }

    /// <summary>
    /// Material used by the atlas.
    /// </summary>

    public Material spriteMaterial
    {
        get
        {
            return (mReplacement != null) ? mReplacement.spriteMaterial : material;
        }
        set
        {
            if (mReplacement != null)
            {
                mReplacement.spriteMaterial = value;
            }
            else
            {
                if (material == null)
                {
                    mPMA = 0;
                    material = value;
                }
                else
                {
                    MarkAsChanged();
                    mPMA = -1;
                    material = value;
                    MarkAsChanged();
                }
            }
        }
    }

    // Material used by this atlas. Name is kept only for backwards compatibility, it used to be public.
    [HideInInspector]
    [SerializeField]
    Material material;

    // Replacement atlas can be used to completely bypass this atlas, pulling the data from another one instead.
    [HideInInspector]
    [SerializeField]
    UIAtlas mReplacement;

    // List of all sprites inside the atlas. Name is kept only for backwards compatibility, it used to be public.
    [HideInInspector]
    [SerializeField]
    List<AtlasSprite> mSprites = new List<AtlasSprite>();

    // Legacy functionality -- do not use
    [HideInInspector]
    [SerializeField]
    Coordinates mCoordinates = Coordinates.Pixels;
    [HideInInspector]
    [SerializeField]
    List<Sprite> sprites = new List<Sprite>();


    // Dictionary lookup to speed up sprite retrieval at run-time
    Dictionary<string, int> mSpriteIndices = new Dictionary<string, int>();

    /// <summary>
    /// Texture used by the atlas.
    /// </summary>

    public Texture texture { get { return (mReplacement != null) ? mReplacement.texture : (material != null ? material.mainTexture as Texture : null); } }

    /// <summary>
    /// Sort the list of sprites within the atlas, making them alphabetical.
    /// </summary>

    public void SortAlphabetically()
    {
        mSprites.Sort(delegate (AtlasSprite s1, AtlasSprite s2) { return s1.name.CompareTo(s2.name); });
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    /// <summary>
    /// Mark all widgets associated with this atlas as having changed.
    /// </summary>

    public void MarkAsChanged()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    /// <summary>
    /// Convenience function that retrieves a list of all sprite names.
    /// </summary>

    public List<string> GetListOfSprites()
    {
        if (mReplacement != null) return mReplacement.GetListOfSprites();
        if (mSprites.Count == 0) Upgrade();

        List<string> list = new List<string>();

        for (int i = 0, imax = mSprites.Count; i < imax; ++i)
        {
            AtlasSprite s = mSprites[i];
            if (s != null && !string.IsNullOrEmpty(s.name)) list.Add(s.name);
        }
        return list;
    }

    /// <summary>
    /// Convenience function that retrieves a sprite by name.
    /// </summary>

    public AtlasSprite GetSprite(string name)
    {
        if (mReplacement != null)
        {
            return mReplacement.GetSprite(name);
        }
        else if (!string.IsNullOrEmpty(name))
        {
            if (mSprites.Count == 0) Upgrade();
            if (mSprites.Count == 0) return null;

            // O(1) lookup via a dictionary
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                // The number of indices differs from the sprite list? Rebuild the indices.
                if (mSpriteIndices.Count != mSprites.Count)
                    MarkSpriteListAsChanged();

                int index;
                if (mSpriteIndices.TryGetValue(name, out index))
                {
                    // If the sprite is present, return it as-is
                    if (index > -1 && index < mSprites.Count) return mSprites[index];

                    // The sprite index was out of range -- perhaps the sprite was removed? Rebuild the indices.
                    MarkSpriteListAsChanged();

                    // Try to look up the index again
                    return mSpriteIndices.TryGetValue(name, out index) ? mSprites[index] : null;
                }
            }

            // Sequential O(N) lookup.
            for (int i = 0, imax = mSprites.Count; i < imax; ++i)
            {
                AtlasSprite s = mSprites[i];

                // string.Equals doesn't seem to work with Flash export
                if (!string.IsNullOrEmpty(s.name) && name == s.name)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying) return s;
#endif
                    // If this point was reached then the sprite is present in the non-indexed list,
                    // so the sprite indices should be updated.
                    MarkSpriteListAsChanged();
                    return s;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Rebuild the sprite indices. Call this after modifying the spriteList at run time.
    /// </summary>

    public void MarkSpriteListAsChanged()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {
            mSpriteIndices.Clear();
            for (int i = 0, imax = mSprites.Count; i < imax; ++i)
                mSpriteIndices[mSprites[i].name] = i;
        }
    }

    /// <summary>
    /// Convenience function that retrieves a list of all sprite names that contain the specified phrase
    /// </summary>

    public List<string> GetListOfSprites(string match)
    {
        if (mReplacement) return mReplacement.GetListOfSprites(match);
        if (string.IsNullOrEmpty(match)) return GetListOfSprites();

        if (mSprites.Count == 0) Upgrade();
        List<string> list = new List<string>();

        // First try to find an exact match
        for (int i = 0, imax = mSprites.Count; i < imax; ++i)
        {
            AtlasSprite s = mSprites[i];

            if (s != null && !string.IsNullOrEmpty(s.name) && string.Equals(match, s.name, StringComparison.OrdinalIgnoreCase))
            {
                list.Add(s.name);
                return list;
            }
        }

        // No exact match found? Split up the search into space-separated components.
        string[] keywords = match.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < keywords.Length; ++i) keywords[i] = keywords[i].ToLower();

        // Try to find all sprites where all keywords are present
        for (int i = 0, imax = mSprites.Count; i < imax; ++i)
        {
            AtlasSprite s = mSprites[i];

            if (s != null && !string.IsNullOrEmpty(s.name))
            {
                string tl = s.name.ToLower();
                int matches = 0;

                for (int b = 0; b < keywords.Length; ++b)
                {
                    if (tl.Contains(keywords[b])) ++matches;
                }
                if (matches == keywords.Length) list.Add(s.name);
            }
        }
        return list;
    }

    /// <summary>
    /// Performs an upgrade from the legacy way of specifying data to the new one.
    /// </summary>

    bool Upgrade()
    {
        if (mReplacement) return mReplacement.Upgrade();

        if (mSprites.Count == 0 && sprites.Count > 0 && material)
        {
            Texture tex = material.mainTexture;
            int width = (tex != null) ? tex.width : 512;
            int height = (tex != null) ? tex.height : 512;

            for (int i = 0; i < sprites.Count; ++i)
            {
                Sprite old = sprites[i];
                Rect outer = old.outer;
                Rect inner = old.inner;

                if (mCoordinates == Coordinates.TexCoords)
                {
                    AltasMath.ConvertToPixels(outer, width, height, true);
                    AltasMath.ConvertToPixels(inner, width, height, true);
                }

                AtlasSprite sd = new AtlasSprite();
                sd.name = old.name;

                sd.x = Mathf.RoundToInt(outer.xMin);
                sd.y = Mathf.RoundToInt(outer.yMin);
                sd.width = Mathf.RoundToInt(outer.width);
                sd.height = Mathf.RoundToInt(outer.height);

                sd.paddingLeft = Mathf.RoundToInt(old.paddingLeft * outer.width);
                sd.paddingRight = Mathf.RoundToInt(old.paddingRight * outer.width);
                sd.paddingBottom = Mathf.RoundToInt(old.paddingBottom * outer.height);
                sd.paddingTop = Mathf.RoundToInt(old.paddingTop * outer.height);

                sd.borderLeft = Mathf.RoundToInt(inner.xMin - outer.xMin);
                sd.borderRight = Mathf.RoundToInt(outer.xMax - inner.xMax);
                sd.borderBottom = Mathf.RoundToInt(outer.yMax - inner.yMax);
                sd.borderTop = Mathf.RoundToInt(inner.yMin - outer.yMin);

                mSprites.Add(sd);
            }
            sprites.Clear();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class AtlasSprite
{
    public string name = "Sprite";
    public int x = 0;
    public int y = 0;
    public int width = 0;
    public int height = 0;

    public int borderLeft = 0;
    public int borderRight = 0;
    public int borderTop = 0;
    public int borderBottom = 0;

    public int paddingLeft = 0;
    public int paddingRight = 0;
    public int paddingTop = 0;
    public int paddingBottom = 0;

    //bool rotated = false;

    /// <summary>
    /// Whether the sprite has a border.
    /// </summary>

    public bool hasBorder { get { return (borderLeft | borderRight | borderTop | borderBottom) != 0; } }

    /// <summary>
    /// Whether the sprite has been offset via padding.
    /// </summary>

    public bool hasPadding { get { return (paddingLeft | paddingRight | paddingTop | paddingBottom) != 0; } }

    /// <summary>
    /// Convenience function -- set the X, Y, width, and height.
    /// </summary>

    public void SetRect(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    /// <summary>
    /// Convenience function -- set the sprite's padding.
    /// </summary>

    public void SetPadding(int left, int bottom, int right, int top)
    {
        paddingLeft = left;
        paddingBottom = bottom;
        paddingRight = right;
        paddingTop = top;
    }

    /// <summary>
    /// Convenience function -- set the sprite's border.
    /// </summary>

    public void SetBorder(int left, int bottom, int right, int top)
    {
        borderLeft = left;
        borderBottom = bottom;
        borderRight = right;
        borderTop = top;
    }

    /// <summary>
    /// Copy all values of the specified sprite data.
    /// </summary>

    public void CopyFrom(AtlasSprite sd)
    {
        name = sd.name;

        x = sd.x;
        y = sd.y;
        width = sd.width;
        height = sd.height;

        borderLeft = sd.borderLeft;
        borderRight = sd.borderRight;
        borderTop = sd.borderTop;
        borderBottom = sd.borderBottom;

        paddingLeft = sd.paddingLeft;
        paddingRight = sd.paddingRight;
        paddingTop = sd.paddingTop;
        paddingBottom = sd.paddingBottom;
    }

    /// <summary>
    /// Copy the border information from the specified sprite.
    /// </summary>

    public void CopyBorderFrom(AtlasSprite sd)
    {
        borderLeft = sd.borderLeft;
        borderRight = sd.borderRight;
        borderTop = sd.borderTop;
        borderBottom = sd.borderBottom;
    }
}

public static class AltasMath
{
    /// <summary>
    /// Convert from bottom-left based UV coordinates to top-left based pixel coordinates.
    /// </summary>

    static public Rect ConvertToPixels(Rect rect, int width, int height, bool round)
    {
        Rect final = rect;

        if (round)
        {
            final.xMin = Mathf.RoundToInt(rect.xMin * width);
            final.xMax = Mathf.RoundToInt(rect.xMax * width);
            final.yMin = Mathf.RoundToInt((1f - rect.yMax) * height);
            final.yMax = Mathf.RoundToInt((1f - rect.yMin) * height);
        }
        else
        {
            final.xMin = rect.xMin * width;
            final.xMax = rect.xMax * width;
            final.yMin = (1f - rect.yMax) * height;
            final.yMax = (1f - rect.yMin) * height;
        }
        return final;
    }
}
