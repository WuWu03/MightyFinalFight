
//数据实体基类
using GameFrameWork.UI;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameFrameWork.LocalData
{
    public abstract class BaseLocalData
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

        public abstract void Read(LocalDataParser parser);

        private int m_Id;
    }
}