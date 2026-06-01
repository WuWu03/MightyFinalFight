using System.Collections.Generic;

namespace WuWuFramework.Editor
{
    public class BehaviourTreeUtil
    {
        public static string[] GetNodePathList(bool isRoot, params string[] paths)
        {
            List<string> list = new List<string>();
            string[][] nodePaths = GetNodePaths(isRoot, paths);

            for (int i = 0; i < nodePaths.Length; i++)
            {
                list.AddRange(nodePaths[i]);
            }

            return list.ToArray();
        }

        public static string[] GetNodePathNameList()
        {
            List<string> list = new List<string>();
            string[][] nodeNames = GetNodeNames();

            for (int i = 0; i < nodeNames.Length; i++)
            {
                list.AddRange(nodeNames[i]);
            }

            return list.ToArray();
        }

        public static string[][] GetNodePaths(bool isRoot,params string[] paths)
        {
            string[][] nodeNames = GetNodeNames();
            List<string[]> nodePaths = new List<string[]>();// new string[nodeNames.Length][];
            string[] classList = new string[3]
            {
                "Composites",
                "Actions",
                "Decorators"
            };

            string str = string.Empty;

            if (paths.Length > 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    str += "{";
                    str += i.ToString();
                    str += "}/";
                }

                str = string.Format(str, paths);
            }

            for (int i = 0; i < 2; i++)
            {
                str += "{";
                str += i.ToString();
                str += "}";

                if (i < 1)
                {
                    str += "/";
                }
            }

            for (int i = 0; i < nodeNames.Length; i++)
            {
                if(isRoot && i == 1)
                {
                    continue;
                }

                nodePaths.Add(new string[nodeNames[i].Length]);

                for (int j = 0; j < nodeNames[i].Length; j++)
                {
                    nodePaths[nodePaths.Count - 1][j] = string.Format(str, classList[i], nodeNames[i][j]);
                }
            }

            return nodePaths.ToArray();
        }

        public static string[][] GetNodeNames()
        {
            if (s_NodeNames == null)
            {
                s_NodeNames = new string[3][];
                s_NodeNames[0] = EditorUtil.GetAssemblyTypeNames("WuWuFramework.BehaviourTree.Composite", false, "Composite", "Entry");
                s_NodeNames[1] = EditorUtil.GetAssemblyTypeNames("WuWuFramework.BehaviourTree.Action", false, "Action");
                s_NodeNames[2] = EditorUtil.GetAssemblyTypeNames("WuWuFramework.BehaviourTree.Decorator", false, "Decorator");
            }

            return s_NodeNames;
        }

        public static string[] GetPreConditionNames()
        {
            if (m_PreConditionNames == null)
            {
                m_PreConditionNames = EditorUtil.GetAssemblyTypeNames("WuWuFramework.BehaviourTree.PreCondition", false, "PreCondition");
            }

            return m_PreConditionNames;
        }

        private static string[][] s_NodeNames = null;
        private static string[] m_PreConditionNames = null;
    }
}