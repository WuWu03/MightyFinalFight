using System.Collections.Generic;

namespace GameFrameWork.UI
{
    public abstract class UIBaseData
    {
        private readonly Dictionary<uint, object> m_DataBinders = new();
        public void SetData<T>(uint key, T value)
        {
            IDataBinder<T> dataBinder = GetDataBinder<T>(key);

            if (dataBinder == null)
            {
                dataBinder = new DataBinder<T>(key);
                dataBinder.value = value;
                m_DataBinders.Add(key, dataBinder);
                return;
            }

            dataBinder.value = value;
        }
        
        public T GetData<T>(uint key)
        {
            IDataBinder<T> dataBinder = GetDataBinder<T>(key);
            return dataBinder != null ? dataBinder.value : default;
        }

        private IDataBinder<T> GetDataBinder<T>(uint key)
        {
            if (m_DataBinders.TryGetValue(key, out object dataBinder))
            {
                if (dataBinder is IDataBinder<T> tempDataBinder && tempDataBinder.key == key)
                {
                    return tempDataBinder;
                }

                Log.LogError("数据类型错误，原类型为：", dataBinder.GetType().ToString());
            }

            return null;
        }
    }
}