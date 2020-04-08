namespace Runtime
{
    public abstract class BaseData
    {
        public string DataName
        {
            get
            {
                return m_DataName;
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
                return !string.IsNullOrEmpty(m_Receiver) && !string.IsNullOrEmpty(m_Sender) && !string.IsNullOrEmpty(m_DataName);
            }
        }

        public BaseData(string dataName)
        {
            m_DataName = dataName;
            m_Receiver = null;
            m_Sender = null;
        }

        public BaseData(string dataName, string sender, string receiver)
        {
            m_DataName = dataName;
            m_Receiver = receiver;
            m_Sender = sender;
        }

        public override string ToString()
        {
            return "Receiver: [" + m_Receiver + "] ,Sender: [" + m_Sender + "] , Command: [" + m_DataName + "]";
        }

        protected string m_Receiver = string.Empty;
        protected string m_Sender = string.Empty;
        protected string m_DataName = string.Empty;
    }
}