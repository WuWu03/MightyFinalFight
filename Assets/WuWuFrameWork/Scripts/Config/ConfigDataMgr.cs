using System;
using System.Collections.Generic;
using WuWuFramework.Resources;
using WuWuFramework.Utils;

namespace WuWuFramework.ConfigData
{
    public class ConfigDataMgr : WuWuFrameworkModule , IConfigDataMgr
    {
        private readonly Dictionary<string, object> m_ConfigData;
        private IResourcesMgr m_ResourceMgr;
        
        public ConfigDataMgr()
        {
            m_ConfigData = new();
        }

        public override void Shutdown()
        {
            RemoveAll();
        }
        
        public void SetResourceMgr(IResourcesMgr resourceMgr)
        {
            m_ResourceMgr = resourceMgr;
            ConfigDataHelper.SetResourcesMgr(resourceMgr);
        }

        public T[] Get<T>(string fileName = "") where T : BaseConfigData, new()
        {
            string filePath = GetFilePath<T>(fileName);
            
            if (m_ConfigData.TryGetValue(filePath, out object configData))
            {
                return configData as T[];
            }
            
            T[] result = ConfigDataHelper.LoadConfigData<T>(filePath);
            
            if (!m_ConfigData.TryAdd(filePath, result))
            {
                throw new Exception("配置数据已经存在");
            }

            return result;
        }

        public bool Remove<T>(string fileName = "") where T : BaseConfigData, new()
        {
            string filePath = GetFilePath<T>(fileName);
            return m_ConfigData.Remove(filePath);
        }

        public void RemoveAll()
        {
            m_ConfigData.Clear();
        }

        private string GetFilePath<T>(string fileName) where T : BaseConfigData, new()
        {
            fileName = string.IsNullOrEmpty(fileName) ? typeof(T).Name : fileName;
            return PathUtil.FormatPath(WuWuFrameworkEntry.config.configDataPath, fileName, ".bytes");
        }
    }
}