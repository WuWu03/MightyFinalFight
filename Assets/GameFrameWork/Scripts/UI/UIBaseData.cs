using System.Collections.Generic;

namespace GameFrameWork.UI
{
    public class UIBaseData
    {
        public void SetData<T>(uint key, T value)
        {
            DataBinder<T> dataBinder = GetDataBinder<T>(key);

            if (dataBinder == null)
            {
                dataBinder = new(key);
                dataBinder.value = value;
                m_DataBinders.Add(key, dataBinder);
                return;
            }

            dataBinder.value = value;
        }
        
        public T GetData<T>(uint key)
        {
            DataBinder<T> dataBinder = GetDataBinder<T>(key);
            return dataBinder != null ? dataBinder.value : default;
        }

        private DataBinder<T> GetDataBinder<T>(uint key)
        {
            if (m_DataBinders.TryGetValue(key, out IDataBinder dataBinder))
            {
                if (dataBinder.key == key && dataBinder is DataBinder<T> tempDataBinder)
                {
                    return tempDataBinder;
                }

                Log.LogError("数据类型错误，原类型为：", dataBinder.GetType().ToString());
            }

            return null;
        }

        private Dictionary<uint, IDataBinder> m_DataBinders = new();
    }
}