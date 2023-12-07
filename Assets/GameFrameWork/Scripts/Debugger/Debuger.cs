using GameFrameWork.Utilities;
using UnityEngine;

namespace GameFrameWork.Debug
{
    public static class GameFrameworkLog
    {
        public static void Debug(params object[] args)
        {
            if (!AppConfig.instance.openLog)
            {
                return;
            }

            UnityEngine.Debug.Log(GetDebugInfo(args));
        }

        public static void DebugError(params object[] args)
        {
            if (!AppConfig.instance.openLog)
            {
                return;
            }

            UnityEngine.Debug.LogError(GetDebugInfo(args));
        }

        private static string GetDebugInfo(object[] args)
        {
            m_LogColor = AppConfig.instance.logColor;

            string logInfo = StringUtil.FormatDefault(args);
            string color = CommonUtil.RGBToHex(m_LogColor);

            if (!string.IsNullOrEmpty(color))
            {
                logInfo = StringUtil.FormatDefault("<color=", color, ">", logInfo, "</color>");
            }

            return logInfo;
        }

        public static Color m_LogColor = Color.white;
    }
}