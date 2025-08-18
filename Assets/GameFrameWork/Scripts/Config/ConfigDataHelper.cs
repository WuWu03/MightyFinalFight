using GameFrameWork.Assets;
using GameFrameWork.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameFrameWork.ConfigData
{
    public static class ConfigDataHelper
    {
        public static T[] LoadConfigData<T>(string fileName) where T : BaseConfigData, new()
        {
            string path = PathUtil.FormatPath(GameFrameWorkEntry.config.configDataPath, fileName);

            T[] t = null;

            TextAsset txt = AssetsMgr.instance.LoadAssetSync<TextAsset>(path);

            if (txt == null || txt.bytes == null)
            {
                Log.LogError("读取配置文件失败 : ", path);
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
                    index++;
                }
            }

            return t;
        }

        public static T GetConfigDataById<T>(this T[] datas, int id) where T : BaseConfigData, new()
        {
            int left = 0;
            int right = datas.Length;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                if (datas[mid].id == id)
                {
                    return datas[mid];
                }

                if (id > datas[mid].id)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return null;
        }

        public static T GetSingConfigDataByAttr<T>(this T[] datas, string attr) where T : BaseConfigData, new()
        {
            T[] result = GetConfigDataByAttr(datas, attr, true);

            if (result != null && result.Length > 0)
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
                List<T> values = new();

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
}