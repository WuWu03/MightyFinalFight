using GameFrameWork.Serialize;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(BehaviourTreeWindowConfig))]
    public class BehaviourTreeWindowConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            //GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}
