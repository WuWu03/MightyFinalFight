namespace GameFrameWork.Event
{
    public class GameFrameWorkCall : GameFrameWorkBaseCall<GameFrameWorkAction>
    {
        public static GameFrameWorkCall Create(GameFrameWorkAction action)
        {
            GameFrameWorkCall call = ReferencePool.Acquire<GameFrameWorkCall>();
            call.action = action;
            return call;
        }

        public override void Invoke()
        {
            action?.Invoke();
        }
    }

    public class GameFrameWorkCall<T1> : GameFrameWorkBaseCall<GameFrameWorkAction<T1>>
    {
        public T1 arg1 { get; set; }

        public static GameFrameWorkCall<T1> Create(GameFrameWorkAction<T1> action)
        {
            GameFrameWorkCall<T1> call = ReferencePool.Acquire<GameFrameWorkCall<T1>>();
            call.action = action;
            return call;
        }

        public override void Invoke()
        {
            action?.Invoke(arg1);
            ResetArg();
        }

        public override void Clear()
        {
            base.Clear();
            ResetArg();
        }

        private void ResetArg()
        {
            arg1 = default;
        }
    }

    public class GameFrameWorkCall<T1, T2> : GameFrameWorkBaseCall<GameFrameWorkAction<T1,T2>>
    {
        public T1 arg1 { get; set; }
        public T2 arg2 { get; set; }

        public static GameFrameWorkCall<T1, T2> Create(GameFrameWorkAction<T1,T2> action)
        {
            GameFrameWorkCall<T1, T2> call = ReferencePool.Acquire<GameFrameWorkCall<T1, T2>>();
            call.action = action;
            return call;
        }

        public override void Invoke()
        {
            (action as GameFrameWorkAction<T1, T2>)?.Invoke(arg1, arg2);
            ResetArg();
        }

        public override void Clear()
        {
            base.Clear();
            ResetArg();
        }

        private void ResetArg()
        {
            arg1 = default;
            arg2 = default;
        }
    }

    public class GameFrameWorkCall<T1, T2, T3> : GameFrameWorkBaseCall<GameFrameWorkAction<T1,T2, T3>>
    {
        public T1 arg1 { get; set; }
        public T2 arg2 { get; set; }
        public T3 arg3 { get; set; }

        public static GameFrameWorkCall<T1, T2, T3> Create(GameFrameWorkAction<T1,T2, T3> action)
        {
            GameFrameWorkCall<T1, T2, T3> call = ReferencePool.Acquire<GameFrameWorkCall<T1, T2, T3>>();
            call.action = action;
            return call;
        }

        public override void Invoke()
        {
            action?.Invoke(arg1, arg2, arg3);
            ResetArg();
        }

        public override void Clear()
        {
            base.Clear();
            ResetArg();
        }

        private void ResetArg()
        {
            arg1 = default;
            arg2 = default;
            arg3 = default;
        }
    }
}