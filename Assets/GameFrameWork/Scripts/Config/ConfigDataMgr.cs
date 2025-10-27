using System;
using System.Collections.Generic;
using GameFrameWork.Assets;
using GameFrameWork.Utils;

namespace GameFrameWork.ConfigData
{
    public class ConfigDataMgr : GameFrameWorkModule , IConfigDataMgr
    {
        private readonly Dictionary<string, object> m_ConfigData;
        private IResourceMgr m_ResourceMgr;
        
        public ConfigDataMgr()
        {
            m_ConfigData = new();
        }

        public override void Shutdown()
        {
            RemoveAll();
        }
        
        public void SetResourceMgr(IResourceMgr resourceMgr)
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
            return PathUtil.FormatPath(GameFrameWorkEntry.config.configDataPath, fileName, ".bytes");
        }
    }
}