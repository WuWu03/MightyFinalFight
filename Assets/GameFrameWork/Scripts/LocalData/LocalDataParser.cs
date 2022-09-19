using GameFrameWork.Resources;
using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFrameWork.LocalData
{
    public class LocalDataParser : IDisposable
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
            get { return m_FieldName; }
        }

        /// <summary>
        /// 是否结束
        /// </summary>
        public bool eof
        {
            get
            {
                return m_CurRowNo == row - 1;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LocalDataParser(string path)
        {
            m_FieldNameDic = new Dictionary<string, int>();

            TextAsset txt = ResMgr.instance.LoadAsset<TextAsset>(path);

            //2解压缩
            byte[] buffer = ZlibHelper.DeCompressBytes(txt.bytes);

            //3解析数据到数组
            using (MemoryStreamEx mse = new MemoryStreamEx(buffer))
            {
                row = mse.ReadInt();
                column = mse.ReadInt();

                m_GameData = new String[row - 1, column];
                m_FieldName = new string[column];

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        string str = mse.ReadUTF8String();

                        if (i == 0)
                        {
                            //表示读取的是字段
                            m_FieldName[j] = str;
                            m_FieldNameDic[str] = j;
                        }
                        else
                        {
                            //表示读取的是内容
                            m_GameData[i - 1, j] = str;
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
            m_CurRowNo++;
        }

        /// <summary>
        /// 获取字段值
        /// </summary>
        /// <returns></returns>
        public string GetFieldValue(string fieldName)
        {
            try
            {
                if (m_CurRowNo < 0 || m_CurRowNo >= row)
                {
                    return null;
                }

                return m_GameData[m_CurRowNo, m_FieldNameDic[fieldName]];
            }
            catch 
            {
                return null; 
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            m_FieldNameDic.Clear();
            m_FieldNameDic = null;

            m_FieldName = null;
            m_GameData = null;
        }

        /// <summary>
        /// 字段名称
        /// </summary>
        private string[] m_FieldName;

        /// <summary>
        /// 游戏数据
        /// </summary>
        private string[,] m_GameData;

        /// <summary>
        /// 当前行号
        /// </summary>
        private int m_CurRowNo = 0;

        /// <summary>
        /// 字段名称字典
        /// </summary>
        private Dictionary<string, int> m_FieldNameDic;
    }
}