using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HitTrigger))]
public class HitTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        base.OnInspectorGUI();
        //GUI.enabled = true;
    }
}
