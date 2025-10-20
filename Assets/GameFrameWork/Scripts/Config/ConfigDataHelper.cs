using System;
using GameFrameWork.Assets;
using GameFrameWork.Utils;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameFrameWork.ConfigData
{
    public static class ConfigDataHelper
    {
        private static IResourceMgr m_ResourceMgr;
        public static void SetResourcesMgr(IResourceMgr resourceMgr)
        {
            m_ResourceMgr = resourceMgr;
        }
        
        public static T[] LoadConfigData<T>(string fileName) where T : BaseConfigData, new()
        {
            string path = PathUtil.FormatPath(GameFrameWorkEntry.config.configDataPath, fileName);
            TextAsset txt = m_ResourceMgr.Load<TextAsset>(path);

            if (txt?.bytes == null)
            {
                throw new Exception(StringUtil.Append("读取配置文件失败 : ", path));
            }

            using ConfigDataParser parser = new(txt.bytes);
            T[] data = new T[parser.row - 1];
            int index = 0;
            
            while (!parser.eof)
            {
                data[index] = new T();
                data[index].Read(parser);
                parser.Next();
                index++;
            }

            return data;
        }

        public static T GetConfigDataById<T>(this T[] data, int id) where T : BaseConfigData, new()
        {
            int left = 0;
            int right = data.Length;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                if (data[mid].id == id)
                {
                    return data[mid];
                }

                if (id > data[mid].id)
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

        public static T GetSingConfigDataByAttr<T>(this T[] data, string attr) where T : BaseConfigData, new()
        {
            T[] result = GetConfigDataByAttr(data, attr, true);

            if (result != null && result.Length > 0)
            {
                return result[0];
            }

            return null;
        }
        
        private static T[] GetConfigDataByAttr<T>(T[] data, string attr, bool isSingle = false) where T : BaseConfigData, new()
        {
            if (string.IsNullOrEmpty(attr) || !attr.StartsWith("{") || !attr.EndsWith("}"))
            {
                throw new GameFrameWorkException("查询格式串错误");
            }
            
            attr = attr.Replace(" ", string.Empty);

            Match match = Regex.Match(attr, @"((\w)+=(\w)+)");
            if (match.Success)
            {
                List<T> values = new();

                foreach (var datum in data)
                {
                    bool isMatch = true;
                    Match tempMatch = match;

                    while (tempMatch.Success)
                    {
                        string[] condition = tempMatch.Value.Split("=");
                        PropertyInfo property = datum.GetType().GetProperty(condition[0]);

                        if (property == null || !property.GetValue(datum).ToString().Equals(condition[1]))
                        {
                            isMatch = false;
                            break;
                        }

                        tempMatch = tempMatch.NextMatch();
                    }

                    if (isMatch)
                    {
                        values.Add(datum);

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