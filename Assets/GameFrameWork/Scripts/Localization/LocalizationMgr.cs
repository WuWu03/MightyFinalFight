using System;
using System.Collections.Generic;
using GameFrameWork.Assets;
using GameFrameWork.Event;
using GameFrameWork.Utils;
using UnityEngine;

namespace GameFrameWork.Localization
{
    public class LocalizationMgr : GameFrameWorkModule,ILocalizationMgr
    {
        private readonly Dictionary<LanguageType, BaseLanguageLoader> m_DicLanguageLoader;
        private const string CacheKey = "_GAME_LANGUAGE_NAME_";
        private LanguageType m_LanguageType;
        private IResourceMgr m_ResourceMgr;
        private event GameFrameWorkAction m_LanguageChangeEvent;
        
        public LocalizationMgr()
        {
            m_LanguageType = (LanguageType)PlayerPrefs.GetInt(CacheKey, (int)LanguageType.None);
            m_DicLanguageLoader = new();
        }
        
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
        
        public override void Shutdown()
        {
            m_DicLanguageLoader.Clear();
            m_LanguageChangeEvent = null;
        }

        public void SetResourceManager(IResourceMgr resourceMgr)
        {
            m_ResourceMgr = resourceMgr;
        }

        public void AddLanguageLoader(LanguageType languageType, BaseLanguageLoader loader)
        {
            if (!m_DicLanguageLoader.TryAdd(languageType, loader))
            {
                throw new Exception(StringUtil.Append("语言读取器已经存在：[", languageType.ToString(), "]"));
            }

            if (m_LanguageType == languageType)
            {
                loader.Init(m_ResourceMgr);
            }
        }

        public void SetDefaultLanguage(LanguageType languageType)
        {
            if (m_LanguageType != LanguageType.None)
            {
                return;
            }

            m_LanguageType = languageType;
            PlayerPrefs.SetInt(CacheKey, (int)languageType);
        }

        public void ChangeLanguage(LanguageType languageType)
        {
            if (m_LanguageType == languageType || languageType == LanguageType.None)
            {
                return;
            }

            if (m_DicLanguageLoader.Count < 1)
            {
                throw new Exception("未初始化语言读取器，请先添加对应语言的读取器");
            }

            BaseLanguageLoader loader;

            if (m_LanguageType != LanguageType.None)
            {
                if (!m_DicLanguageLoader.TryGetValue(m_LanguageType, out loader))
                {
                    throw new Exception(StringUtil.Append("语言读取器不存在：[", m_LanguageType.ToString(), "]"));
                }

                loader.Release(m_ResourceMgr);
            }

            if (!m_DicLanguageLoader.TryGetValue(languageType, out loader))
            {
                throw new Exception(StringUtil.Append("语言读取器不存在：[", languageType.ToString(), "]"));
            }
            
            PlayerPrefs.SetInt(CacheKey, (int)languageType);
            m_LanguageType = languageType;
            loader.Init(m_ResourceMgr);
            m_LanguageChangeEvent?.Invoke();
        }

        public void ReloadLanguage()
        {
            if (m_LanguageType == LanguageType.None)
            {
                throw new Exception("未设置语言类型，请先设置默认语言");
            }

            if (!m_DicLanguageLoader.TryGetValue(m_LanguageType, out BaseLanguageLoader loader))
            {
                throw new Exception(StringUtil.Append("语言读取器不存在：[", m_LanguageType.ToString(), "]"));
            }

            loader.Reload(m_ResourceMgr);
            m_LanguageChangeEvent?.Invoke();
        }

        public string GetLanguageText(string key)
        {
            if (!m_DicLanguageLoader.TryGetValue(m_LanguageType, out BaseLanguageLoader loader))
            {
                throw new Exception(StringUtil.Append("语言读取器不存在：[", m_LanguageType.ToString(), "]"));
            }

            return loader.GetLanguageText(key);
        }
    }
}