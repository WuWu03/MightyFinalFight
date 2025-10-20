using System;
using GameFrameWork.Localization;
using GameFrameWork.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    [AddComponentMenu("UI/LanguageText")]
    public class LanguageText : MonoBehaviour
    {
        public string languageTextKey;
        private string m_AppendArg = string.Empty;
        private Text m_Text;
        private TextMeshProUGUI m_TextMeshProUGUI;
        private static ILocalizationMgr m_LocalizationMgr;
        private void Awake()
        {
            if (GameFrameWorkEntry.isStartUp)
            {
                InitComponent();
                UpdateLanguage();
            }
        }

        private void OnEnable()
        {
            if (GameFrameWorkEntry.isStartUp)
            {
                m_LocalizationMgr.lanuageChangeEvent += OnLanguageChange;
                UpdateLanguage();
            }
        }

        private void OnDisable()
        {
            if (GameFrameWorkEntry.isStartUp)
            {
                m_LocalizationMgr.lanuageChangeEvent -= OnLanguageChange;
            }
        }

        public static void SetLocalizationMgr(ILocalizationMgr localizationMgr)
        {
            m_LocalizationMgr = localizationMgr;
        }
        
        public void SetText(string text)
        {
            InitComponent();
            if (!string.IsNullOrEmpty(m_AppendArg))
            {
                text = StringUtil.Append(text, m_AppendArg);
            }

            if (m_TextMeshProUGUI != null)
            {
                m_TextMeshProUGUI.text = text;
            }
            else if (m_Text != null)
            {
                m_Text.text = text;
            }
        }

        public void SetLanguageTextKey(string key)
        {
            if (languageTextKey == key)
            {
                return;
            }

            languageTextKey = key;
            UpdateLanguage();
        }

        public void SetLanguageTextKey(string key, string arg1)
        {
            SetLanguageTextKey(key, arg1, null, null, null, null, null, null);
        }

        public void SetLanguageTextKey(string key, string arg1, string arg2)
        {
            SetLanguageTextKey(key, arg1, arg2, null, null, null, null, null);
        }

        public void SetLanguageTextKey(string key, string arg1, string arg2, string arg3)
        {
            SetLanguageTextKey(key, arg1, arg2, arg3, null, null, null, null);
        }

        public void SetLanguageTextKey(string key, string arg1, string arg2, string arg3, string arg4)
        {
            SetLanguageTextKey(key, arg1, arg2, arg3, arg4, null, null, null);
        }

        public void SetLanguageTextKey(string key, string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            SetLanguageTextKey(key, arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public void SetLanguageTextKey(string key, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            SetLanguageTextKey(key, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public void SetLanguageTextKey(string key, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            if (languageTextKey == key)
            {
                return;
            }

            languageTextKey = key;
            UpdateLanguage(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public void SetLanguageTextParams(string arg1)
        {
            SetLanguageTextParams(arg1, null, null, null, null, null, null);
        }

        public void SetLanguageTextParams(string arg1, string arg2)
        {
            SetLanguageTextParams(arg1, arg2, null, null, null, null, null);
        }

        public void SetLanguageTextParams(string arg1, string arg2, string arg3)
        {
            SetLanguageTextParams(arg1, arg2, arg3, null, null, null, null);
        }

        public void SetLanguageTextParams(string arg1, string arg2, string arg3, string arg4)
        {
            SetLanguageTextParams(arg1, arg2, arg3, arg4, null, null, null);
        }

        public void SetLanguageTextParams(string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            SetLanguageTextParams(arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public void SetLanguageTextParams(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            SetLanguageTextParams(arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public void SetLanguageTextParams(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            if (string.IsNullOrEmpty(languageTextKey))
            {
                Log.LogError("语言文本键为空，请检查");
                return;
            }

            UpdateLanguage(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public void Append(string arg1)
        {
            if (string.IsNullOrEmpty(arg1))
            {
                return;
            }

            m_AppendArg = arg1;
            UpdateLanguage();
        }

        private void OnLanguageChange()
        {
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            if (string.IsNullOrEmpty(languageTextKey))
            {
                return;
            }

            string text = m_LocalizationMgr.GetLanguageText(languageTextKey);
            SetText(text);
        }

        private void UpdateLanguage(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            if (string.IsNullOrEmpty(languageTextKey))
            {
                return;
            }

            string text = m_LocalizationMgr.GetLanguageText(languageTextKey);
            text = StringUtil.Format(text, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

            SetText(text);
        }

        private void InitComponent()
        {
            if (m_TextMeshProUGUI != null || m_Text != null)
            {
                return;
            }

            if (!TryGetComponent(out m_TextMeshProUGUI) && !TryGetComponent(out m_Text))
            {
                Log.LogError("文本组件为空，请检查");
            }
        }
    }
}