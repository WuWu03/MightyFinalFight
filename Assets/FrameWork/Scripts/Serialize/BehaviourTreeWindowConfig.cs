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
        public BehaviourTreeWindowData(string name,int id,float x = 20,float y = 20)
        {
            Name = name;
            ID = id;
            Children = new List<BehaviourTreeWindowData>();
            PreConditions = new List<BehaviourTreeWindowData>();
            WindowRect = new Rect(x, y, 150, 80);
        }

        [SerializeField]
        public int ID;
        [SerializeField]
        public string Name;
        [SerializeField]
        public string ClassType;
        [SerializeField]
        public string Args;
        [SerializeField]
        public Rect ListRect;
        [SerializeField]
        public Rect WindowRect;
        [SerializeField]
        public List<BehaviourTreeWindowData> Children;
        [SerializeField]
        public List<BehaviourTreeWindowData> PreConditions;
    }
}