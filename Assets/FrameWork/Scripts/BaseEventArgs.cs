using System;

namespace FrameWork
{
    public abstract class BaseEventArgs:EventArgs
    {
        public int ID { get; set; }
        public virtual void Clear() { }
        public virtual BaseEventArgs Clone() { return null; }
    }
}