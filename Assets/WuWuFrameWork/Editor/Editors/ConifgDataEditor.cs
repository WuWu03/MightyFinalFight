using UnityEngine;
using WuWuFramework.Serialize;

namespace WuWuFramework.Editor
{
    public abstract class ConfigDataEditor<T, P> : UnityEditor.Editor where T : BaseScriptableObject<P> where P : BaseScriptableConfigData
    {
        T Config;

        private void OnEnable()
        {
            Config = (target as T);
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("排序"))
            {
                Config.listDatas.Sort();
            }
        }
    }
}