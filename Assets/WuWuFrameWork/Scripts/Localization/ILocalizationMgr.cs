using System.Collections.Generic;
using System.Resources;
using WuWuFramework.Resources;
using WuWuFramework.Event;
using UnityEngine;

namespace WuWuFramework.Localization
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

    public interface ILocalizationMgr
    {
        public event WuWuFrameworkAction lanuageChangeEvent;
        public void SetResourceManager(IResourcesMgr resourceMgr);
        public void AddLanguageLoader(LanguageType languageType, BaseLanguageLoader loader);
        public void SetDefaultLanguage(LanguageType languageType);
        public void ChangeLanguage(LanguageType languageType);
        public void ReloadLanguage();
        public string GetLanguageText(string key);
    }
}