namespace GameFrameWork.Event
{
    public class GameFrameWorkCall : GameFrameWorkBaseCall<GameFrameWorkAction>
    {
        public override void Invoke()
        {
            action?.Invoke();
        }
    }

    public class GameFrameWorkCall<T1> : GameFrameWorkBaseCall<GameFrameWorkAction<T1>>
    {
        public T1 arg1 { get; set; }
        
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
        
        public override void Invoke()
        {
            action?.Invoke(arg1, arg2);
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
    
    public class GameFrameWorkCall<T1, T2, T3,T4> : GameFrameWorkBaseCall<GameFrameWorkAction<T1,T2, T3,T4>>
    {
        public T1 arg1 { get; set; }
        public T2 arg2 { get; set; }
        public T3 arg3 { get; set; }
        public T4 arg4 { get; set; }

        public override void Invoke()
        {
            action?.Invoke(arg1, arg2, arg3, arg4);
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
            arg4 = default;
        }
    }
}