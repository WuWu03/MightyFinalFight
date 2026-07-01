using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using WuWuFramework.Event;
using WuWuFramework.Resources;
using WuWuFramework.Utils;

namespace WuWuFramework.ConfigData
{
    public static class ConfigDataHelper
    {
        private static IResourcesMgr s_ResourceMgr;
        private static readonly Dictionary<Type, object> s_PredicateCache = new();

        public static void SetResourcesMgr(IResourcesMgr resourceMgr)
        {
            s_ResourceMgr = resourceMgr;
        }

        public static T[] LoadConfigData<T>(string filePath) where T : BaseConfigData, new()
        {
            TextAsset txt = s_ResourceMgr.Load<TextAsset>(filePath);
            byte[] bytes = txt.bytes;
            s_ResourceMgr.Unload(filePath);

            if (bytes == null)
            {
                throw new Exception(StringUtil.Append("读取配置文件失败 : ", filePath));
            }

            ConfigDataParser parser = ReferencePool.Acquire<ConfigDataParser>();
            parser.Init(bytes);
            T[] data = new T[parser.row - 1];
            int index = 0;

            while (!parser.eof)
            {
                data[index] = new T();
                data[index].Read(parser);
                parser.Next();
                index++;
            }

            parser.Release();
            return data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>(this T[] data, int id) where T : BaseConfigData, new()
        {
            int left = 0;
            int right = data.Length - 1;

            while (left <= right)
            {
                int mid = left + ((right - left) >> 1);
                int currentId = data[mid].id;

                if (currentId == id)
                {
                    return data[mid];
                }
                else if(currentId < id)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            // 如果没找到，返回 null
            return null;
        }

        public static T Get<T>(this T[] data, WuWuFrameworkFunc<T, bool> predicate) where T : BaseConfigData, new()
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (var datum in data)
            {
                if (predicate(datum))
                {
                    return datum;
                }
            }

            return null;
        }

        public static void ForEach<T>(this T[] data, WuWuFrameworkFunc<T, bool> predicate, WuWuFrameworkAction<T> onMatch, bool stopOnFirst = false) where T : BaseConfigData, new()
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (onMatch == null)
            {
                throw new ArgumentNullException(nameof(onMatch));
            }

            for (int i = 0; i < data.Length; i++)
            {
                T datum = data[i];

                if (predicate(datum))
                {
                    onMatch(datum);

                    if (stopOnFirst)
                    {
                        return;
                    }
                }
            }
        }

        public static IList<T> ForEach<T>(this T[] data, WuWuFrameworkFunc<T, bool> predicate) where T : BaseConfigData, new()
        {
            IList<T> list = GetPredicateCache<T>(data.Length);

            foreach (var datum in data)
            {
                if (predicate(datum))
                {
                    list.Add(datum);
                }
            }

            return list;
        }

        public static void ClearPredicateCache()
        {
            s_PredicateCache.Clear();
        }

        private static IList<T> GetPredicateCache<T>(int dataLength) where T : BaseConfigData, new()
        {
            List<T> result;
            Type type = typeof(T);

            if (!s_PredicateCache.TryGetValue(type, out object list))
            {
                list = new List<T>(Mathf.Min(16, dataLength));
                s_PredicateCache.Add(type, list);
            }

            result = (List<T>)list;

            if (result == null)
            {
                return null;
            }

            result.Clear();
            return result;
        }
    }
}