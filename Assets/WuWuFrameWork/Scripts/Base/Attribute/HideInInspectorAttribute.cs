using UnityEngine;

namespace WuWuFramework
{
    /// <summary>
    /// 在Inspector中隐藏属性的自定义属性类。可以通过设置条件来控制属性是否在Inspector中显示。
    /// </summary>
    public class HideInInspectorAttribute : PropertyAttribute
    {
        public bool condition
        {
            get
            {
                return m_Condition;
            }
            set
            {
                m_Condition = value;
            }
        }

        public HideInInspectorAttribute()
        {
            m_Condition = false;
        }

        public HideInInspectorAttribute(bool condition)
        {
            m_Condition = condition;
        }

        private bool m_Condition = false;
    }
}
