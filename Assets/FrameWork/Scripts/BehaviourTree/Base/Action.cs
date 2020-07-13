using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.BehaviourTree
{
    public abstract class Action : Node
    {
        public Action(string name,string args,object owner) : base(name, args, owner) { }

        public override void AddChild(Node node) 
        {
            throw new System.Exception("Can not add child to a leaf which type is <Action>");
        }

        public override void AddPreCondition(Node node) 
        {
            throw new System.Exception("Can not add precondition to a leaf which type is <Action>");
        }
    }
}