using WuWuFramework.Resources;

namespace WuWuFramework.ConfigData
{
    public interface IConfigDataMgr
    {
        public void SetResourceMgr(IResourcesMgr resourceMgr);
        public T[] Get<T>(string fileName = "") where T : BaseConfigData, new();
        public bool Remove<T>(string fileName = "") where T : BaseConfigData, new();
        public void RemoveAll();
    }
}