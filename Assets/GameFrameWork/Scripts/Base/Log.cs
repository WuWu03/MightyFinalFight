using GameFrameWork.Utilities;
using UnityEngine;

namespace GameFrameWork
{
    public static class Log
    {
        public static void LogInfo(params object[] args)
        {
            LogInfo(AppConfig.instance.logColor, args);
        }

        public static void LogInfo(Color color, params object[] args)
        {
            if (!AppConfig.instance.openLog)
            {
                return;
            }

            Debug.Log(GetLogInfo(color, args));
        }

        public static void LogError(params object[] args)
        {
            LogError(AppConfig.instance.logColor, args);
        }

        public static void LogError(Color color, params object[] args)
        {
            if (!AppConfig.instance.openLog)
            {
                return;
            }

            Debug.LogError(GetLogInfo(color, args));
        }

        private static string GetLogInfo(Color color, object[] args)
        {
            string logInfo = StringUtil.Format(args);
            string hex = CommonUtil.RGBToHex(color);

            if (!string.IsNullOrEmpty(hex))
            {
                logInfo = StringUtil.Format("<color=", hex, ">", logInfo, "</color>");
            }

            return logInfo;
        }
    }
}