using WuWuFramework;
using WuWuFramework.ConfigData;
using WuWuFramework.Localization;
using System.Collections.Generic;
using UnityEngine;

public class LanguageLoader : BaseLanguageLoader
{
    private readonly Dictionary<string, string> m_LanguageMap;

    public LanguageLoader(string dataPath) : base(dataPath)
    {
        m_LanguageMap = new Dictionary<string, string>();
    }

    protected override void OnInit(TextAsset textAsset)
    {
        if (textAsset.bytes == null || textAsset.bytes.Length < 1)
        {
            Log.LogError("语言文件错误");
            return;
        }

        using ConfigDataParser parser = new(textAsset.bytes);

        while (!parser.eof)
        {
            string content = parser.GetFieldValue("content").Replace("\\n", "\n");
            m_LanguageMap.Add(parser.GetFieldValue("key"), content);
            parser.Next();
        }
    }

    public override string GetLanguageText(string key)
    {
        if (m_LanguageMap.TryGetValue(key, out string result))
        {
            return result;
        }

        return string.Empty;
    }

    protected override void OnRelease()
    {
        m_LanguageMap.Clear();
    }
}