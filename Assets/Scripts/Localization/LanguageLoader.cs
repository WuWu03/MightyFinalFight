using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.Localization;
using System.Collections;
using UnityEngine;

public class LanguageLoader : BaseLanguageLoader
{
    public LanguageLoader(string dataPath) : base(dataPath) 
    {
        m_LanguageDataTable = new Hashtable();
    }

    protected override void OnInit(TextAsset textAsset)
    {
        if (textAsset.bytes == null || textAsset.bytes.Length < 1)
        {
            Log.LogError("语言文件错误");
            return;
        }

        using (ConfigDataParser parser = new ConfigDataParser(textAsset.bytes))
        {
            while (!parser.eof)
            {
                m_LanguageDataTable.Add(parser.GetFieldValue("key"), parser.GetFieldValue("content"));
                parser.Next();
            }
        }
    }

    public override string GetLanguageText(string key)
    {
        if (m_LanguageDataTable.ContainsKey(key))
        {
            return m_LanguageDataTable[key].ToString();
        }

        return string.Empty;
    }

    protected override void OnRelease()
    {
        m_LanguageDataTable.Clear();
        m_LanguageDataTable = null;
    }

    private Hashtable m_LanguageDataTable = null;
}
