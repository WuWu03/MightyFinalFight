using System;
using UnityEngine;

namespace WuWuFramework
{
    /// <summary>
    /// 枚举标签属性，用于为枚举类型或枚举字段添加自定义标签和排序信息，以便在编辑器中显示更友好的名称和顺序。
    /// </summary>
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