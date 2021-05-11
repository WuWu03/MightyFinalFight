using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Serialize
{
    [Serializable]
    public class BehaviourTreeWindowConfig : ScriptableObject
    {
        public string BehaviourConfigPath;
        public List<BehaviourTreeWindowData> WindowDatas;
    }

    [Serializable]
    public class BehaviourTreeWindowData
    {
        [SerializeField]
        public int ID;
        [SerializeField]
        public string Name;
        [SerializeField]
        public string ClassType;
        [SerializeField]
        public string Args;
        [SerializeField]
        public Rect Rect;
        [SerializeField]
        public List<BehaviourTreeWindowData> Childs;
        [SerializeField]
        public List<BehaviourTreeWindowData> PreConditions;
    }
}