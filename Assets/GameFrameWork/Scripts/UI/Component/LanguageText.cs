using GameFrameWork.Event;
using GameFrameWork.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    [AddComponentMenu("UI/LanguageText")]
    public class LanguageText : MonoBehaviour
    {
        public enum LanguageMode
        {
            UseId,
            UseKey,
        }

        public int languageTextId;
        public string languageTextKey;
        public LanguageMode languageMode = LanguageMode.UseKey;

        private void Awake()
        {
            if (!TryGetComponent(out m_TextMesh) && !TryGetComponent(out m_Text))
            {
                Log.LogError("文本组件为空，请检查");
                return;
            }

            UpdateLanguage();
        }

        private void OnEnable()
        {
            EventMgr.instance.Subscribe(GameFrameWorkCommonEvent.LanguageChangeEvent, OnLanguageChange);
            UpdateLanguage();
        }

        private void OnDisable()
        {
            if (GameFrameWorkEntry.IsApplicationRunning())
            {
                EventMgr.instance.UnSubscribe(GameFrameWorkCommonEvent.LanguageChangeEvent, OnLanguageChange);
            }
        }

        public void UpdateLanguageTextId(int id)
        {
            if (languageTextId == id)
            {
                return;
            }

            languageTextId = id;
            languageMode = LanguageMode.UseId;
            UpdateLanguage();
        }

        public void UpdateLanguageTextKey(string key)
        {
            if (languageTextKey == key)
            {
                return;
            }

            languageTextKey = key;
            languageMode = LanguageMode.UseKey;
            UpdateLanguage();
        }

        private void OnLanguageChange(object sender, GameEventArgs e)
        {
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            string text;

            if (languageMode == LanguageMode.UseKey)
            {
                if (string.IsNullOrEmpty(languageTextKey))
                {
                    return;
                }

                text = LocalizationMgr.instance.GetLanguageText(languageTextKey);
            }
            else
            {
                if (languageTextId < 1)
                {
                    return;
                }

                text = LocalizationMgr.instance.GetLanguageText(languageTextId);
            }

            if (m_TextMesh != null)
            {
                m_TextMesh.text = text;
            }
            else if (m_Text != null)
            {
                m_Text.text = text;
            }
        }

        private Text m_Text;
        private TextMeshProUGUI m_TextMesh;
    }
}