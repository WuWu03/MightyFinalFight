using GameFrameWork.Assets;

namespace GameFrameWork.ConfigData
{
    public interface IConfigDataMgr
    {
        public void SetResourceMgr(IResourceMgr resourceMgr);
        public T[] Get<T>(string fileName = "") where T : BaseConfigData, new();
        public bool Remove<T>(string fileName = "") where T : BaseConfigData, new();
        public void RemoveAll();
    }
}