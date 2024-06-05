using GameFrameWork.Localization;
using GameFrameWork.Utilities;

public class EnglishLanguageLoader : ILanguageLoader
{
    public string GetLanguageText(string key)
    {
        LocalizationConfigData[] datas = ConfigDataHelper.localizationConfigDatas.GetConfigDatasByAttr(StringUtil.Format("key=", key));
        if (datas == null || datas.Length < 1)
        {
            return string.Empty;
        }

        return datas[1].english;
    }

    public string GetLanguageText(int id)
    {
        LocalizationConfigData data = ConfigDataHelper.localizationConfigDatas.GetConfigDataById(id);

        if (data == null)
        {
            return string.Empty;
        }
        return data.english;
    }

}
