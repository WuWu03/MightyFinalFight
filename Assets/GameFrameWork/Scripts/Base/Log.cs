using GameFrameWork.Utilities;
using UnityEngine;

namespace GameFrameWork
{
    public static class Log
    {
        public static void LogInfo(params object[] args)
        {
            if (!AppConfig.instance.openLog)
            {
                return;
            }

            Debug.Log(GetLogInfo(args));
        }

        public static void LogError(params object[] args)
        {
            if (!AppConfig.instance.openLog)
            {
                return;
            }

            Debug.LogError(GetLogInfo(args));
        }

        private static string GetLogInfo(object[] args)
        {
            string logInfo = StringUtil.Format(args);
            string color = CommonUtil.RGBToHex(AppConfig.instance.logColor);

            if (!string.IsNullOrEmpty(color))
            {
                logInfo = StringUtil.Format("<color=", color, ">", logInfo, "</color>");
            }

            return logInfo;
        }
    }
}