namespace GameFrameWork.BehaviourTree
{
    public class BehaviourTreeConfig
    {
        public BehaviourTreeData[] datas;

        public BehaviourTreeData GetDataById(int id)
        {
            if(datas == null || datas.Length < 1)
            {
                return null;
            }
            
            for (int i = 0; i < datas.Length; i++)
            {
                if(datas[i].id == id)
                {
                    return datas[i];
                }
            }

            return null;
        }
    }

    public class BehaviorTreeBaseData
    {
        public int id;
        public string classType;
        public string args;
        public int priority;
        public int repeatCount;
    }

    public class BehaviorTreePreConditionData : BehaviorTreeBaseData
    {
        public bool isAndCondition;
    }

    public class BehaviourTreeData : BehaviorTreeBaseData
    {
        public BehaviourTreeData[] children;
        public BehaviorTreePreConditionData[] preConditions;
    }
}