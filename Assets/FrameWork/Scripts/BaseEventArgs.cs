using System;

namespace FrameWork
{
    public abstract class BaseEventArgs:EventArgs
    {
        public int ID { get; set; }
        public abstract void Clear();
        public abstract BaseEventArgs Clone();
    }
}