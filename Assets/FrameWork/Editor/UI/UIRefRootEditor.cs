using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIRefRoot))]
public class UIRefRootEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        base.OnInspectorGUI();
        GUI.enabled = true;
    }
}
