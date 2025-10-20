using System;

namespace GameFrameWork.Event
{
    public interface IEventMgr
    {
        public int currEventCount { get; }
        public int eventHandlerCount { get; }
        public void Subscribe(uint eventId, EventHandler<EventArg> handler);
        public void UnSubscribe(uint eventId, EventHandler<EventArg> handler);
        public bool Check(uint eventId, EventHandler<EventArg> handler);
        public int Count(uint eventId);
        public void Dispatch(object sender, EventArg arg);
        public void DispatchNow(object sender, EventArg arg);
    }
}