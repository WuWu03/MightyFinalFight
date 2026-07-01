using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using WuWuFramework.Event;

namespace WuWuFramework.Serialize
{
    public static class BytesTypeReader
    {
        private static readonly Dictionary<Type, Delegate> s_Readers = new(20);

        static BytesTypeReader()
        {
            s_Readers.Add(typeof(byte), new WuWuFrameworkFunc<MemoryStreamEx, byte>(ReadByte));
            s_Readers.Add(typeof(short), new WuWuFrameworkFunc<MemoryStreamEx, short>(ReadShort));
            s_Readers.Add(typeof(int), new WuWuFrameworkFunc<MemoryStreamEx, int>(ReadInt));
            s_Readers.Add(typeof(long), new WuWuFrameworkFunc<MemoryStreamEx, long>(ReadLong));
            s_Readers.Add(typeof(float), new WuWuFrameworkFunc<MemoryStreamEx, float>(ReadFloat));
            s_Readers.Add(typeof(double), new WuWuFrameworkFunc<MemoryStreamEx, double>(ReadDouble));
            s_Readers.Add(typeof(bool), new WuWuFrameworkFunc<MemoryStreamEx, bool>(ReadBool));
            s_Readers.Add(typeof(string), new WuWuFrameworkFunc<MemoryStreamEx, string>(ReadUTF8String));
            s_Readers.Add(typeof(Vector2), new WuWuFrameworkFunc<MemoryStreamEx, Vector2>(ReadVector2));
            s_Readers.Add(typeof(Vector3), new WuWuFrameworkFunc<MemoryStreamEx, Vector3>(ReadVector3));
            s_Readers.Add(typeof(byte[]), new WuWuFrameworkFunc<MemoryStreamEx, byte[]>(ReadByteArray));
            s_Readers.Add(typeof(short[]), new WuWuFrameworkFunc<MemoryStreamEx, short[]>(ReadShortArray));
            s_Readers.Add(typeof(int[]), new WuWuFrameworkFunc<MemoryStreamEx, int[]>(ReadIntArray));
            s_Readers.Add(typeof(long[]), new WuWuFrameworkFunc<MemoryStreamEx, long[]>(ReadLongArray));
            s_Readers.Add(typeof(float[]), new WuWuFrameworkFunc<MemoryStreamEx, float[]>(ReadFloatArray));
            s_Readers.Add(typeof(double[]), new WuWuFrameworkFunc<MemoryStreamEx, double[]>(ReadDoubleArray));
            s_Readers.Add(typeof(bool[]), new WuWuFrameworkFunc<MemoryStreamEx, bool[]>(ReadBoolArray));
            s_Readers.Add(typeof(string[]), new WuWuFrameworkFunc<MemoryStreamEx, string[]>(ReadUTF8StringArray));
            s_Readers.Add(typeof(Vector2[]), new WuWuFrameworkFunc<MemoryStreamEx, Vector2[]>(ReadVector2Array));
            s_Readers.Add(typeof(Vector3[]), new WuWuFrameworkFunc<MemoryStreamEx, Vector3[]>(ReadVector3Array));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(MemoryStreamEx mse)
        {
            if (s_Readers.TryGetValue(typeof(T), out Delegate tempReader))
            {
                WuWuFrameworkFunc<MemoryStreamEx, T> reader = (WuWuFrameworkFunc<MemoryStreamEx, T>)tempReader;
                return reader(mse);
            }

            throw new WuWuFrameworkException("未找到类型");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(MemoryStreamEx mse)
        {
            ushort length = mse.ReadUShort();
            Dictionary<TKey, TValue> dictionary = new();

            for (int i = 0; i < length; i++)
            {
                TKey key = Read<TKey>(mse);
                TValue value = Read<TValue>(mse);
                dictionary.Add(key, value);
            }

            return dictionary;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ReadByte(MemoryStreamEx mse)
        {
            return (byte)mse.ReadByte();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] ReadByteArray(MemoryStreamEx mse)
        {
            ushort length = mse.ReadUShort();
            byte[] array = new byte[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = (byte)mse.ReadByte();
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short ReadShort(MemoryStreamEx mse)
        {
            return mse.ReadShort();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short[] ReadShortArray(MemoryStreamEx mse)
        {
            ushort length = mse.ReadUShort();
            short[] array = new short[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = mse.ReadShort();
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ReadInt(MemoryStreamEx mse)
        {
            return mse.ReadInt();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int[] ReadIntArray(MemoryStreamEx mse)
        {
            ushort length = mse.ReadUShort();
            int[] array = new int[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = mse.ReadInt();
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ReadLong(MemoryStreamEx mse)
        {
            return mse.ReadLong();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long[] ReadLongArray(MemoryStreamEx mse)
        {
            ushort length = mse.ReadUShort();
            long[] array = new long[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = mse.ReadLong();
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ReadFloat(MemoryStreamEx mse)
        {
            return mse.ReadFloat();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float[] ReadFloatArray(MemoryStreamEx mse)
        {
            ushort length = mse.ReadUShort();
            float[] array = new float[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = mse.ReadFloat();
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ReadDouble(MemoryStreamEx mse)
        {
            return mse.ReadDouble();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double[] ReadDoubleArray(MemoryStreamEx mse)
        {
            ushort length = mse.ReadUShort();
            double[] array = new double[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = mse.ReadDouble();
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ReadBool(MemoryStreamEx mse)
        {
            return mse.ReadBool();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool[] ReadBoolArray(MemoryStreamEx mse)
        {
            ushort length = mse.ReadUShort();
            bool[] array = new bool[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = mse.ReadBool();
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ReadUTF8String(MemoryStreamEx mse)
        {
            return mse.ReadUTF8String();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string[] ReadUTF8StringArray(MemoryStreamEx mse)
        {
            ushort length = mse.ReadUShort();
            string[] array = new string[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = mse.ReadUTF8String();
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2 ReadVector2(MemoryStreamEx mse)
        {
            Vector2 vector2 = new(mse.ReadFloat(), mse.ReadFloat());
            return vector2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2[] ReadVector2Array(MemoryStreamEx mse)
        {
            ushort length = mse.ReadUShort();
            Vector2[] array = new Vector2[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = new(mse.ReadFloat(), mse.ReadFloat());
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 ReadVector3(MemoryStreamEx mse)
        {
            Vector3 vector3 = new(mse.ReadFloat(), mse.ReadFloat(), mse.ReadFloat());
            return vector3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3[] ReadVector3Array(MemoryStreamEx mse)
        {
            ushort length = mse.ReadUShort();
            Vector3[] array = new Vector3[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = new(mse.ReadFloat(), mse.ReadFloat(), mse.ReadFloat());
            }

            return array;
        }
    }
}