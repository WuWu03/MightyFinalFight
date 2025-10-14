namespace GameFrameWork.Event
{
    public class GameFrameWorkEvent : GameFrameWorkBaseEvent<GameFrameWorkCall, GameFrameWorkAction>
    {
        public void Invoke()
        {
            foreach (GameFrameWorkCall call in calls)
            {
                call.Invoke();
            }
        }
    }

    public class GameFrameWorkEvent<T1> : GameFrameWorkBaseEvent<GameFrameWorkCall<T1>, GameFrameWorkAction<T1>>
    {
        public void Invoke(T1 arg1)
        {
            foreach (GameFrameWorkCall<T1> call in calls)
            {
                call.arg1 = arg1;
                call.Invoke();
            }
        }
    }

    public class GameFrameWorkEvent<T1, T2> : GameFrameWorkBaseEvent<GameFrameWorkCall<T1, T2>, GameFrameWorkAction<T1, T2>>
    {
        public void Invoke(T1 arg1,T2 arg2)
        {
            foreach (GameFrameWorkCall<T1,T2> call in calls)
            {
                call.arg1 = arg1;
                call.arg2 = arg2;
                call.Invoke();
            }
        }
    }

    public class GameFrameWorkEvent<T1, T2, T3> : GameFrameWorkBaseEvent<GameFrameWorkCall<T1, T2, T3>, GameFrameWorkAction<T1, T2, T3>>
    {
        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            foreach (GameFrameWorkCall<T1, T2, T3> call in calls)
            {
                call.arg1 = arg1;
                call.arg2 = arg2;
                call.arg3 = arg3;
                call.Invoke();
            }
        }
    }
}