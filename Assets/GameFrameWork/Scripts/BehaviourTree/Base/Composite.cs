using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
{
    public abstract class Composite : ParentTask
    {
        public Composite(string name, string args, object owner, int priority) : base(name, args, owner, priority) { }
    }
}
