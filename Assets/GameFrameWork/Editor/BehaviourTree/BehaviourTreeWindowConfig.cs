using System.Collections.Generic;

namespace GameFrameWork.Editor.Config
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

        public BehaviourTreeWindowData(string name, int id, float x = 20, float y = 20)
        {
            this.name = name;
            this.id = id;
            children = new List<BehaviourTreeWindowData>();
            preConditions = new List<BehaviourTreeWindowPreConditon>();
            windowRect = new WindowRect(x, y, 230, 205);
        }

        public int id;
        public string name;
        public string classType;
        public string args;
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