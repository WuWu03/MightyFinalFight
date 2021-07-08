using System;

namespace GameFrameWork
{
    public abstract class BaseEventArgs : GameFrameWorkEventArgs
    {
        public int Id { get; set; }
    }
}