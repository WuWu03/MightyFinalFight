using System.Collections.Generic;

namespace WuWuFramework.Editor.Config
{
    public class BehaviourTreeWindowConfig
    {
        public List<BehaviourTreeWindowData> dataList;
    }

    public class BehaviourTreeWindowData
    {
        public BehaviourTreeWindowData()
        {

        }

        public BehaviourTreeWindowData(string name, string classType,int id, float x = 20, float y = 20)
        {
            this.name = name;
            this.id = id;
            this.classType = classType;
            children = new List<BehaviourTreeWindowData>();
            preConditions = new List<BehaviourTreeWindowPreConditon>();
            windowRect = new WindowRect(x, y, 230, 270);
        }

        public int id;
        public string name;
        public string classType;
        public string args;
        public int priority;
        public WindowRect listRect;
        public WindowRect windowRect;
        public List<BehaviourTreeWindowData> children;
        public List<BehaviourTreeWindowPreConditon> preConditions;
    }

    public class BehaviourTreeWindowPreConditon
    {
        public string classType;
        public int selectIndex;
        public string args;
        public bool isAndCondition;
    }

    public class WindowRect
    {
        public float x;
        public float y;
        public float width;
        public float height;

        public WindowRect()
        {

        }

        public WindowRect(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        } 
    }
}