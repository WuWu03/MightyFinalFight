using GameFrameWork;
using GameFrameWork.Localization;
using System.Collections.Generic;
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

        if (m_DicLanguageText == null)
        {
            m_DicLanguageText = new Dictionary<string, string>();
        }

        string[] contents = textAsset.text.Split("#");

        foreach (string line in contents)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            string result = line.TrimStart('\r', '\n');
            string[] datas = result.Split(",", 2);
            m_DicLanguageText.Add(datas[0], datas[1]);
        }
    }

    public override string GetLanguageText(string key)
    {
        if (m_DicLanguageText.TryGetValue(key, out string text))
        {
            return text;
        }

        return string.Empty;
    }

    public override string GetLanguageText(int id)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnRelease()
    {
        m_DicLanguageText.Clear();
        m_DicLanguageText = null;
    }

    private Dictionary<string, string> m_DicLanguageText = null;
}
