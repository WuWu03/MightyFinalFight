using System;
using UnityEngine;
using GameFrameWork.Serialize;

namespace GameFrameWork.Editor
{
    public abstract class ConfigDataEditor<T, P> : UnityEditor.Editor where T : BaseScriptableObject<P> where P : BaseConfigData
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