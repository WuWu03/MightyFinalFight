using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    public abstract class GameFrameWorkEventArgs : EventArgs, IReference
    {
        public abstract void Clear();
        public virtual GameFrameWorkEventArgs Clone() { return null; }
    }
}