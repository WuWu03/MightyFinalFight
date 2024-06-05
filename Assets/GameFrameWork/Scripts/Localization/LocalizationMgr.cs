using GameFrameWork.Event;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Localization
{
    public enum LanguageType
    {
        None = 0,
        SimplifiedChinese = 1,
        TraditionalChinese = 2,
        English = 3,
        Japanese = 4,
        Korean = 5,
    }

    public class LocalizationMgr : BaseMgr<LocalizationMgr>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            m_Language = (LanguageType)PlayerPrefs.GetInt(m_CacheKey, (int)LanguageType.None);
            m_DicLanguageLoader = new Dictionary<LanguageType, ILanguageLoader>();
        }

        public void SetDefaultLanguage(LanguageType languageType)
        {
            if (m_Language != LanguageType.None)
            {
                return;
            }

            ChangeLanguage(languageType);
        }

        public void ChangeLanguage(LanguageType languageType)
        {
            if (m_Language == languageType)
            {
                return;
            }

            PlayerPrefs.SetInt(m_CacheKey, (int)languageType);
            m_Language = languageType;
            EventMgr.instance.Dispatch(this, GameEventArgs.Create(GameFrameWorkCommonEvent.LanguageChangeEvent));
        }

        public void AddLanguageLoader(LanguageType languageType,ILanguageLoader loader)
        {
            if (m_DicLanguageLoader.ContainsKey(languageType))
            {
                Log.LogError("语言读取器已经存在：[", languageType, "]");
            }

            m_DicLanguageLoader.Add(languageType, loader);
        }

        public string GetLanguageText(string key)
        {
            if (m_DicLanguageLoader.TryGetValue(m_Language, out ILanguageLoader loader))
            {
                return loader.GetLanguageText(key);
            }

            Log.LogError("语言读取器不存在：[", m_Language, "]");

            return string.Empty;
        }

        public string GetLanguageText(int id)
        {
            if (m_DicLanguageLoader.TryGetValue(m_Language, out ILanguageLoader loader))
            {
                return loader.GetLanguageText(id);
            }

            Log.LogError("语言读取器不存在：[", m_Language, "]");

            return string.Empty;
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();
            m_DicLanguageLoader.Clear();
            m_DicLanguageLoader = null;
        }

        private const string m_CacheKey = "_GAME_LANGUAGE_NAME_";
      
        private LanguageType m_Language = LanguageType.None;
        private Dictionary<LanguageType,ILanguageLoader> m_DicLanguageLoader = null;
    }
}