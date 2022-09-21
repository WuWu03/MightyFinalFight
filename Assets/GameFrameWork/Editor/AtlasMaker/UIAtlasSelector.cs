using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class UIAtlasSelector : ScriptableWizard
{
    static public UIAtlasSelector instance;

    void OnEnable() { instance = this; }
    void OnDisable() { instance = null; }

    public delegate void Callback(string sprite);
    /// <summary>
    /// Show the sprite selection wizard.
    /// </summary>

    SerializedObject mObject;
    SerializedProperty mProperty;

    //Vector2 mPos = Vector2.zero;
    public Callback mCallback;
    //float mClickTime = 0f;

    static public void ShowSelected()
    {
        if (UIAtlasMakerSettings.atlas != null)
        {

            Show(delegate (string sel) { UIAtlasMakerTools.SelectSprite(sel); });
        }
    }

    /// <summary>
    /// Property-based selection result.
    /// </summary>

    void OnSpriteSelection(string sp)
    {
        if (mObject != null && mProperty != null)
        {
            mObject.Update();
            mProperty.stringValue = sp;
            mObject.ApplyModifiedProperties();
        }
    }

    /// <summary>
    /// Show the sprite selection wizard.
    /// </summary>

    static public void Show(SerializedObject ob, SerializedProperty pro, UIAtlas atlas)
    {
        if (instance != null)
        {
            instance.Close();
            instance = null;
        }

        if (ob != null && pro != null && atlas != null)
        {
            UIAtlasSelector comp = ScriptableWizard.DisplayWizard<UIAtlasSelector>("Select a Sprite");
            UIAtlasMakerSettings.atlas = atlas;
            UIAtlasMakerSettings.selectedSprite = pro.hasMultipleDifferentValues ? null : pro.stringValue;
            comp.mObject = ob;
            comp.mProperty = pro;
            comp.mCallback = comp.OnSpriteSelection;
        }
    }

    /// <summary>
    /// Show the selection wizard.
    /// </summary>

    static public void Show(Callback callback)
    {
        if (instance != null)
        {
            instance.Close();
            instance = null;
        }

        UIAtlasSelector comp = ScriptableWizard.DisplayWizard<UIAtlasSelector>("Select a Sprite");
        comp.mCallback = callback;
    }
}
