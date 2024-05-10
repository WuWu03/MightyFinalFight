using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GameFrameWork.Utilities
{
    public static class StringUtil
    {
        public static string Format(bool isPath, object arg1)
        {
            return Format(isPath, arg1, null, null, null, null, null, null);
        }

        public static string Format(bool isPath, object arg1, object arg2)
        {
            return Format(isPath, arg1, arg2, null, null, null, null, null);
        }

        public static string Format(bool isPath, object arg1, object arg2, object arg3)
        {
            return Format(isPath, arg1, arg2, arg3, null, null, null, null);
        }

        public static string Format(bool isPath, object arg1, object arg2, object arg3, object arg4)
        {
            return Format(isPath, arg1, arg2, arg3, arg4, null, null, null);
        }

        public static string Format(bool isPath, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return Format(isPath, arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static string Format(bool isPath, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return Format(isPath, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static string Format(bool isPath, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            m_ListArgs.Clear();

            AddArg(arg1);
            AddArg(arg2);
            AddArg(arg3);
            AddArg(arg4);
            AddArg(arg5);
            AddArg(arg6);
            AddArg(arg7);

            return Format(isPath);
        }

        public static string Format(bool isPath, params object[] args)
        {
            m_ListArgs.Clear();
            m_ListArgs.AddRange(args);

            return Format(isPath);
        }

        public static string Format(object arg1)
        {
            return Format(false, arg1, null, null, null, null, null, null);
        }

        public static string Format(object arg1, object arg2)
        {
            return Format(false, arg1, arg2, null, null, null, null, null);
        }

        public static string Format(object arg1, object arg2, object arg3)
        {
            return Format(false, arg1, arg2, arg3, null, null, null, null);
        }

        public static string Format(object arg1, object arg2, object arg3, object arg4)
        {
            return Format(false, arg1, arg2, arg3, arg4, null, null, null);
        }

        public static string Format(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return Format(false, arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static string Format(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return Format(false, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static string Format(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            return Format(false, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static string Format(params object[] args)
        {
            m_ListArgs.Clear();
            m_ListArgs.AddRange(args);

            return Format(false);
        }

        public static string FormatDefault(string format, object arg1)
        {
            return FormatDefault(format, arg1, null, null, null, null, null, null);
        }

        public static string FormatDefault(string format, object arg1, object arg2)
        {
            return FormatDefault(format, arg1, arg2, null, null, null, null, null);
        }

        public static string FormatDefault(string format, object arg1, object arg2, object arg3)
        {
            return FormatDefault(format, arg1, arg2, arg3, null, null, null, null);
        }

        public static string FormatDefault(string format, object arg1, object arg2, object arg3, object arg4)
        {
            return FormatDefault(format, arg1, arg2, arg3, arg4, null, null, null);
        }

        public static string FormatDefault(string format, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return FormatDefault(format, arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static string FormatDefault(string format, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return FormatDefault(format, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static string FormatDefault(string format, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            m_ListArgs.Clear();

            AddArg(arg1);
            AddArg(arg2);
            AddArg(arg3);
            AddArg(arg4);
            AddArg(arg5);
            AddArg(arg6);
            AddArg(arg7);

            return FormatDefault(format);
        }

        public static string FormatDefault(string format, params object[] args)
        {
            m_ListArgs.Clear();
            m_ListArgs.AddRange(args);
            return FormatDefault(format);
        }

        private static string Format(bool isPath)
        {
            if (m_ListArgs == null || m_ListArgs.Count < 1)
            {
                return string.Empty;
            }

            if (m_ListArgs.Count < 2)
            {
                return m_ListArgs[0].ToString();
            }

            m_StringBuilder.Clear();

            for (int i = 0; i < m_ListArgs.Count; i++)
            {
                if (isPath && i > 0)
                {
                    bool conditon = m_ListArgs[i] != null;

                    if (m_ListArgs[i] is string argStr)
                    {
                        conditon = !string.IsNullOrEmpty(argStr);
                    }

                    if (conditon)
                    {
                        m_StringBuilder.Append("/");
                    }
                }

                m_StringBuilder.Append(m_ListArgs[i]);
            }

            return m_StringBuilder.ToString();
        }

        private static string FormatDefault(string format)
        {
            if (format == null)
            {
                throw new Exception("Format is invalid.");
            }

            m_StringBuilder.Clear();
            m_StringBuilder.AppendFormat(format, m_ListArgs.ToArray());

            return m_StringBuilder.ToString();
        }

        private static void AddArg(object arg)
        {
            if (arg != null || (arg is string argStr && !string.IsNullOrEmpty(argStr)))
            {
                m_ListArgs.Add(arg);
            }
        }

        /// <summary>
        /// 整数转罗马数字
        /// </summary>
        public static string GetRomanValue(int num)
        {
            int roman1 = 5;
            int roman2 = 10;
            int offset = 1;

            m_StringBuilder.Clear();

            while (num > 0)
            {
                int value = num % 10 * offset;

                if (value == roman2)
                {
                    m_StringBuilder.Append(GetRomanStr(roman2));
                }
                else if (value == roman1)
                {
                    m_StringBuilder.Append(GetRomanStr(roman1));
                }
                else if (value > roman2)
                {
                    int temp = value - roman2;

                    for (int i = 0; i < temp; i += offset)
                    {
                        m_StringBuilder.Append(GetRomanStr(offset));
                    }

                    m_StringBuilder.Append(GetRomanStr(roman2));
                }
                else if (value >= roman2 - offset)
                {
                    m_StringBuilder.Append(GetRomanStr(offset));
                    m_StringBuilder.Append(GetRomanStr(roman2));
                }
                else if (value > roman1)
                {
                    int temp = value - roman1;

                    for (int i = 0; i < temp; i += offset)
                    {
                        m_StringBuilder.Append(GetRomanStr(offset));
                    }

                    m_StringBuilder.Append(GetRomanStr(roman1));
                }
                else if (value >= roman1 - offset)
                {
                    m_StringBuilder.Append(GetRomanStr(offset));
                    m_StringBuilder.Append(GetRomanStr(roman1));
                }
                else
                {
                    for (int i = 0; i < value; i += offset)
                    {
                        m_StringBuilder.Append(GetRomanStr(offset));
                    }
                }

                num /= 10;
                roman1 *= 10;
                roman2 *= 10;
                offset *= 10;
            }

            return m_StringBuilder.ToString();
        }

        private static string GetRomanStr(int num)
        {
            switch (num)
            {
                case 1:
                    return "I";
                case 5:
                    return "V";
                case 10:
                    return "X";
                case 50:
                    return "L";
                case 100:
                    return "C";
                case 500:
                    return "D";
                case 1000:
                    return "M";
            }

            return string.Empty;
        }

        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        public static string MD5(string source)
        {
            return MD5(Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// 计算二进制的MD5
        /// </summary>

        public static string MD5(byte[] source)
        {
            try
            {
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    byte[] result = md5.ComputeHash(source); 

                    m_StringBuilder.Clear();

                    for (int i = 0; i < result.Length; i++)
                    {
                        m_StringBuilder.Append(result[i].ToString("x2"));
                    }

                    return m_StringBuilder.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MD5 caculation error:" + ex.Message);
            }
        }

        private static List<object> m_ListArgs = new List<object>();
        private static StringBuilder m_StringBuilder = new StringBuilder();
    }
}
