using WuWuFramework.Event;

namespace WuWuFramework
{
    public class DataBinder<T>
    {
        private readonly uint m_Key;
        private event WuWuFrameworkAction<T> m_Event;
        private T m_Value;

        public DataBinder(uint key)
        {
            m_Key = key;
        }

        public uint key
        {
            get { return m_Key; }
        }

        public T value
        {
            get { return m_Value; }
            set
            {
                if (!this.value.Equals(value))
                {
                    m_Event.Invoke(value);
                }

                m_Value = value;
            }
        }
    }
}