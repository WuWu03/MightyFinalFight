/*
 * @Desc: UI工厂
 * @Date: 2026-05-22 22:20:53
 * @Author: WuWu
 */

using WuWuFramework.Event;
using WuWuFramework.Utils;
using System;
using System.Collections.Generic;

namespace WuWuFramework.UI
{
    public static partial class UIFactory
    {
        private static readonly Dictionary<Type, WuWuFrameworkFunc<IUIView>> s_Factories = new();
        private static readonly Dictionary<string, Type> s_ViewTypes = new();

        public static IUIView GetUIView(Type uiType)
        {
            if (s_Factories.TryGetValue(uiType, out WuWuFrameworkFunc<IUIView> factory))
            {
                return factory();
            }

            throw new WuWuFrameworkException("未找到对应的UI类型 : [" + uiType.Name + "]，请生成对应的UI类或UIFactory文件");
        }

        public static Type GetViewType(string viewName)
        {
            if (s_ViewTypes.TryGetValue(viewName, out Type viewType))
            {
                return viewType;
            }

            throw new Exception(StringUtil.Append("[", viewName, "] 不存在,请使用RegisterViewType方法进行类型注册"));
        }

        public static void RegisterViewType<T>(string viewName) where T : class, IUIView, new()
        {
            s_ViewTypes.Add(viewName, typeof(T));
        }

        // 运行时允许覆盖（便于测试/DI）
        public static void RegisterFactory(Type uiType, WuWuFrameworkFunc<IUIView> factory)
        {
            if (uiType == null) throw new ArgumentNullException(nameof(uiType));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            s_Factories[uiType] = factory;
        }

        private static T CreateUIView<T>() where T : class, IUIView, new()
        {
            return new T();
        }
    }
}