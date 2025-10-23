using GameFrameWork.Event;

namespace GameFrameWork.UI
{
    public class DataBinder<T> : IDataBinder<T>
    {
        private readonly uint m_Key;
        private event GameFrameWorkAction<T> m_Event;
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

        public void Bind(GameFrameWorkAction<T> callback)
        {
            m_Event += callback;
        }

        public void UnBind(GameFrameWorkAction<T> callback)
        {
            m_Event -= callback;
        }

        public void UnBindAll()
        {
            m_Event = null;
        }
    }
}