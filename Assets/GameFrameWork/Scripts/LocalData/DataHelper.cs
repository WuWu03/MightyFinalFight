using GameFrameWork.LocalData;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public static  partial class DataHelper
{
    public static T[] LoadData<T>(string filePath, string fileName) where T : BaseLocalData, new()
    {
        string path = string.Format(filePath + "{0}", fileName);

        T[] t = null;
        using (LocalDataParser parser = new LocalDataParser(path))
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

    public static T GetDataById<T>(this T[] datas,int id) where T : BaseLocalData, new()
    {
        for (int i = 0; i < datas.Length; i++)
        {
            if(datas[i].id == id)
            {
                return datas[i];
            }
        }

        return datas.Single(t => t.id == id);
    }

    public static T[] GetDatasByAttr<T>(this T[] datas,string attr) where T : BaseLocalData, new()
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

                    if (property == null || property.GetValue(datas[i]).ToString() != condition[1])
                    {
                        isMatch = false;
                        break;
                    }

                    tempMatch = tempMatch.NextMatch();
                }

                if (isMatch)
                {
                    values.Add(datas[i]);
                }
            }

            return values.ToArray();
        }

        return null;
    }

}
