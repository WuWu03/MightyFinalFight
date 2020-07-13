using DG.Tweening.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.BehaviourTree
{
    public static class BehaviourFactory
    {
        public static void AddMap(string className,Type type)
        {
            m_DicBehavior.Add(className, type);
        }

        public static Node GetNodeByClassType(string name, string className, string args, object owner)
        {
            Type t = Type.GetType(className);
            if (t == null)//!m_DicBehavior.TryGetValue(name, out t))
            {
                FrameWork.Log.Debugger.LogError("Behaviour entity is invalid!");
                return null;
            }

            return (Node)System.Activator.CreateInstance(t, name, args, owner);
        }

        private static Dictionary<string, Type> m_DicBehavior = new Dictionary<string, Type>();
    }
}
