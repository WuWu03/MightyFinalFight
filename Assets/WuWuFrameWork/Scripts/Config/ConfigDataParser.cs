using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T Read<T>()
        {
            Type type = typeof(T);

            if (type == typeof(byte))
            {
                byte value = this.ReadByte();
                return Unsafe.As<byte, T>(ref value);
            }
            if (type == typeof(short))
            {
                short value = this.ReadShort();
                return Unsafe.As<short, T>(ref value);
            }
            if (type == typeof(int))
            {
                int value = this.ReadInt();
                return Unsafe.As<int, T>(ref value);
            }
            if (type == typeof(long))
            {
                long value = this.ReadLong();
                return Unsafe.As<long, T>(ref value);
            }
            if (type == typeof(float))
            {
                float value = this.ReadFloat();
                return Unsafe.As<float, T>(ref value);
            }
            if (type == typeof(double))
            {
                double value = this.ReadDouble();
                return Unsafe.As<double, T>(ref value);
            }
            if (type == typeof(bool))
            {
                bool value = this.ReadBool();
                return Unsafe.As<bool, T>(ref value);
            }
            if (type == typeof(Vector2))
            {
                Vector2 value = this.ReadVector2();
                return Unsafe.As<Vector2, T>(ref value);
            }
            if (type == typeof(Vector3))
            {
                Vector3 value = this.ReadVector3();
                return Unsafe.As<Vector3, T>(ref value);
            }

            //引用类型无需Unsafe，直接强转
            if (type == typeof(string)) return (T)(object)this.ReadUTF8String();
            if (type == typeof(byte[])) return (T)(object)this.ReadByteArray();
            if (type == typeof(short[])) return (T)(object)this.ReadShortArray();
            if (type == typeof(int[])) return (T)(object)this.ReadIntArray();
            if (type == typeof(long[])) return (T)(object)this.ReadLongArray();
            if (type == typeof(float[])) return (T)(object)this.ReadFloatArray();
            if (type == typeof(double[])) return (T)(object)this.ReadDoubleArray();
            if (type == typeof(bool[])) return (T)(object)this.ReadBoolArray();
            if (type == typeof(string[])) return (T)(object)this.ReadUTF8StringArray();
            if (type == typeof(Vector2[])) return (T)(object)this.ReadVector2Array();
            if (type == typeof(Vector3[])) return (T)(object)this.ReadVector3Array();

            throw new WuWuFrameworkException("未找到类型");
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