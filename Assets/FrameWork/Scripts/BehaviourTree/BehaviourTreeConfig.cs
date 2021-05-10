using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork.Serialize;

namespace GameFrameWork.BehaviourTree
{
    public class BehaviourTreeConfig : BaseScriptableObject<BehaviourTreeData>
    {

    }

    [Serializable]
    public class BehaviorTreeBaseData : BaseConfigData
    {
        public string Name;
        public string ClassType;
        public string Args;
    }

    [Serializable]
    public class BehaviourTreeData : BehaviorTreeBaseData
    {
        public BehaviourTreeData[] Childs;
        public BehaviorTreeBaseData[] PreConditions;
    }
}
