using System;

namespace GameFrameWork.Serialize
{
    [Serializable]
    public abstract class BaseScriptableConfigData : IComparable
    {
        public int id;

        public virtual int CompareTo(object obj)
        {
            if (obj is BaseScriptableConfigData data)
            {
                return id.CompareTo(data.id);
            }

            return 0;
        }
    }
}