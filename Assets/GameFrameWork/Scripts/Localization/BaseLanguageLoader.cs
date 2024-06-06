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

            if (m_TextAsset != null)
            {
                return;
            }

            m_TextAsset = AssetsMgr.instance.LoadAssetSync<TextAsset>(m_DataPath);

            if(m_TextAsset == null)
            {
                Log.LogError("语言文件不存在");
                return;
            }

            m_IsInit = true;
            OnInit(m_TextAsset);
        }

        public void Release()
        {
            if (!m_IsInit)
            {
                return;
            }

            AssetsMgr.instance.UnloadAsset(m_DataPath);
            m_TextAsset = null;
            m_IsInit = false;
            OnRelease();
        }

        protected abstract void OnInit(TextAsset textAsset);

        public abstract string GetLanguageText(string key);

        public abstract string GetLanguageText(int id);

        protected abstract void OnRelease();

        private bool m_IsInit = false;
        private TextAsset m_TextAsset = null;
        private string m_DataPath = string.Empty;
    }
}