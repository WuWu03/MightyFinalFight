using System;

namespace GameFrameWork
{
    public abstract class GameFrameWorkEventArgs : EventArgs, IReference
    {
        public GameFrameWorkEventArgs()
        {

        }

        public abstract void Clear();
        public virtual GameFrameWorkEventArgs Clone()
        {
            return null;
        }
    }
}