using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.BehaviourTree
{
    public abstract class Action : Composites
    {
        public Action(string name,string args,object owner) : base(name, args, owner) { }
    }
}