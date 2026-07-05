using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace WuWuFramework.Utils
{
    public static class StringUtil
    {
        private static readonly string[] s_ChineseUnits = { "\0", "十", "百", "千", "万", "亿", "兆", "京" };
        private static readonly string[] s_ChineseDigits = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
        private static readonly int[] s_UnitDigits = { 4, 8, 16, 32 };
        private static readonly string s_ChineseNegative = "负";
        private static readonly string s_ChineseDot = "点";
        private static readonly ThreadLocal<StringBuilder> s_ThreadLocalStringBuilder = new(GetThreadLocalStringBuilder);
        private static readonly ThreadLocal<List<string>> s_ThreadLocalArgs = new(GetThreadLocalArgs);
        private static readonly ThreadLocal<string[]> s_ThreadLocalFormatArgs = new(GetThreadLocalFormatArgs);

        private static StringBuilder s_StringBuilder => s_ThreadLocalStringBuilder.Value;
        private static List<string> s_Args => s_ThreadLocalArgs.Value;
        private static string[] s_FormatArgs => s_ThreadLocalFormatArgs.Value;

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static string Format(string format, string arg1)
        {
            return Format(format, arg1, null);
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static string Format(string format, string arg1, string arg2)
        {
            return Format(format, arg1, arg2, null);
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        public static string Format(string format, string arg1, string arg2, string arg3)
        {
            return Format(format, arg1, arg2, arg3, null);
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <returns></returns>
        public static string Format(string format, string arg1, string arg2, string arg3, string arg4)
        {
            return Format(format, arg1, arg2, arg3, arg4, null);
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <returns></returns>
        public static string Format(string format, string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            return Format(format, arg1, arg2, arg3, arg4, arg5, null);
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <returns></returns>
        public static string Format(string format, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            return Format(format, arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <returns></returns>
        public static string Format(string format, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            ClearArgs();
            AddArg(arg1);
            AddArg(arg2);
            AddArg(arg3);
            AddArg(arg4);
            AddArg(arg5);
            AddArg(arg6);
            AddArg(arg7);
            return Format(format);
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(string format, params string[] args)
        {
            ClearArgs();

            foreach (var arg in args)
            {
                if (!string.IsNullOrEmpty(arg))
                {
                    AddArg(arg);
                }
            }

            return Format(format);
        }

        /// <summary>
        /// 追加字符串
        /// </summary>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static string Append(string arg1)
        {
            return Append(arg1, null);
        }

        /// <summary>
        /// 追加字符串
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static string Append(string arg1, string arg2)
        {
            return Append(arg1, arg2, null);
        }

        /// <summary>
        /// 追加字符串
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        public static string Append(string arg1, string arg2, string arg3)
        {
            return Append(arg1, arg2, arg3, null);
        }

        /// <summary>
        /// 追加字符串
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <returns></returns>
        public static string Append(string arg1, string arg2, string arg3, string arg4)
        {
            return Append(arg1, arg2, arg3, arg4, null);
        }

        /// <summary>
        /// 追加字符串
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <returns></returns>
        public static string Append(string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            return Append(arg1, arg2, arg3, arg4, arg5, null);
        }

        /// <summary>
        /// 追加字符串
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <returns></returns>

        public static string Append(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            return Append(arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        /// <summary>
        /// 追加字符串
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <returns></returns>
        public static string Append(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            ClearArgs();
            AddArg(arg1);
            AddArg(arg2);
            AddArg(arg3);
            AddArg(arg4);
            AddArg(arg5);
            AddArg(arg6);
            AddArg(arg7);
            return Append(false);
        }

        /// <summary>
        /// 追加字符串
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Append(params string[] args)
        {
            ClearArgs();

            foreach (var arg in args)
            {
                if (!string.IsNullOrEmpty(arg))
                {
                    AddArg(arg);
                }
            }

            return Append(false);
        }

        /// <summary>
        /// 追加字符串
        /// isPath为true时会在每个参数后面加上"/"，最后一个参数不会加上"/"
        /// 如果最后一个参数是以"."开头的，则倒数第二个参数不会加上"/"
        /// </summary>
        /// <param name="isPath"></param>
        /// <returns></returns>
        public static string Append(bool isPath)
        {
            if (s_Args == null || s_Args.Count < 1)
            {
                return string.Empty;
            }

            s_StringBuilder.Clear();

            for (int i = 0; i < s_Args.Count; i++)
            {
                string currArg = s_Args[i];
                bool canAddPath = false;

                if (isPath)
                {
                    currArg = currArg.Trim().TrimEnd('/').TrimEnd('\\');

                    if (i == s_Args.Count - 2)
                    {
                        string lastArg = s_Args[^1].ToString();
                        canAddPath = !lastArg.StartsWith('.');
                    }
                    else if (i < s_Args.Count - 1)
                    {
                        canAddPath = true;
                    }
                }

                s_StringBuilder.Append(currArg);

                if (canAddPath)
                {
                    s_StringBuilder.Append("/");
                }
            }

            ClearArgs();
            return s_StringBuilder.ToString();
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="arg"></param>
        public static void AddArg(string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                s_Args.Add(arg);
            }
        }

        /// <summary>
        /// 清理参数
        /// </summary>
        public static void ClearArgs()
        {
            s_Args.Clear();
        }

        /// <summary>
        /// 整数转罗马数字
        /// </summary>
        public static string GetRomanValue(int num)
        {
            int roman1 = 5;
            int roman2 = 10;
            int offset = 1;
            s_StringBuilder.Clear();

            while (num > 0)
            {
                int value = num % 10 * offset;

                if (value == roman2)
                {
                    s_StringBuilder.Append(GetRomanStr(roman2));
                }
                else if (value == roman1)
                {
                    s_StringBuilder.Append(GetRomanStr(roman1));
                }
                else if (value > roman2)
                {
                    int temp = value - roman2;

                    for (int i = 0; i < temp; i += offset)
                    {
                        s_StringBuilder.Append(GetRomanStr(offset));
                    }

                    s_StringBuilder.Append(GetRomanStr(roman2));
                }
                else if (value >= roman2 - offset)
                {
                    s_StringBuilder.Append(GetRomanStr(offset));
                    s_StringBuilder.Append(GetRomanStr(roman2));
                }
                else if (value > roman1)
                {
                    int temp = value - roman1;

                    for (int i = 0; i < temp; i += offset)
                    {
                        s_StringBuilder.Append(GetRomanStr(offset));
                    }

                    s_StringBuilder.Append(GetRomanStr(roman1));
                }
                else if (value >= roman1 - offset)
                {
                    s_StringBuilder.Append(GetRomanStr(offset));
                    s_StringBuilder.Append(GetRomanStr(roman1));
                }
                else
                {
                    for (int i = 0; i < value; i += offset)
                    {
                        s_StringBuilder.Append(GetRomanStr(offset));
                    }
                }

                num /= 10;
                roman1 *= 10;
                roman2 *= 10;
                offset *= 10;
            }

            return s_StringBuilder.ToString();
        }

        /// <summary>
        /// 阿拉伯数字转中文数字
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string GetChineseNum(decimal num)
        {
            s_StringBuilder.Clear();
            ClearArgs();

            if (num == 0)
            {
                s_StringBuilder.Append(s_ChineseDigits[0]);
                return s_StringBuilder.ToString();
            }

            decimal integerNum = Math.Round(num, 0);
            decimal decimalNum = num - integerNum;
            decimal tempIntegerNum = integerNum;
            int unitIndex = 0;
            int prevUnitDigit = 0;
            int lastDigit = 0;
            int[] isUnitDigitsAdd = new int[s_UnitDigits.Length];

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
                            AddArg(s_ChineseUnits[tempUnitIndex]);
                            tempUnitIndex -= 4;
                        }
                        else
                        {
                            for (int i = prevUnitDigit; i < s_UnitDigits.Length; i++)
                            {
                                if (tempUnitIndex < s_UnitDigits[i])
                                {
                                    int digitIndex = i - 1;
                                    if (digitIndex > prevUnitDigit)
                                    {
                                        for (int j = 0; j < s_UnitDigits.Length; j++)
                                        {
                                            isUnitDigitsAdd[j] = 0;
                                        }

                                        prevUnitDigit = digitIndex;
                                    }

                                    if (isUnitDigitsAdd[digitIndex] != s_UnitDigits[digitIndex])
                                    {
                                        AddArg(s_ChineseUnits[digitIndex + 4]);
                                        isUnitDigitsAdd[digitIndex] = s_UnitDigits[digitIndex];
                                    }

                                    tempUnitIndex -= s_UnitDigits[digitIndex];
                                    break;
                                }
                            }
                        }
                    }

                    AddArg(s_ChineseDigits[digit]);
                }
                else
                {
                    if (s_Args.Count > 0 && s_Args[^1] != s_ChineseDigits[0])
                    {
                        AddArg(s_ChineseDigits[0]);
                    }
                }

                tempIntegerNum /= 10;
                unitIndex++;
            }

            lastDigit = (int)(tempIntegerNum * 10);

            if (lastDigit == 1 && (unitIndex - 1) % 4 == 1) //十，十万，十亿，十兆等十开头的移除最高位的1，否则会出现“一十万”这样的数字
            {
                s_Args.RemoveAt(s_Args.Count - 1);
            }

            if (integerNum == 0)
            {
                AddArg(s_ChineseDigits[0]);
            }

            if (integerNum < 0)
            {
                AddArg(s_ChineseNegative);
            }

            s_Args.Reverse();

            if (decimalNum > 0)
            {
                AddArg(s_ChineseDot);

                while (decimalNum > 0 && decimalNum < 1)
                {
                    decimalNum *= 10;
                    int decimalValue = (int)(decimalNum);
                    decimalNum -= decimalValue;
                    AddArg(s_ChineseDigits[decimalValue]);
                }
            }

            s_StringBuilder.AppendJoin(string.Empty, s_Args);
            return s_StringBuilder.ToString();
        }


        /// <summary>
        /// 计算文件大小的字符串表示，单位为 B、KB、MB、GB、TB、PB
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        /// <exception cref="WuWuFrameworkException"></exception>
        public static string FormatFileSize(ulong bytes, int digits = 2)
        {
            int counter = 0;
            double number = bytes;

            // 最大单位就是 PB，从 0 开始数: "Bytes", "KB", "MB", "GB", "TB", "PB"，PB 是第 5 级
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
                _ => throw new WuWuFrameworkException("单位等级溢出，更新 [maxCount] 等级")
            };

            return Format("{0}{1}", number.ToString(CultureInfo.InvariantCulture), suffix);
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

                s_StringBuilder.Clear();

                foreach (var num in result)
                {
                    s_StringBuilder.Append(num.ToString("x2"));
                }

                return s_StringBuilder.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public static void Dispose()
        {
            s_ThreadLocalStringBuilder?.Dispose();
            s_ThreadLocalArgs?.Dispose();
            s_ThreadLocalFormatArgs?.Dispose();
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        /// <exception cref="WuWuFrameworkException"></exception>
        private static string Format(string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                throw new WuWuFrameworkException("格式串错误");
            }

            for (int i = 0; i < s_Args.Count; i++)
            {
                s_FormatArgs[i] = s_Args[i];
            }

            for (int i = s_Args.Count; i < s_FormatArgs.Length; i++)
            {
                s_FormatArgs[i] = null;
            }

            s_StringBuilder.Clear();
            s_StringBuilder.AppendFormat(format, s_FormatArgs);
            return s_StringBuilder.ToString();
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

        private static StringBuilder GetThreadLocalStringBuilder()
        {
            return new StringBuilder(256);
        }

        private static List<string> GetThreadLocalArgs()
        {
            return new List<string>(10);
        }

        private static string[] GetThreadLocalFormatArgs()
        {
            return new string[10];
        }
    }
}