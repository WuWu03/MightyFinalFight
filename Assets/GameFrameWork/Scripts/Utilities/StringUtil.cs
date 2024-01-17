using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

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
                    m_StringBuilder.Append("/");
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

                    for (int i = 0; i < source.Length; i++)
                    {
                        m_StringBuilder.Append(source[i].ToString("x2"));
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
