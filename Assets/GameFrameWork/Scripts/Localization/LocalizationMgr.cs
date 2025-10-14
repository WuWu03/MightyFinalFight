using System.Collections.Generic;
using GameFrameWork.Event;
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
        public event GameFrameWorkAction lanuageChangeEvent
        {
            add
            {
                m_LanguageChangeEvent += value;
            }
            remove
            {
                m_LanguageChangeEvent -= value;
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            m_LanguageType = (LanguageType)PlayerPrefs.GetInt(m_CacheKey, (int)LanguageType.None);
            m_DicLanguageLoader = new();
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();

            m_DicLanguageLoader.Clear();
        }

        protected override void OnDestory()
        {
            base.OnDestory();

            m_LanguageChangeEvent = null;
            m_DicLanguageLoader = null;
        }

        public void AddLanguageLoader(LanguageType languageType, BaseLanguageLoader loader)
        {
            if (m_DicLanguageLoader.ContainsKey(languageType))
            {
                Log.LogError("语言读取器已经存在：[", languageType.ToString(), "]");
            }

            m_DicLanguageLoader.Add(languageType, loader);

            if (m_LanguageType == languageType)
            {
                loader.Init();
            }
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
                    Log.LogError("语言读取器不存在：[", m_LanguageType.ToString(), "]");
                    return;
                }
            }

            if (m_DicLanguageLoader.TryGetValue(languageType, out loader))
            {
                PlayerPrefs.SetInt(m_CacheKey, (int)languageType);
                m_LanguageType = languageType;
                loader.Init();
                m_LanguageChangeEvent?.Invoke();
            }
            else
            {
                Log.LogError("语言读取器不存在：[", languageType.ToString(), "]");
            }
        }

        public void ReloadLanguage()
        {
            if (m_LanguageType == LanguageType.None)
            {
                Log.LogError("未设置语言类型，请先设置默认语言");
                return;
            }

            if (m_DicLanguageLoader.TryGetValue(m_LanguageType, out BaseLanguageLoader loader))
            {
                loader.Reload();
                m_LanguageChangeEvent?.Invoke();
            }
            else
            {
                Log.LogError("语言读取器不存在：[", m_LanguageType.ToString(), "]");
            }
        }

        public string GetLanguageText(string key)
        {
            if (m_DicLanguageLoader.TryGetValue(m_LanguageType, out BaseLanguageLoader loader))
            {
                return loader.GetLanguageText(key);
            }

            Log.LogError("语言读取器不存在：[", m_LanguageType.ToString(), "]");

            return string.Empty;
        }

        private const string m_CacheKey = "_GAME_LANGUAGE_NAME_";
        private LanguageType m_LanguageType = LanguageType.None;
        private Dictionary<LanguageType, BaseLanguageLoader> m_DicLanguageLoader = null;
        private event GameFrameWorkAction m_LanguageChangeEvent = null;
    }
}