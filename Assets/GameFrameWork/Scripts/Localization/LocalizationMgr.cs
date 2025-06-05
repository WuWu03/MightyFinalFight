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
            m_LanguageType = (LanguageType)PlayerPrefs.GetInt(m_CacheKey, (int)LanguageType.None);
            m_DicLanguageLoader = new Dictionary<LanguageType, BaseLanguageLoader>();
        }

        public void SetDefaultLanguage(LanguageType languageType)
        {
            if (m_LanguageType != LanguageType.None)
            {
                return;
            }

            m_LanguageType = languageType;
            PlayerPrefs.SetInt(m_CacheKey, (int)languageType);
        }

        public void ChangeLanguage(LanguageType languageType)
        {
            if (m_LanguageType == languageType || languageType == LanguageType.None)
            {
                return;
            }

            if (m_DicLanguageLoader.Count < 1)
            {
                Log.LogError("未初始化语言读取器，请先添加对应语言的读取器");
                return;
            }

            BaseLanguageLoader loader;

            if (m_LanguageType != LanguageType.None)
            {
                if (m_DicLanguageLoader.TryGetValue(m_LanguageType, out loader))
                {
                    loader.Release();
                }
                else
                {
                    Log.LogError("语言读取器不存在：[", m_LanguageType, "]");
                    return;
                }
            }

            if (m_DicLanguageLoader.TryGetValue(languageType, out loader))
            {
                PlayerPrefs.SetInt(m_CacheKey, (int)languageType);
                m_LanguageType = languageType;
                loader.Init();
                EventMgr.instance.Dispatch(this, GameEventArgs.Create(GameFrameWorkCommonEvent.LanguageChangeEvent));
            }
            else
            {
                Log.LogError("语言读取器不存在：[", languageType, "]");
            }
        }

        public void AddLanguageLoader(LanguageType languageType, BaseLanguageLoader loader)
        {
            if (m_DicLanguageLoader.ContainsKey(languageType))
            {
                Log.LogError("语言读取器已经存在：[", languageType, "]");
            }

            m_DicLanguageLoader.Add(languageType, loader);

            if (m_LanguageType == languageType)
            {
                loader.Init();
            }
        }

        public string GetLanguageText(string key)
        {
            if (m_DicLanguageLoader.TryGetValue(m_LanguageType, out BaseLanguageLoader loader))
            {
                return loader.GetLanguageText(key);
            }

            Log.LogError("语言读取器不存在：[", m_LanguageType, "]");

            return string.Empty;
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();
            m_DicLanguageLoader.Clear();
            m_DicLanguageLoader = null;
        }

        private const string m_CacheKey = "_GAME_LANGUAGE_NAME_";

        private LanguageType m_LanguageType = LanguageType.None;
        private Dictionary<LanguageType, BaseLanguageLoader> m_DicLanguageLoader = null;
    }
}