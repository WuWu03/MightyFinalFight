using UnityEditor;
using UnityEngine;

namespace WuWuFramework.Editor
{
    [CustomEditor(typeof(WuWuFrameworkConfig))]
    public class WuWuFrameworkConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}