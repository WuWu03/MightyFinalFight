using GameFrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(BehaviourTreeConfig), true)]
    public class BehaviourTreeConfigEditor : ConfigDataEditor<BehaviourTreeConfig, BehaviourTreeData> 
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
        }
    }
}
