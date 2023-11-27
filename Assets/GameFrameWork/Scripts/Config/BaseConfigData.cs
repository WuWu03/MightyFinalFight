using GameFrameWork.ConfigData;

namespace GameFrameWork.ConfigData
{
    public abstract class BaseConfigData
    {
        public int id
        {
            get
            {
                return m_Id;
            }
            set
            {
                m_Id = value;
            }
        }

        public abstract void Read(ConfigDataParser parser);

        private int m_Id;
    }
}