using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.Resources;
using GameFrameWork.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class ConfigDataHelper
{
    public static T[] LoadConfigData<T>(string filePath, string fileName) where T : BaseConfigData, new()
    {
        string path = PathUtil.FormatPath(filePath, fileName);

        T[] t = null;

        TextAsset txt = ResourcesMgr.instance.LoadAsset<TextAsset>(path);

        if (txt == null || txt.bytes == null)
        {
            Log.LogError("∂¡»°≈‰÷√Œƒº˛ ß∞‹ : ", path);
            return null;
        }

        using (ConfigDataParser parser = new ConfigDataParser(txt.bytes))
        {
            t = new T[parser.row - 1];
            int index = 0;
            while (!parser.eof)
            {
                t[index] = new T();
                t[index].Read(parser);
                parser.Next();
                index++; ;
            }
        }
        return t;
    }

    public static T GetConfigDataById<T>(this T[] datas, int id) where T : BaseConfigData, new()
    {
        try
        {
            return datas.First(data => data.id == id);
        }
        catch
        {
            return null;
        }
    }

    public static T GetSingConfigDataByAttr<T>(this T[] datas, string attr) where T : BaseConfigData, new()
    {
        T[] result = GetConfigDataByAttr(datas, attr, true);

        if(result != null && result.Length > 0)
        {
            return result[0];
        }

        return null;
    }

    public static T[] GetConfigDatasByAttr<T>(this T[] datas, string attr) where T : BaseConfigData, new()
    {
        return GetConfigDataByAttr(datas, attr);
    }

    private static T[] GetConfigDataByAttr<T>(T[] datas, string attr, bool isSingle = false) where T : BaseConfigData, new()
    {
        attr = attr.Replace("{", string.Empty).Replace("}", string.Empty).Replace(" ", string.Empty);

        Match match = Regex.Match(attr, "[^,]+");

        if (match.Success)
        {
            List<T> values = new List<T>();

            for (int i = 0; i < datas.Length; i++)
            {
                bool isMatch = true;
                Match tempMatch = match;

                while (tempMatch.Success)
                {
                    string[] condition = tempMatch.Value.Split("=");
                    PropertyInfo property = datas[i].GetType().GetProperty(condition[0]);

                    if (property == null || !property.GetValue(datas[i]).ToString().Equals(condition[1]))
                    {
                        isMatch = false;
                        break;
                    }

                    tempMatch = tempMatch.NextMatch();
                }

                if (isMatch)
                {
                    values.Add(datas[i]);

                    if (isSingle)
                    {
                        break;
                    }
                }
            }

            return values.ToArray();
        }

        return null;
    }
}