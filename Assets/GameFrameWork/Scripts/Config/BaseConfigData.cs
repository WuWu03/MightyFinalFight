namespace GameFrameWork.ConfigData
{
    public abstract class BaseConfigData
    {
        private int m_Id;
        public int id
        {
            get
            {
                return m_Id;
            }
            protected set
            {
                m_Id = value;
            }
        }

        public abstract void Read(ConfigDataParser parser);
    }
}