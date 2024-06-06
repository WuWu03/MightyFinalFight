using GameFrameWork;
using GameFrameWork.Localization;
using LitJson;
using UnityEngine;

public class JapaneseLanguageLoader : BaseLanguageLoader
{
    public JapaneseLanguageLoader(string dataPath) : base(dataPath) { }
    protected override void OnInit(TextAsset textAsset)
    {
        if (string.IsNullOrEmpty(textAsset.text))
        {
            Log.LogError("日语语言文件错误");
            return;
        }

        m_JsonData = JsonMapper.ToObject(textAsset.text);
    }

    public override string GetLanguageText(string key)
    {
        if (m_JsonData.ContainsKey(key))
        {
            return m_JsonData[key].ToString();
        }

        return string.Empty;
    }

    public override string GetLanguageText(int id)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnRelease()
    {
        m_JsonData.Clear();
        m_JsonData = null;
    }

    private JsonData m_JsonData = null;
}
