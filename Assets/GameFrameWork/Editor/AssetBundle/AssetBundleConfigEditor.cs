using UnityEditor;
using UnityEngine;


namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(AssetBundleConfig))]
    public class AssetBundleConfigEditor : ConfigDataEditor<AssetBundleConfig, AssetBundleData>
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}
