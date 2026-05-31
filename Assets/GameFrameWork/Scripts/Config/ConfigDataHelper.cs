using System;
using GameFrameWork.Resources;
using GameFrameWork.Utils;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameFrameWork.ConfigData
{
    public static class ConfigDataHelper
    {
        private static IResourcesMgr m_ResourceMgr;
        public static void SetResourcesMgr(IResourcesMgr resourceMgr)
        {
            m_ResourceMgr = resourceMgr;
        }
        
        public static T[] LoadConfigData<T>(string filePath) where T : BaseConfigData, new()
        {
            TextAsset txt = m_ResourceMgr.Load<TextAsset>(filePath);
            byte[] bytes = txt.bytes;
            m_ResourceMgr.Unload(filePath);

            if (bytes == null)
            {
                throw new Exception(StringUtil.Append("读取配置文件失败 : ", filePath));
            }

            using ConfigDataParser parser = new(bytes);
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

            while (left < right)
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

            return data[left];
        }

        public static T GetSingConfigDataByAttr<T>(this T[] data, string attr, int index = 0) where T : BaseConfigData, new()
        {
            T[] result = GetConfigDataByAttr(data, attr, true);

            if (result is { Length: > 0 })
            {
                return result[index];
            }

            return null;
        }

        public static T[] GetConfigDataByAttr<T>(this T[] data, string attr) where T : BaseConfigData, new()
        {
            return GetConfigDataByAttr<T>(data, attr, false);
        }
        
        private static T[] GetConfigDataByAttr<T>(T[] data, string attr, bool isSingle) where T : BaseConfigData, new()
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