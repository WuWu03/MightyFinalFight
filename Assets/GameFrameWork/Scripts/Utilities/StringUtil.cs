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
        public static string FormatDefault(params object[] args)
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
            }
    
            string format = m_StringBuilder.ToString();
            m_StringBuilder.Clear();
            return Format(format, args);
        }

        public static string Format(string format, params object[] args)
        {
            if (format == null)
            {
                throw new Exception("Format is invalid.");
            }

            m_StringBuilder.Clear();
            m_StringBuilder.AppendFormat(format, args);
            string str = m_StringBuilder.ToString();
            m_StringBuilder.Clear();
            return str;
        }

        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        public static string MD5(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
            byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
            md5.Clear();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5Data.Length; i++)
            {
                sb.Append(System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0'));
            }

            return sb.ToString().PadLeft(32, '0');
        }

        private static StringBuilder m_StringBuilder = new StringBuilder();
    }
}
