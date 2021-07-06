using GameFrameWork.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Log
{
    public static class Debugger
    {
        public static Color LogColor = Color.white;
        public static void Log(string logStr = "", params object[] args)
        {
            if (!AppConfig.Ins.OpenLog) return;
            Debug.Log(GetLogInfo(logStr, args));
        }

        public static void LogError(string logStr = "", params object[] args)
        {
            if (!AppConfig.Ins.OpenLog) return;
            Debug.LogError(GetLogInfo(logStr, args));
        }

        private static string GetLogInfo(string logStr, object[] args)
        {
            string logInfo = logStr;

            if (args != null && args.Length > 0)
            {
                logInfo = !string.IsNullOrEmpty(logStr) ? logStr + ":" : string.Empty;

                for (int i = 0; i < args.Length; i++)
                {
                    logInfo = string.Format("{0}[{1}]{2}", logInfo, args[i], i < args.Length - 1 ? "," : string.Empty);
                }
            }

            string color = Utility.ToRGBHex(LogColor);

            if (!string.IsNullOrEmpty(color))
            {
                logInfo = string.Format("<color={0}>{1}</color>", color, logInfo);
            }

            return logInfo;
        }
    }
}