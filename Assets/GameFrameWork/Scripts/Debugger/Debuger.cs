using GameFrameWork.Utilities;
using UnityEngine;

namespace GameFrameWork.Log
{
    public static class GameFrameworkLog
    {
        public static void Log(params object[] args)
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
            m_LogColor = AppConfig.instance.logColor;

            string logInfo = StringUtil.FormatDefault(args);
            string color = CommonUtil.ToRGBHex(m_LogColor);

            if (!string.IsNullOrEmpty(color))
            {
                logInfo = StringUtil.FormatDefault("<color=", color, ">", logInfo, "</color>");
            }

            return logInfo;
        }

        public static Color m_LogColor = Color.white;
    }
}