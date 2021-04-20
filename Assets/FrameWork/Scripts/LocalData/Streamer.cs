using System.IO;
using System.Text;
using System;

namespace GameFrameWork.LocalData
{
    /// <summary>
    ///从流中读取或写入一个数据
    /// </summary>
    /// <returns></returns>
    public class Streamer : MemoryStream
    {
        public Streamer() { }
        public Streamer(byte[] buffer) : base(buffer) { }

        /// <summary>
        ///从流中读取一个Long
        /// </summary>
        /// <returns></returns>
        public long ReadLong()
        {
            byte[] arr = new byte[8];
            base.Read(arr, 0, 8);
            return BitConverter.ToInt64(arr, 0);
        }

        /// <summary>
        ///从流中读取一个ULong
        /// </summary>
        /// <returns></returns>
        public ulong ReadULong()
        {
            byte[] arr = new byte[8];
            base.Read(arr, 0, 8);
            return BitConverter.ToUInt64(arr, 8);
        }
        /// <summary>
        ///从流中读取一个Int
        /// </summary>
        /// <returns></returns>
        public int ReadInt()
        {
            byte[] arr = new byte[4];
            base.Read(arr, 0, 4);
            return BitConverter.ToInt32(arr, 0);
        }
        /// <summary>
        ///从流中读取一个UInt
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt()
        {
            byte[] arr = new byte[4];
            base.Read(arr, 0, 4);
            return BitConverter.ToUInt32(arr, 0);
        }
        /// <summary>
        ///从流中读取一个Short
        /// </summary>
        /// <returns></returns>
        public short ReadShort()
        {
            byte[] arr = new byte[2];
            base.Read(arr, 0, 2);
            return BitConverter.ToInt16(arr, 0);
        }
        /// <summary>
        ///从流中读取一个UShort
        /// </summary>
        /// <returns></returns>
        public ushort ReadUShort()
        {
            byte[] arr = new byte[2];
            base.Read(arr, 0, 2);
            return BitConverter.ToUInt16(arr, 0);
        }

        /// <summary>
        ///从流中读取一个float
        /// </summary>
        /// <returns></returns>
        public float ReadSingle()
        {
            byte[] arr = new byte[4];
            base.Read(arr, 0, 4);
            return BitConverter.ToSingle(arr, 0);
        }

        /// <summary>
        ///从流中读取一个double
        /// </summary>
        /// <returns></returns>
        public double ReadDouble()
        {
            byte[] arr = new byte[8];
            base.Read(arr, 0, 8);
            return BitConverter.ToDouble(arr, 0);

        }

        /// <summary>
        ///从流中读取一个bool
        /// </summary>
        /// <returns></returns>
        public bool ReadBoolean()
        {
            return base.ReadByte() == 1;
        }

        /// <summary>
        ///从流中读取一个string
        /// </summary>
        /// <returns></returns>
        public string ReadUTF8String()
        {
            ushort lenth = ReadUShort();
            byte[] arr = new byte[lenth];
            base.Read(arr, 0, lenth);
            return Encoding.UTF8.GetString(arr);
        }

        /// <summary>
        ///向流中写入一个short
        /// </summary>
        /// <returns></returns>
        public void WriteShort(short value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        /// <summary>
        ///向流中写入一个UShort
        /// </summary>
        /// <returns></returns>
        public void WriteUShort(ushort value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        /// <summary>
        ///向流中写入一个Int
        /// </summary>
        /// <returns></returns>
        public void WriteInt(int value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        /// <summary>
        ///向流中写入一个UInt
        /// </summary>
        /// <returns></returns>
        public void WriteUInt(uint value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        /// <summary>
        ///向流中写入一个Long
        /// </summary>
        /// <returns></returns>
        public void WriteLong(long value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        /// <summary>
        ///向流中写入一个ULong
        /// </summary>
        /// <returns></returns>
        public void WriteULong(ulong value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        /// <summary>
        ///向流中写入一个float
        /// </summary>
        /// <returns></returns>
        public void WriteSingle(float value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        /// <summary>
        ///向流中写入一个double
        /// </summary>
        /// <returns></returns>
        public void WriteDouble(double vaule)
        {
            byte[] arr = BitConverter.GetBytes(vaule);
            base.Write(arr, 0, arr.Length);
        }

        /// <summary>
        ///向流中写入一个bool
        /// </summary>
        /// <returns></returns>
        public void WriteBoolean(bool value)
        {
            base.WriteByte((byte)(value == true ? 1 : 0));
        }

        /// <summary>
        ///向流中写入一个string
        /// </summary>
        /// <returns></returns>
        public void WriteUTF8String(string str)
        {
            byte[] arr = Encoding.UTF8.GetBytes(str);
            if (arr.Length > 65535)
            {
                throw new InvalidCastException("字符串过大！");
            }
            WriteUShort((ushort)arr.Length);
            base.Write(arr, 0, arr.Length);
        }
    }
}