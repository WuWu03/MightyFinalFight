using GameFrameWork.Utility;
using UnityEngine;

namespace GameFrameWork.Log
{
    public static class GameFrameworkLog
    {
        public static void Log(params object[] args)
        {
            if (!AppConfig.Ins.OpenLog) return;
            Debug.Log(GetLogInfo(args));
        }

        public static void LogError(params object[] args)
        {
            if (!AppConfig.Ins.OpenLog) return;
            Debug.LogError(GetLogInfo(args));
        }

        private static string GetLogInfo(object[] args)
        {
            m_LogColor = AppConfig.Ins.LogColor;

            string logInfo = TextUtil.FormatDefault(args);
            string color = Util.ToRGBHex(m_LogColor);

            if (!string.IsNullOrEmpty(color))
            {
                logInfo = TextUtil.FormatDefault("<color=", color, ">", logInfo, "</color>");
            }

            return logInfo;
        }

        public static Color m_LogColor = Color.white;
    }
}