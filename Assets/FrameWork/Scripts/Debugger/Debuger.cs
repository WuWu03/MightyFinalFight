using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    public static bool IsLog = true;
    public static Color LogColor = Color.black;
    public static void Log(string logStr = "", params object[] args)
    {
        if (!IsLog) return;
        Debug.Log(GetLogInfo(logStr, args));
    }

    public static void LogError(string logStr = "", params object[] args)
    {
        if (!IsLog) return;
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

        string color = ToRGBHex(LogColor);
        LogColor = Color.black;

        if (!string.IsNullOrEmpty(color))
        {
            logInfo = string.Format("<color={0}>{1}</color>", color, logInfo);
        }

        return logInfo;
    }

    private static string ToRGBHex(Color c)
    {
        if (c == default(Color)) c = Color.black;
        return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a));
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }
}
