using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
{
    public class Entry : Composite
    {
        public Entry(string name, string args, object owner, int priority) : base(name, args, owner, priority)
        {

        }
    }
}