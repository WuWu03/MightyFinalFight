using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
{
    public class Decorator : Task
    {
        public Decorator(string name, string args, object owner, int priority) : base(name, args, owner, priority) { }
    }
}