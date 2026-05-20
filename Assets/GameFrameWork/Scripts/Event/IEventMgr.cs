using System;

namespace GameFrameWork.Event
{
    public interface IEventMgr
    {
        public int currEventCount { get; }
        public int eventHandlerCount { get; }
        public UnSubscribe Subscribe<T>(EventHandler<T> handler) where T : struct;
        public void UnSubscribe<T>(EventHandler<T> handler) where T : struct;
        public bool Check<T>(EventHandler<T> handler) where T : struct;
        public int Count<T>() where T : struct;
        public void Dispatch<T>(object sender, T arg) where T : struct;
        public void DispatchNow<T>(object sender, T arg) where T : struct;
    }
}