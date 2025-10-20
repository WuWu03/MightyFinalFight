using System;
using System.Collections.Generic;
using GameFrameWork.Utils;

namespace GameFrameWork
{
    public static class GameFrameWorkMgr
    {
        private static readonly LinkedList<GameFrameWorkModule> s_GameFrameWorkModules = new();

        /// <summary>
        /// 所有模块Update
        /// </summary>
        /// <param name="deltaTime">帧间隔（受时间缩放影响）</param>
        /// <param name="unscaledDeltaTime">帧间隔（不受时间缩放影响）</param>
        /// <param name="time">当前时间（受时间缩放影响）</param>
        /// <param name="unscaledTime">当前时间（不受时间缩放影响）</param>
        public static void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            for (LinkedListNode<GameFrameWorkModule> current = s_GameFrameWorkModules.Last; current != null; current = current.Previous)
            {
                current.Value.Update(deltaTime, unscaledDeltaTime, time, unscaledTime);
            }
        }

        /// <summary>
        /// 所有模块LateUpdate
        /// </summary>
        /// <param name="deltaTime">帧间隔（受时间缩放影响）</param>
        /// <param name="unscaledDeltaTime">帧间隔（不受时间缩放影响）</param>
        /// <param name="time">当前时间（受时间缩放影响）</param>
        /// <param name="unscaledTime">当前时间（不受时间缩放影响）</param>
        public static void LateUpdate(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            for (LinkedListNode<GameFrameWorkModule> current = s_GameFrameWorkModules.Last; current != null; current = current.Previous)
            {
                current.Value.LateUpdate(deltaTime, unscaledDeltaTime, time, unscaledTime);
            }
        }
        
        /// <summary>
        /// 所有模块FixedUpdate
        /// </summary>
        /// <param name="fixedDeltaTime">帧间隔（受时间缩放影响）</param>
        /// <param name="fixedUnscaledDeltaTime">帧间隔（不受时间缩放影响）</param>
        /// <param name="fixedTime">当前时间（受时间缩放影响）</param>
        /// <param name="fixedUnscaledTime">当前时间（不受时间缩放影响）</param>
        public static void FixedUpdate(float fixedDeltaTime, float fixedUnscaledDeltaTime, float fixedTime, float fixedUnscaledTime)
        {
            for (LinkedListNode<GameFrameWorkModule> current = s_GameFrameWorkModules.Last; current != null; current = current.Previous)
            {
                current.Value.FixedUpdate(fixedDeltaTime, fixedUnscaledDeltaTime, fixedTime, fixedUnscaledTime);
            }
        }
        
        /// <summary>
        /// 关闭并清理所有游戏模块。
        /// </summary>
        public static void Shutdown()
        {
            for (LinkedListNode<GameFrameWorkModule> current = s_GameFrameWorkModules.First; current != null; current = current.Next)
            {
                current.Value.Shutdown();
            }

            s_GameFrameWorkModules.Clear();
            ReferencePool.Shutdown();
        }

        /// <summary>
        /// 获取模块。
        /// </summary>
        public static T GetModule<T>() where T : class
        {
            Type interfaceType = typeof(T);
            if (!interfaceType.IsInterface)
            {
                throw new Exception(StringUtil.Append("必须以接口形式获取模块，[", interfaceType.FullName, "] 不是接口"));
            }

            if (string.IsNullOrEmpty(interfaceType.FullName))
            {
                throw new Exception(StringUtil.Append("[", interfaceType.FullName, "]接口类型错误"));
            }

            if (!interfaceType.FullName.StartsWith("GameFrameWork.", StringComparison.Ordinal))
            {
                throw new Exception(StringUtil.Append("[", interfaceType.FullName, "]接口类型错误，该接口非框架模块接口"));
            }

            foreach (GameFrameWorkModule module in s_GameFrameWorkModules)
            {
                if (module is T result)
                {
                    return result;
                }
            }

            string moduleName = StringUtil.Append(interfaceType.Namespace,".", interfaceType.Name.Substring(1));
            return CreateModule<T>(moduleName);
        }

        /// <summary>
        /// 创建模块。
        /// </summary>
        private static T CreateModule<T>(string moduleName) where T : class
        {
            Type moduleType = Type.GetType(moduleName);
            
            if (moduleType == null)
            {
                throw new Exception( StringUtil.Append("获取模块 [", moduleName,"] 类型失败"));
            }
            
            T instance = Activator.CreateInstance(moduleType) as T;

            if (instance == null)
            {
                throw new Exception(StringUtil.Append("创建模块 [", moduleName, "] 失败"));
            }

            if (instance is GameFrameWorkModule gameFrameWorkModule)
            {
                LinkedListNode<GameFrameWorkModule> current = s_GameFrameWorkModules.First;

                while (current != null)
                {
                    if (gameFrameWorkModule.priority > current.Value.priority)
                    {
                        break;
                    }

                    current = current.Next;
                }

                if (current != null)
                {
                    s_GameFrameWorkModules.AddBefore(current, gameFrameWorkModule);
                }
                else
                {
                    s_GameFrameWorkModules.AddLast(gameFrameWorkModule);
                }

                return instance;
            }

            throw new Exception(StringUtil.Append("创建模块 [", moduleName, "] 失败"));
        }
    }
}