using WuWuFramework.Resources;

namespace WuWuFramework.ConfigData
{
    public interface IConfigDataMgr
    {
        void SetResourcesMgr(IResourcesMgr resourceMgr);
        T[] Get<T>(string fileName = null) where T : BaseConfigData, new();
        void Cache<T>(string fileName = null) where T : BaseConfigData, new();
        bool Remove<T>(string fileName = null) where T : BaseConfigData, new();
        void RemoveAll();
    }
}