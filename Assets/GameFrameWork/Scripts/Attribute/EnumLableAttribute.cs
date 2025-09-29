using System;
using UnityEngine;

namespace GameFrameWork
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class EnumLabelAttribute : PropertyAttribute
    {
        public string label
        {
            get
            {
                return m_Label;
            }
            set
            {
                m_Label = value;
            }
        }

        public int[] orders
        {
            get
            {
                return m_Orders;
            }
            set
            {
                m_Orders = value;
            }
        }

        public EnumLabelAttribute(string label)
        {
            m_Label = label;
        }

        public EnumLabelAttribute(string label, params int[] orders)
        {
            m_Label = label;
            m_Orders = orders;
        }

        private string m_Label = string.Empty;
        private int[] m_Orders = null;
    }

}