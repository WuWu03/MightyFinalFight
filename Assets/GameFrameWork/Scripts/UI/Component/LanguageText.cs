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
        public string languageTextKey;

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


        public void UpdateLanguageTextKey(string key)
        {
            if (languageTextKey == key)
            {
                return;
            }

            languageTextKey = key;
            UpdateLanguage();
        }

        private void OnLanguageChange(object sender, GameEventArgs e)
        {
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            if (string.IsNullOrEmpty(languageTextKey))
            {
                return;
            }

            string text = LocalizationMgr.instance.GetLanguageText(languageTextKey);

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