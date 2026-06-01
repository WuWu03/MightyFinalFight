using WuWuFramework.Utils;
using UnityEngine;

namespace WuWuFramework
{
    public static class Log
    {
        public static void LogInfo(string arg1)
        {
            LogInfo(WuWuFrameworkEntry.config.logColor, arg1, null, null, null, null, null, null);
        }

        public static void LogInfo(string arg1, string arg2)
        {
            LogInfo(WuWuFrameworkEntry.config.logColor, arg1, arg2, null, null, null, null, null);
        }

        public static void LogInfo(string arg1, string arg2, string arg3)
        {
            LogInfo(WuWuFrameworkEntry.config.logColor, arg1, arg2, arg3, null, null, null, null);
        }

        public static void LogInfo(string arg1, string arg2, string arg3, string arg4)
        {
            LogInfo(WuWuFrameworkEntry.config.logColor, arg1, arg2, arg3, arg4, null, null, null);
        }

        public static void LogInfo(string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            LogInfo(WuWuFrameworkEntry.config.logColor, arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static void LogInfo(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            LogInfo(WuWuFrameworkEntry.config.logColor, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static void LogInfo(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            LogInfo(WuWuFrameworkEntry.config.logColor, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void LogInfo(params string[] args)
        {
            LogInfo(WuWuFrameworkEntry.config.logColor, args);
        }

        public static void LogInfo(Color color, string arg1)
        {
            LogInfo(color, arg1, null, null, null, null, null, null);
        }

        public static void LogInfo(Color color, string arg1, string arg2)
        {
            LogInfo(color, arg1, arg2, null, null, null, null, null);
        }

        public static void LogInfo(Color color, string arg1, string arg2, string arg3)
        {
            LogInfo(color, arg1, arg2, arg3, null, null, null, null);
        }

        public static void LogInfo(Color color, string arg1, string arg2, string arg3, string arg4)
        {
            LogInfo(color, arg1, arg2, arg3, arg4, null, null, null);
        }

        public static void LogInfo(Color color, string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            LogInfo(color, arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static void LogInfo(Color color, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            LogInfo(color, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static void LogInfo(Color color, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            if (!WuWuFrameworkEntry.config.isOpenLog)
            {
                return;
            }

            Debug.Log(GetLogInfo(color, arg1, arg2, arg3, arg4, arg5, arg6, arg7));
        }

        public static void LogInfo(Color color, params string[] args)
        {
            if (!WuWuFrameworkEntry.config.isOpenLog)
            {
                return;
            }

            Debug.Log(GetLogInfo(color, args));
        }

        public static void LogError(string arg1)
        {
            LogError(WuWuFrameworkEntry.config.logColor, arg1, null, null, null, null, null, null);
        }

        public static void LogError(string arg1, string arg2)
        {
            LogError(WuWuFrameworkEntry.config.logColor, arg1, arg2, null, null, null, null, null);
        }

        public static void LogError(string arg1, string arg2, string arg3)
        {
            LogError(WuWuFrameworkEntry.config.logColor, arg1, arg2, arg3, null, null, null, null);
        }

        public static void LogError(string arg1, string arg2, string arg3, string arg4)
        {
            LogError(WuWuFrameworkEntry.config.logColor, arg1, arg2, arg3, arg4, null, null, null);
        }

        public static void LogError(string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            LogError(WuWuFrameworkEntry.config.logColor, arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static void LogError(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            LogError(WuWuFrameworkEntry.config.logColor, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static void LogError(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            LogError(WuWuFrameworkEntry.config.logColor, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void LogError(params string[] args)
        {
            LogError(WuWuFrameworkEntry.config.logColor, args);
        }

        public static void LogError(Color color, string arg1)
        {
            LogError(color, arg1, null, null, null, null, null, null);
        }

        public static void LogError(Color color, string arg1, string arg2)
        {
            LogError(color, arg1, arg2, null, null, null, null, null);
        }

        public static void LogError(Color color, string arg1, string arg2, string arg3)
        {
            LogError(color, arg1, arg2, arg3, null, null, null, null);
        }

        public static void LogError(Color color, string arg1, string arg2, string arg3, string arg4)
        {
            LogError(color, arg1, arg2, arg3, arg4, null, null, null);
        }

        public static void LogError(Color color, string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            LogError(color, arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static void LogError(Color color, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            LogError(color, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static void LogError(Color color, string arg1, string arg2, string arg3, string arg4, string  arg5, string arg6, string arg7)
        {
            Debug.LogError(GetLogInfo(color, arg1, arg2, arg3, arg4, arg5, arg6, arg7));
        }

        public static void LogError(Color color, params string[] args)
        {
            Debug.LogError(GetLogInfo(color, args));
        }

        private static string GetLogInfo(Color color, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            string logInfo = StringUtil.Append(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return GetLogInfo(color, logInfo);
        }

        private static string GetLogInfo(Color color, params string[] args)
        {
            string logInfo = StringUtil.Append(args);
            return GetLogInfo(color, logInfo);
        }

        private static string GetLogInfo(Color color, string logInfo)
        {
            string hex = CommonUtil.RGBToHex(color);

            if (!string.IsNullOrEmpty(hex))
            {
                logInfo = StringUtil.Append("<color=", hex, ">", logInfo, "</color>");
            }

            return logInfo;
        }
    }
}