using GameFrameWork.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class BehaviourTreeUtil
    {
        // Start is called before the first frame update

        public static string[] GetNodePathList(params string[] paths)
        {
            List<string> list = new List<string>();
            string[][] nodePaths = GetNodePaths(paths);

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

        public static string[][] GetNodePaths(params string[] paths)
        {
            string[][] nodeNames = GetNodeNames();
            string[][] nodePaths = new string[nodeNames.Length][];
            string[] classList = new string[3]
            {
                "Composites",
                "Actions",
                "Decorators"
            };

            string str = string.Empty;

            if(paths.Length > 0)
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
                nodePaths[i] = new string[nodeNames[i].Length];

                for (int j = 0; j < nodeNames[i].Length; j++)
                {
                    nodePaths[i][j] = string.Format(str, classList[i], nodeNames[i][j]);
                }
            }

            return nodePaths;
        }

        public static string[][] GetNodeNames()
        {
            if (s_NodeNames == null)
            {
                s_NodeNames = new string[3][];
                s_NodeNames[0] = GetAssembly("GameFrameWork.BehaviourTree.Composite", "Composite","Entry");
                s_NodeNames[1] = GetAssembly("GameFrameWork.BehaviourTree.Action", "Action");
                s_NodeNames[2] = GetAssembly("GameFrameWork.BehaviourTree.Decorator", "Decorator");
            }

            return s_NodeNames;
        }

        public static string[] GetPreConditionNames()
        {
            if(m_PreConditionNames == null)
            {
                m_PreConditionNames = GetAssembly("GameFrameWork.BehaviourTree.PreCondition", "PreCondition");
            }

            return m_PreConditionNames;
        }

        private static string[] GetAssembly(string typeName, params string[] parttern)
        {
            Assembly assembly = Assembly.Load("Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            Type baseType = assembly.GetType(typeName);
            List<string> list = new List<string>();
            Type[] allTypes = assembly.GetTypes();

            foreach (Type type in allTypes)
            {
                Type temp = type;
                while (temp.BaseType != null)
                {
                    if (temp.Name.Equals(baseType.Name))
                    {
                        bool isParttern = false;

                        for (int i = 0; i < parttern.Length; i++)
                        {
                            if (parttern[i].Equals(type.Name))
                            {
                                isParttern = true;
                                break;
                            }
                        }

                        if (!isParttern)
                        {
                            list.Add(type.Name);
                            break;
                        }
                    }

                    temp = temp.BaseType;
                }
            }
            list.Sort();
            return list.ToArray();
        }

        private static string[][] s_NodeNames = null;
        private static string[] m_PreConditionNames = null;
    }
}