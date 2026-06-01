using UnityEditor;
using UnityEngine;

namespace WuWuFramework.Editor
{
    [CustomEditor(typeof(WuWuFrameworkConfigWindowData))]
    public class WuWuFrameworkConfigWindowDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}