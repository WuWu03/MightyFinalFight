using GameFrameWork.Assets;
using UnityEngine;

namespace GameFrameWork.Localization
{
    public abstract class BaseLanguageLoader
    {
        public BaseLanguageLoader(string dataPath)
        {
            m_DataPath = dataPath;
            m_IsInit = false;
        }

        public void Init()
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

            TextAsset txt = AssetsMgr.instance.LoadAssetSync<TextAsset>(m_DataPath);

            if(txt == null)
            {
                Log.LogError("语言文件不存在");
                return;
            }

            m_IsInit = true;
            OnInit(txt);
        }

        public void Release()
        {
            if (!m_IsInit)
            {
                return;
            }

            AssetsMgr.instance.UnloadAsset(m_DataPath);
            m_IsInit = false;
            OnRelease();
        }

        protected abstract void OnInit(TextAsset textAsset);

        public abstract string GetLanguageText(string key);

        protected abstract void OnRelease();

        private bool m_IsInit = false;
        private string m_DataPath = string.Empty;
    }
}