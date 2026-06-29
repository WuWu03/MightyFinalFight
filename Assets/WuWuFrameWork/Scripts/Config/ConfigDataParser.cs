using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using WuWuFramework.Serialize;

namespace WuWuFramework.ConfigData
{
    public class ConfigDataParser : IDisposable
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

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigDataParser(byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            byte[] buffer = ZlibHelper.DeCompressBytes(bytes);//解压缩
            m_MSE = new MemoryStreamEx();
            m_MSE.Write(buffer, 0, buffer.Length);
            m_MSE.Position = 0;
            row = m_MSE.ReadInt();
            column = m_MSE.ReadInt();
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

        public byte ReadByte()
        {
            return (byte)m_MSE.ReadByte();
        }

        public byte[] ReadByteArray()
        {
            ushort length = m_MSE.ReadUShort();
            byte[] array = new byte[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = (byte)m_MSE.ReadByte();
            }

            return array;
        }

        public short ReadShort()
        {
            return m_MSE.ReadShort();
        }

        public ushort ReadUShort()
        {
            return m_MSE.ReadUShort();
        }

        public short[] ReadShortArray()
        {
            ushort length = m_MSE.ReadUShort();
            short[] array = new short[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = m_MSE.ReadShort();
            }

            return array;
        }

        public int ReadInt()
        {
            return m_MSE.ReadInt();
        }

        public int[] ReadIntArray()
        {
            ushort length = m_MSE.ReadUShort();
            int[] array = new int[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = m_MSE.ReadInt();
            }

            return array;
        }

        public long ReadLong()
        {
            return m_MSE.ReadLong();
        }

        public long[] ReadLongArray()
        {
            ushort length = m_MSE.ReadUShort();
            long[] array = new long[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = m_MSE.ReadLong();
            }

            return array;
        }

        public float ReadFloat()
        {
            return m_MSE.ReadFloat();
        }

        public float[] ReadFloatArray()
        {
            ushort length = m_MSE.ReadUShort();
            float[] array = new float[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = m_MSE.ReadFloat();
            }

            return array;
        }

        public double ReadDouble()
        {
            return m_MSE.ReadDouble();
        }

        public double[] ReadDoubleArray()
        {
            ushort length = m_MSE.ReadUShort();
            double[] array = new double[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = m_MSE.ReadDouble();
            }

            return array;
        }

        public bool ReadBool()
        {
            return m_MSE.ReadBool();
        }

        public bool[] ReadBoolArray()
        {
            ushort length = m_MSE.ReadUShort();
            bool[] array = new bool[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = m_MSE.ReadBool();
            }

            return array;
        }

        public string ReadUTF8String()
        {
            return m_MSE.ReadUTF8String();
        }

        public string[] ReadUTF8StringArray()
        {
            ushort length = m_MSE.ReadUShort();
            string[] array = new string[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = m_MSE.ReadUTF8String();
            }

            return array;
        }

        public Vector2 ReadVector2()
        {
            Vector2 vector2 = new(m_MSE.ReadFloat(), m_MSE.ReadFloat());
            return vector2;
        }

        public Vector2[] ReadVector2Array()
        {
            ushort length = m_MSE.ReadUShort();
            Vector2[] array = new Vector2[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = new(m_MSE.ReadFloat(), m_MSE.ReadFloat());
            }

            return array;
        }

        public Vector3 ReadVector3()
        {
            Vector3 vector3 = new(m_MSE.ReadFloat(), m_MSE.ReadFloat(), m_MSE.ReadFloat());
            return vector3;
        }

        public Vector3[] ReadVector3Array()
        {
            ushort length = m_MSE.ReadUShort();
            Vector3[] array = new Vector3[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = new(m_MSE.ReadFloat(), m_MSE.ReadFloat(), m_MSE.ReadFloat());
            }

            return array;
        }

        public Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>()
        {
            ushort length = m_MSE.ReadUShort();
            Dictionary<TKey, TValue> dictionary = new();

            for (int i = 0; i < length; i++)
            {
                TKey key = this.Read<TKey>();
                TValue value = this.Read<TValue>();
                dictionary.Add(key, value);
            }

            return dictionary;

        }

        /// <summary>
        /// 泛型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="WuWuFrameworkException"></exception>
        private T Read<T>()
        {
            Type type = typeof(T);

            if (type == typeof(byte))
            {
                byte value = this.ReadByte();
                return Unsafe.As<byte, T>(ref value);
            }
            else if (type == typeof(short))
            {
                short value = this.ReadShort();
                return Unsafe.As<short, T>(ref value);
            }
            else if (type == typeof(int))
            {
                int value = this.ReadInt();
                return Unsafe.As<int, T>(ref value);
            }
            else if (type == typeof(long))
            {
                long value = this.ReadLong();
                return Unsafe.As<long, T>(ref value);
            }
            else if (type == typeof(float))
            {
                float value = this.ReadFloat();
                return Unsafe.As<float, T>(ref value);
            }
            else if (type == typeof(double))
            {
                double value = this.ReadDouble();
                return Unsafe.As<double, T>(ref value);
            }
            else if (type == typeof(bool))
            {
                bool value = this.ReadBool();
                return Unsafe.As<bool, T>(ref value);
            }
            else if (type == typeof(string))
            {
                string value = this.ReadUTF8String();
                return Unsafe.As<string, T>(ref value);
            }
            else if (type == typeof(byte[]))
            {
                byte[] values = this.ReadByteArray();
                return Unsafe.As<byte[], T>(ref values);
            }
            else if (type == typeof(short[]))
            {
                short[] values = this.ReadShortArray();
                return Unsafe.As<short[], T>(ref values);
            }
            else if (type == typeof(int[]))
            {
                int[] values = this.ReadIntArray();
                return Unsafe.As<int[], T>(ref values);
            }
            else if (type == typeof(long[]))
            {
                long[] values = this.ReadLongArray();
                return Unsafe.As<long[], T>(ref values);
            }
            else if (type == typeof(float[]))
            {
                float[] values = this.ReadFloatArray();
                return Unsafe.As<float[], T>(ref values);
            }
            else if (type == typeof(double[]))
            {
                double[] values = this.ReadDoubleArray();
                return Unsafe.As<double[], T>(ref values);
            }
            else if (type == typeof(bool[]))
            {
                bool[] values = this.ReadBoolArray();
                return Unsafe.As<bool[], T>(ref values);
            }
            else if (type == typeof(string[]))
            {
                string[] values = this.ReadUTF8StringArray();
                return Unsafe.As<string[], T>(ref values);
            }

            throw new WuWuFrameworkException("未找到类型");
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            m_MSE.Dispose();
        }
    }
}