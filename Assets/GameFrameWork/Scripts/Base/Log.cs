using GameFrameWork.Utils;
using UnityEngine;

namespace GameFrameWork
{
    public static class Log
    {
        public static void LogInfo(object arg1)
        {
            LogInfo(GameFrameWorkEntry.config.logColor, arg1, null, null, null, null, null, null);
        }

        public static void LogInfo(object arg1, object arg2)
        {
            LogInfo(GameFrameWorkEntry.config.logColor, arg1, arg2, null, null, null, null, null);
        }

        public static void LogInfo(object arg1, object arg2, object arg3)
        {
            LogInfo(GameFrameWorkEntry.config.logColor, arg1, arg2, arg3, null, null, null, null);
        }

        public static void LogInfo(object arg1, object arg2, object arg3, object arg4)
        {
            LogInfo(GameFrameWorkEntry.config.logColor, arg1, arg2, arg3, arg4, null, null, null);
        }

        public static void LogInfo(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            LogInfo(GameFrameWorkEntry.config.logColor, arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static void LogInfo(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            LogInfo(GameFrameWorkEntry.config.logColor, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static void LogInfo(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            LogInfo(GameFrameWorkEntry.config.logColor, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void LogInfo(params object[] args)
        {
            LogInfo(GameFrameWorkEntry.config.logColor, args);
        }

        public static void LogInfo(Color color, object arg1)
        {
            LogInfo(color, arg1, null, null, null, null, null, null);
        }

        public static void LogInfo(Color color, object arg1, object arg2)
        {
            LogInfo(color, arg1, arg2, null, null, null, null, null);
        }

        public static void LogInfo(Color color, object arg1, object arg2, object arg3)
        {
            LogInfo(color, arg1, arg2, arg3, null, null, null, null);
        }

        public static void LogInfo(Color color, object arg1, object arg2, object arg3, object arg4)
        {
            LogInfo(color, arg1, arg2, arg3, arg4, null, null, null);
        }

        public static void LogInfo(Color color, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            LogInfo(color, arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static void LogInfo(Color color, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            LogInfo(color, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static void LogInfo(Color color, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            if (!GameFrameWorkEntry.config.isOpenLog)
            {
                return;
            }

            Debug.Log(GetLogInfo(color, arg1, arg2, arg3, arg4, arg5, arg6, arg7));
        }

        public static void LogInfo(Color color, params object[] args)
        {
            if (!GameFrameWorkEntry.config.isOpenLog)
            {
                return;
            }

            Debug.Log(GetLogInfo(color, args));
        }

        public static void LogError(object arg1)
        {
            LogError(GameFrameWorkEntry.config.logColor, arg1, null, null, null, null, null, null);
        }

        public static void LogError(object arg1, object arg2)
        {
            LogError(GameFrameWorkEntry.config.logColor, arg1, arg2, null, null, null, null, null);
        }

        public static void LogError(object arg1, object arg2, object arg3)
        {
            LogError(GameFrameWorkEntry.config  , arg1, arg2, arg3, null, null, null, null);
        }

        public static void LogError(object arg1, object arg2, object arg3, object arg4)
        {
            LogError(GameFrameWorkEntry.config.logColor, arg1, arg2, arg3, arg4, null, null, null);
        }

        public static void LogError(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            LogError(GameFrameWorkEntry.config.logColor, arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static void LogError(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            LogError(GameFrameWorkEntry.config.logColor, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static void LogError(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            LogError(GameFrameWorkEntry.config.logColor, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void LogError(params object[] args)
        {
            LogError(GameFrameWorkEntry.config.logColor, args);
        }

        public static void LogError(Color color, object arg1)
        {
            LogError(color, arg1, null, null, null, null, null, null);
        }

        public static void LogError(Color color, object arg1, object arg2)
        {
            LogError(color, arg1, arg2, null, null, null, null, null);
        }

        public static void LogError(Color color, object arg1, object arg2, object arg3)
        {
            LogError(color, arg1, arg2, arg3, null, null, null, null);
        }

        public static void LogError(Color color, object arg1, object arg2, object arg3, object arg4)
        {
            LogError(color, arg1, arg2, arg3, arg4, null, null, null);
        }

        public static void LogError(Color color, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            LogError(color, arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static void LogError(Color color, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            LogError(color, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static void LogError(Color color, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            Debug.LogError(GetLogInfo(color, arg1, arg2, arg3, arg4, arg5, arg6, arg7));
        }

        public static void LogError(Color color, params object[] args)
        {
            Debug.LogError(GetLogInfo(color, args));
        }

        private static string GetLogInfo(Color color, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            string logInfo = StringUtil.Format(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return GetLogInfo(color, logInfo);
        }

        private static string GetLogInfo(Color color, params object[] args)
        {
            string logInfo = StringUtil.Format(args);
            return GetLogInfo(color, logInfo);
        }

        private static string GetLogInfo(Color color, string logInfo)
        {
            string hex = CommonUtil.RGBToHex(color);

            if (!string.IsNullOrEmpty(hex))
            {
                logInfo = StringUtil.Format("<color=", hex, ">", logInfo, "</color>");
            }

            return logInfo;
        }
    }
}