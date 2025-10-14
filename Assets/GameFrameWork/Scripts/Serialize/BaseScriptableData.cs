using System;

namespace GameFrameWork.Serialize
{
    [Serializable]
    public abstract class BaseScriptableConfigData : IComparable
    {
        public int id;

        public virtual int CompareTo(object obj)
        {
            BaseScriptableConfigData data = obj as BaseScriptableConfigData;
            return id.CompareTo(data.id);
        }
    }
}