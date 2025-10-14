using GameFrameWork.Event;

namespace GameFrameWork.UI
{
    public class DataBinder<T> : IDataBinder
    {
        private uint m_Key = 0;

        public uint key
        {
            get { return m_Key; }
        }

        private T m_Value = default;

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

        private GameFrameWorkEvent<T> m_Event = new();

        public DataBinder(uint key)
        {
            m_Key = key;
        }

        public void Bind(object call)
        {
            m_Event.AddListener(call as GameFrameWorkAction<T>);
        }

        public void UnBind(object call)
        {
            m_Event.RemoveListener(call as GameFrameWorkAction<T>);
        }

        public void UnBindAll()
        {
            m_Event.RemoveAllListeners();
        }
    }
}