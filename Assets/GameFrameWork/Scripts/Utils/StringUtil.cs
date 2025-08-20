using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace GameFrameWork.Utils
{
    public static class StringUtil
    {
        public static string Format(string format, string arg1)
        {
            return Format(format, arg1, null, null, null, null, null, null);
        }

        public static string Format(string format, string arg1, string arg2)
        {
            return Format(format, arg1, arg2, null, null, null, null, null);
        }

        public static string Format(string format, string arg1, string arg2, string arg3)
        {
            return Format(format, arg1, arg2, arg3, null, null, null, null);
        }

        public static string Format(string format, string arg1, string arg2, string arg3, string arg4)
        {
            return Format(format, arg1, arg2, arg3, arg4, null, null, null);
        }

        public static string Format(string format, string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            return Format(format, arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static string Format(string format, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            return Format(format, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static string Format(string format, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            m_ListArgs.Clear();

            AddArg(arg1);
            AddArg(arg2);
            AddArg(arg3);
            AddArg(arg4);
            AddArg(arg5);
            AddArg(arg6);
            AddArg(arg7);

            return Format(format);
        }

        public static string Format(string format, params string[] args)
        {
            m_ListArgs.Clear();
            m_ListArgs.AddRange(args);

            return Format(format);
        }

        public static string Append(string arg1)
        {
            return Append(arg1, null, null, null, null, null, null);
        }

        public static string Append(string arg1, string arg2)
        {
            return Append(arg1, arg2, null, null, null, null, null);
        }

        public static string Append(string arg1, string arg2, string arg3)
        {
            return Append(arg1, arg2, arg3, null, null, null, null);
        }

        public static string Append(string arg1, string arg2, string arg3, string arg4)
        {
            return Append(arg1, arg2, arg3, arg4, null, null, null);
        }

        public static string Append(string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            return Append(arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static string Append(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            return Append(arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static string Append(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            m_ListArgs.Clear();

            AddArg(arg1);
            AddArg(arg2);
            AddArg(arg3);
            AddArg(arg4);
            AddArg(arg5);
            AddArg(arg6);
            AddArg(arg7);

            return Append(false);
        }

        public static string Append(params string[] args)
        {
            m_ListArgs.Clear();
            m_ListArgs.AddRange(args);

            return Append(false);
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

        public static string GetChineseNum(decimal num)
        {
            if (num == 0)
            {
                return new string(new char[] { m_ChineseDigit[0] });
            }

            decimal integerNum = Math.Round(num, 0);
            decimal decimalNum = num - integerNum;
            decimal tempIntegerNum = integerNum;
            int unitIndex = 0;
            int prevUnitDigit = 0;
            int lastDigit = 0;
            int[] isUnitDigtsAdd = new int[m_UnitDigits.Length];
            m_StringBuilder.Clear();

            while (tempIntegerNum >= 1)
            {
                int digit = (int)(tempIntegerNum % 10);

                if (digit > 0)
                {
                    string chineseUnitStr = string.Empty;
                    int tempUnitIndex = unitIndex;

                    while (tempUnitIndex > 0)
                    {
                        if (tempUnitIndex < 4)
                        {
                            m_StringBuilder.Insert(0, m_ChineseUnit[tempUnitIndex]);
                            tempUnitIndex -= 4;
                        }
                        else
                        {
                            for (int i = prevUnitDigit; i < m_UnitDigits.Length; i++)
                            {
                                if (tempUnitIndex < m_UnitDigits[i])
                                {
                                    int digitIndex = i - 1;
                                    if (digitIndex > prevUnitDigit)
                                    {
                                        for (int j = 0; j < m_UnitDigits.Length; j++)
                                        {
                                            isUnitDigtsAdd[j] = 0;
                                        }

                                        prevUnitDigit = digitIndex;
                                    }

                                    if (isUnitDigtsAdd[digitIndex] != m_UnitDigits[digitIndex])
                                    {
                                        m_StringBuilder.Insert(0, m_ChineseUnit[digitIndex + 4]);
                                        isUnitDigtsAdd[digitIndex] = m_UnitDigits[digitIndex];
                                    }

                                    tempUnitIndex -= m_UnitDigits[digitIndex];
                                    break;
                                }
                            }
                        }
                    }

                    m_StringBuilder.Insert(0, m_ChineseDigit[digit]);
                }
                else
                {
                    if (m_StringBuilder.Length > 0 && m_StringBuilder[0] != m_ChineseDigit[0])
                    {
                        m_StringBuilder.Insert(0, m_ChineseDigit[0]);
                    }
                }

                tempIntegerNum /= 10;
                unitIndex++;
            }

            lastDigit = (int)(tempIntegerNum * 10);

            if (lastDigit == 1 && (unitIndex - 1) % 4 == 1)//十，十万，十亿，十兆等十开头的移除最高位的1，否则会出现一十五这样的数字
            {
                m_StringBuilder.Remove(0, 1);
            }


            if (integerNum == 0)
            {
                m_StringBuilder.Insert(0, m_ChineseDigit[0]);
            }

            if (decimalNum > 0)
            {
                m_StringBuilder.Append(m_ChineseDot);

                while (decimalNum > 0 && decimalNum < 1)
                {
                    decimalNum *= 10;
                    int decimalValue = (int)(decimalNum);
                    decimalNum -= decimalValue;
                    m_StringBuilder.Append(m_ChineseDigit[decimalValue]);
                }
            }

            if (integerNum < 0)
            {
                m_StringBuilder.Insert(0, m_ChineseNegative);
            }

            return m_StringBuilder.ToString();
        }

        public static string FormatFileSize(ulong bytes,int digits = 2)
        {
            int counter = 0;
            double number = bytes;

            // 最大单位就是 PB 了，而 PB 是第 5 级，从 0 开始数
            // "Bytes", "KB", "MB", "GB", "TB", "PB"
            const int maxCount = 5;

            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;

                if (counter >= maxCount)
                {
                    break;
                }
            }

            number = Math.Round(number, digits);

            var suffix = counter switch
            {
                0 => "B",
                1 => "KB",
                2 => "MB",
                3 => "GB",
                4 => "TB",
                5 => "PB",
                // 通过 maxCount 限制了最大的值就是 5 了
                _ => throw new ArgumentException("骚年，你是不是忘了更新 maxCount 等级了")
            };

            return Format("{0}{1}", number.ToString(), suffix);
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
                using MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(source);

                m_StringBuilder.Clear();

                for (int i = 0; i < result.Length; i++)
                {
                    m_StringBuilder.Append(result[i].ToString("x2"));
                }

                return m_StringBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("MD5 caculation error:" + ex.Message);
            }
        }

        public static void AddArg(string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                m_ListArgs.Add(arg);
            }
        }

        public static void ClearArgs()
        {
            if (m_ListArgs != null && m_ListArgs.count > 0)
            {
                m_ListArgs.Clear();
            }
        }

        public static string Append(bool isPath)
        {
            if (m_ListArgs == null || m_ListArgs.count < 1)
            {
                return string.Empty;
            }

            if (m_ListArgs.count < 2)
            {
                return m_ListArgs[0].ToString();
            }

            m_StringBuilder.Clear();

            for (int i = 0; i < m_ListArgs.count; i++)
            {
                string arg = m_ListArgs[i];
                bool addPath = isPath && !string.IsNullOrEmpty(arg) && !arg.EndsWith("/") && i < m_ListArgs.count - 1;

                if (i == m_ListArgs.count - 2)
                {
                    addPath = addPath && !m_ListArgs[i + 1].StartsWith(".");
                }

                m_StringBuilder.Append(m_ListArgs[i]);

                if (addPath)
                {
                    m_StringBuilder.Append('/');
                }
            }

            m_ListArgs.Clear();
            return m_StringBuilder.ToString();
        }

        private static string Format(string format)
        {
            if (format == null)
            {
                throw new Exception("Format is invalid.");
            }

            m_StringBuilder.Clear();
            m_StringBuilder.AppendFormat(format, m_ListArgs.ToArray());

            return m_StringBuilder.ToString();
        }

        private static string GetRomanStr(int num)
        {
            return num switch
            {
                1 => "I",
                5 => "V",
                10 => "X",
                50 => "L",
                100 => "C",
                500 => "D",
                1000 => "M",
                _ => string.Empty,
            };
        }

        private static readonly char[] m_ChineseUnit = { default, '十', '百', '千', '万', '亿', '兆', '京' };
        private static readonly char[] m_ChineseDigit = { '零', '一', '二', '三', '四', '五', '六', '七', '八', '九', '十' };
        private static readonly int[] m_UnitDigits = { 4, 8, 16, 32 };
        private static readonly char m_ChineseNegative = '负';
        private static readonly char m_ChineseDot = '点';

        private static readonly SmallList<string> m_ListArgs = new();
        private static readonly StringBuilder m_StringBuilder = new();
    }
}