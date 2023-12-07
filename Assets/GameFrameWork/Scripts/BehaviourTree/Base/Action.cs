using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
{
    public abstract class Action : Task
    {
        public Action(string name,string args,object owner, int priority) : base(name, args, owner, priority) { }
    }
}