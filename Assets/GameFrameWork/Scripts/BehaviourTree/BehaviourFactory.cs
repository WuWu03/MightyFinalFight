using System;
using System.Collections.Generic;

namespace GameFrameWork.BehaviourTree
{
    public static class BehaviourFactory
    {
        public static void AddMap(string className,Type type)
        {
            m_DicBehavior.Add(className, type);
        }

        public static Node GetNodeByClassType(string name, string className, string args, object owner,int priority)
        {
            Type t = Type.GetType("GameFrameWork.BehaviourTree." + className);

            if (t == null)
            {
                t = Type.GetType(className);
            }

            if (t == null)
            {
                Log.LogError("Behaviour entity is invalid!" + (owner as BaseEnemyCtrl).owner.entityName);
                return null;
            }

            return (Node)System.Activator.CreateInstance(t, name, args, owner, priority);
        }

        private static Dictionary<string, Type> m_DicBehavior = new Dictionary<string, Type>();
    }
}
