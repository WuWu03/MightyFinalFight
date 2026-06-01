using UnityEngine;

namespace WuWuFramework
{
    public class HideInInspectorExAttribute : PropertyAttribute
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

        public HideInInspectorExAttribute()
        {
            m_Condition = false;
        }

        public HideInInspectorExAttribute(bool condition)
        {
            m_Condition = condition;
        }

        private bool m_Condition = false;
    }
}
