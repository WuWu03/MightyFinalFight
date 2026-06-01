using System.Collections.Generic;
using WuWuFramework.Serialize;

namespace WuWuFramework.BehaviourTree
{
    public class BehaviorTreeBaseData
    {
        public int id;
        public string classType;
        public string args;
        public int priority;
    }

    public class BehaviorTreePreConditionData : BehaviorTreeBaseData
    {
        public bool isAndCondition;
    }

    public class BehaviourTreeData : BehaviorTreeBaseData
    {
        public BehaviourTreeData[] children;
        public BehaviorTreePreConditionData[] preConditions;

        public void DeSerialize(byte[] buffer)
        {
            MemoryStreamEx mse = ReferencePool.Acquire<MemoryStreamEx>();
            mse.Write(buffer, 0, buffer.Length);
            mse.Position = 0;
            Queue<BehaviourTreeData> dataQueue = new();
            dataQueue.Enqueue(this);

            while (dataQueue.Count > 0)
            {
                BehaviourTreeData data = dataQueue.Dequeue();
                data.id = mse.ReadInt();
                data.classType = mse.ReadUTF8String();
                data.args = mse.ReadUTF8String();
                data.priority = mse.ReadInt();
                int childrenCount = mse.ReadInt();
                int preConditionsCount = mse.ReadInt();
                
                if (preConditionsCount > 0)
                {
                    data.preConditions = new BehaviorTreePreConditionData[preConditionsCount];

                    for (int i = 0; i < data.preConditions.Length; i++)
                    {
                        BehaviorTreePreConditionData preConditionData = new()
                        {
                            id = mse.ReadInt(),
                            classType = mse.ReadUTF8String(),
                            args = mse.ReadUTF8String(),
                            priority = mse.ReadInt(),
                            isAndCondition = mse.ReadBool()
                        };
                        data.preConditions[i] = preConditionData;
                    }
                }
                
                if (childrenCount > 0)
                {
                    data.children = new BehaviourTreeData[childrenCount];
                    
                    for (int i = 0; i < data.children.Length; i++)
                    {
                        data.children[i] = new BehaviourTreeData();
                        dataQueue.Enqueue(data.children[i]);
                    }
                }
            }
            
            mse.Release();
        }
    }
}