namespace GameFrameWork.Event
{
    public class GameEventArgs : BaseEventArgs
    {
        public object arg1
        {
            get
            {
                return m_Arg1;
            }
        }

        public object arg2
        {
            get
            {
                return m_Arg2;
            }
        }

        public object arg3
        {
            get
            {
                return m_Arg3;
            }
        }

        public object arg4
        {
            get
            {
                return m_Arg4;
            }
        }

        public object arg5
        {
            get
            {
                return m_Arg5;
            }
        }

        public object arg6
        {
            get
            {
                return m_Arg6;
            }
        }

        public object arg7
        {
            get
            {
                return m_Arg7;
            }
        }

        public static GameEventArgs Create(int id)
        {
            return Create(id, null);
        }

        public static GameEventArgs Create(int id, object arg1)
        {
            return Create(id, arg1, null);
        }

        public static GameEventArgs Create(int id, object arg1, object arg2)
        {
            return Create(id, arg1, arg2, null);
        }

        public static GameEventArgs Create(int id, object arg1, object arg2, object arg3)
        {
            return Create(id, arg1, arg2, arg3, null);
        }

        public static GameEventArgs Create(int id, object arg1, object arg2, object arg3, object arg4)
        {
            return Create(id, arg1, arg2, arg3, arg4, null);
        }

        public static GameEventArgs Create(int id, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return Create(id, arg1, arg2, arg3, arg4, arg5, null);
        }

        public static GameEventArgs Create(int id, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return Create(id, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static GameEventArgs Create(int id, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            GameEventArgs args = ReferencePool.Acquire<GameEventArgs>();
            args.id = id;
            args.m_Arg1 = arg1;
            args.m_Arg2 = arg2;
            args.m_Arg3 = arg3;
            args.m_Arg4 = arg4;
            args.m_Arg5 = arg5;
            args.m_Arg6 = arg6;
            args.m_Arg7 = arg7;
            return args;
        }

        public override void Clear()
        {
            id = 0;
            m_Arg1 = null;
            m_Arg2 = null;
            m_Arg3 = null;
            m_Arg4 = null;
            m_Arg5 = null;
            m_Arg6 = null;
            m_Arg7 = null;
        }

        private object m_Arg1 = null;
        private object m_Arg2 = null;
        private object m_Arg3 = null;
        private object m_Arg4 = null;
        private object m_Arg5 = null;
        private object m_Arg6 = null;
        private object m_Arg7 = null;
    }
}