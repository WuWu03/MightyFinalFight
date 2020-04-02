namespace FrameWork.Fsm
{
    public abstract class BaseFsmStateParam
    {
        public ObjectMsgType Cmd
        {
            get
            {
                return m_Cmd;
            }
        }
        public object Sender
        {
            get
            {
                return m_Sender;
            }
        }

        public string Receiver
        {
            get
            {
                return m_Receiver;
            }
        }

        public bool CanSend
        {
            get
            {
                return !string.IsNullOrEmpty(m_Receiver) && !string.IsNullOrEmpty(m_Sender) && m_Cmd != ObjectMsgType.NONE;
            }
        }

        public BaseFsmStateParam(ObjectMsgType cmd)
        {
            m_Cmd = cmd;
            m_Receiver = null;
            m_Sender = null;
        }

        public BaseFsmStateParam(ObjectMsgType cmd, string sender,string receiver)
        {
            m_Cmd = cmd;
            m_Receiver = receiver;
            m_Sender = sender;
        }


        public override string ToString()
        {
            return "Receiver: [" + m_Receiver + "] ,Sender: [" + m_Sender + "] , Command: [" + m_Cmd.ToString() + "]";
        }

        protected string m_Receiver = string.Empty;
        protected string m_Sender = string.Empty;
        protected ObjectMsgType m_Cmd = ObjectMsgType.NONE;
    }
}