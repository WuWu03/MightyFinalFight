using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.BehaviourTree
{
    public abstract class Action : Composites
    {
        public Action(string name,string args,object owner) : base(name, args, owner) { }

        //public override void AddChild(Node node) 
        //{
        //    base.AddChild(node);
        //}

        //public override void AddPreCondition(Node node) 
        //{
        //    base.
        //}
    }
}