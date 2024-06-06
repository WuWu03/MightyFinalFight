using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;

namespace GameFrameWork.ConfigData
{
    public class ConfigDataParser : IDisposable
    {
        /// <summary>
        /// 行数
        /// </summary>
        public int row { get; private set; }

        /// <summary>
        /// 列数
        /// </summary>
        public int column { get; private set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string[] fieldName
        {
            get
            {
                return m_FieldNames;
            }
        }

        /// <summary>
        /// 是否结束
        /// </summary>
        public bool eof
        {
            get
            {
                return m_CurrRow == row - 1;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigDataParser(byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            m_DicFieldName = new Dictionary<string, int>();
            byte[] buffer = ZlibHelper.DeCompressBytes(bytes);//1解压缩

            using (MemoryStreamEx mse = new MemoryStreamEx(buffer))//2解析数据到数组
            {
                row = mse.ReadInt();
                column = mse.ReadInt();

                m_Datas = new String[row - 1, column];
                m_FieldNames = new string[column];

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        string str = mse.ReadUTF8String();

                        if (i == 0)//表示读取的是字段
                        {
                            m_FieldNames[j] = str;
                            m_DicFieldName[str] = j;
                        }
                        else//表示读取的是数据
                        {
                            m_Datas[i - 1, j] = str;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 转到下一条
        /// </summary>
        public void Next()
        {
            if (eof)
            {
                return;
            }

            m_CurrRow++;
        }

        /// <summary>
        /// 获取字段值
        /// </summary>
        /// <returns></returns>
        public string GetFieldValue(string fieldName)
        {
            try
            {
                if (m_CurrRow < 0 || m_CurrRow >= row)
                {
                    return string.Empty;
                }

                return m_Datas[m_CurrRow, m_DicFieldName[fieldName]];
            }
            catch 
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            m_DicFieldName.Clear();
            m_DicFieldName = null;

            m_FieldNames = null;
            m_Datas = null;
        }

        /// <summary>
        /// 字段名称
        /// </summary>
        private string[] m_FieldNames;

        /// <summary>
        /// 游戏数据
        /// </summary>
        private string[,] m_Datas;

        /// <summary>
        /// 当前行号
        /// </summary>
        private int m_CurrRow = 0;

        /// <summary>
        /// 字段名称字典
        /// </summary>
        private Dictionary<string, int> m_DicFieldName;
    }
}