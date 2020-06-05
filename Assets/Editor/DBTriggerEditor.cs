using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DBTrigger))]
public class DBTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        base.OnInspectorGUI();
        //GUI.enabled = true;
    }
}
