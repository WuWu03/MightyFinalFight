using DG.Tweening.Core;
using FrameWork.BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.BehaviorTree
{
    public static class BehaviorFactory
    {
        public static void AddMap(string className,Type type)
        {
            m_DicBehavior.Add(className, type);
        }

        public static Node GetNodeByClassType(string name, string args, object owner)
        {
            Type t = null;
            if (!m_DicBehavior.TryGetValue(name, out t))
            {
                Debugger.LogError("Behavior entity is invalid!");
                return null;
            }

            return (Node)System.Activator.CreateInstance(t, name, args, owner);
        }

        private static Dictionary<string, Type> m_DicBehavior = new Dictionary<string, Type>();
    }
}
