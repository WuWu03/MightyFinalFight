using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.BehaviorTree
{
    public class BehaviorTreeConfig : BaseScriptableObject<BehaviorTreeData>
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
    public class BehaviorTreeData : BehaviorTreeBaseData
    {
        public BehaviorTreeData[] Childs;
        public BehaviorTreeBaseData[] PreConditions;
    }
}
