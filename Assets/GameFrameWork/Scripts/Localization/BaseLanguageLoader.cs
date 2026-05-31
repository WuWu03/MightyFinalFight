using GameFrameWork.Resources;
using UnityEngine;

namespace GameFrameWork.Localization
{
    public abstract class BaseLanguageLoader
    {
        private readonly string m_DataPath;
        private bool m_IsInit;
        
        protected BaseLanguageLoader(string dataPath)
        {
            m_DataPath = dataPath;
            m_IsInit = false;
        }

        public void Init(IResourcesMgr resourceMgr)
        {
            if (m_IsInit)
            {
                return;
            }

            if (string.IsNullOrEmpty(m_DataPath))
            {
                Log.LogError("语言文件路径配置不存在");
                return;
            }

            TextAsset txt = resourceMgr.Load<TextAsset>(m_DataPath);

            if(txt == null)
            {
                Log.LogError("语言文件不存在");
                return;
            }

            m_IsInit = true;
            OnInit(txt);
        }

        public void Reload(IResourcesMgr resourceMgr)
        {
            Release(resourceMgr);
            Init(resourceMgr);
        }

        public void Release(IResourcesMgr resourceMgr)
        {
            if (!m_IsInit)
            {
                return;
            }

            resourceMgr.Unload(m_DataPath);
            m_IsInit = false;
            OnRelease();
        }

        protected abstract void OnInit(TextAsset textAsset);

        public abstract string GetLanguageText(string key);

        protected abstract void OnRelease();
    }
}