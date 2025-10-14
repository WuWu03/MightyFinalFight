namespace GameFrameWork.Event
{
    public class EventArg : GameEventArg<EventArg>
    {

    }

    public class EventArg<T1> : EventArg
    {
        public T1 arg1 { get; set; }
    }

    public class EventArg<T1, T2> : EventArg<T1>
    {
        public T2 arg2 { get; set; }
    }

    public class EventArg<T1, T2, T3> : EventArg<T1, T2>
    {
        public T3 arg3 { get; set; }
    }

    public class EventArg<T1, T2, T3, T4> : EventArg<T1, T2, T3>
    {
        public T4 arg4 { get; set; }
    }
}