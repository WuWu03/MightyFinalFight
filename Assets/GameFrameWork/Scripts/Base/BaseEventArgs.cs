using System;

namespace GameFrameWork
{
    public abstract class BaseEventArgs : GameFrameWorkEventArgs
    {
        public int id { get; set; }
    }
}