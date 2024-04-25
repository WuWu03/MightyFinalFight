using System;
using System.Security.Cryptography;
using System.Text;

namespace GameFrameWork.Utilities
{
    public static class StringUtil
    {
        public static string Format(params object[] args)
        {
            return Format(false, args);
        }

        public static string Format(bool isPath, params object[] args)
        {
            if (args == null)
            {
                throw new Exception("Args is invalid.");
            }

            if (args.Length < 1)
            {
                return string.Empty;
            }

            if (args.Length < 2)
            {
                return args[0].ToString();
            }

            m_StringBuilder.Clear();

            for (int i = 0; i < args.Length; i++)
            {
                m_StringBuilder.Append("{");
                m_StringBuilder.AppendFormat("{0}", i);
                m_StringBuilder.Append("}");

                if (isPath && i < args.Length - 1)
                {
                    bool conditon = args[i] != null;

                    if (args[i] is string)
                    {
                        conditon = !string.IsNullOrEmpty(args[i] as string);
                    }

                    if(conditon)
                    {
                        m_StringBuilder.Append("/");
                    }
                }
            }

            return FormatDefault(m_StringBuilder.ToString(), args);
        }

        public static string FormatDefault(string format, params object[] args)
        {
            if (format == null)
            {
                throw new Exception("Format is invalid.");
            }

            m_StringBuilder.Clear();
            m_StringBuilder.AppendFormat(format, args);

            return m_StringBuilder.ToString();
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

        private static StringBuilder m_StringBuilder = new StringBuilder();
    }
}
