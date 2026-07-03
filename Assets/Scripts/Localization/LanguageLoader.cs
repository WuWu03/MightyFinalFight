using System.Collections.Generic;
using UnityEngine;
using WuWuFramework;
using WuWuFramework.ConfigData;
using WuWuFramework.Localization;

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

        ConfigDataParser parser = ReferencePool.Acquire<ConfigDataParser>();
        parser.Init(textAsset.bytes);

        while (!parser.eof)
        {
            int id = parser.Read<int>();
            string key = parser.Read<string>();
            string content = parser.Read<string>().Replace("\\n", "\n");
            m_LanguageMap.Add(key, content);
            parser.Next();
        }

        parser.Release();
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