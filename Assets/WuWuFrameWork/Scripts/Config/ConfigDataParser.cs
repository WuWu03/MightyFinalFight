using System.Collections.Generic;
using System.Runtime.CompilerServices;
using WuWuFramework.Serialize;

namespace WuWuFramework.ConfigData
{
    public class ConfigDataParser : IReference
    {
        private int m_CurrRow = 0;
        private MemoryStreamEx m_MSE = null;

        /// <summary>
        /// 行数
        /// </summary>
        public int row { get; private set; }

        /// <summary>
        /// 列数
        /// </summary>
        public int column { get; private set; }

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

        public void Init(byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            byte[] buffer = ZlibHelper.DeCompressBytes(bytes);//解压缩
            m_MSE = new MemoryStreamEx(buffer);
            row = m_MSE.ReadInt();
            column = m_MSE.ReadInt();
            m_CurrRow = 0;
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
        /// 从Bytes中读取字典
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>()
        {
            return BytesTypeReader.ReadDictionary<TKey, TValue>(m_MSE);
        }

        /// <summary>
        /// 从Bytes中读取基本类型数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="WuWuFrameworkException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read<T>()
        {
            return BytesTypeReader.Read<T>(m_MSE);
        }

        public void Release()
        {
            ReferencePool.Release(this);
        }

        public void Clear()
        {
            m_MSE.Dispose();
            m_MSE = null;
            row = 0;
            column = 0;
            m_CurrRow = 0;
        }
    }
}