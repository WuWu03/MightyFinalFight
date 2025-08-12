//using System;
//using ProtoBuf;
//using System.IO;
//using System.Buffers;

//namespace GameFrameWork.Net
//{
//    public class ProtoBufUtil
//    {
//        public static byte[] ObjectToBytes<T>(T instance)
//        {
//            try
//            {
//                byte[] array;
//                if (instance == null)
//                {
//                    array = new byte[0];
//                }
//                else
//                {
//                    MemoryStream memoryStream = new();
//                    Serializer.Serialize(memoryStream, instance);
//                    array = new byte[memoryStream.Length];
//                    memoryStream.Position = 0L;
//                    memoryStream.Read(array, 0, array.Length);
//                    memoryStream.Dispose();
//                    ArrayPool<byte>.Shared.Return(array);
//                }

//                return array;

//            }
//            catch
//            {
//                return new byte[0];
//            }
//        }

//        public static T BytesToObject<T>(byte[] bytesData, int offset, int length)
//        {
//            if (bytesData.Length == 0)
//            {
//                return default;
//            }
//            try
//            {
//                MemoryStream memoryStream = new MemoryStream();
//                memoryStream.Write(bytesData, 0, bytesData.Length);
//                memoryStream.Position = 0L;
//                T result = Serializer.Deserialize<T>(memoryStream);
//                memoryStream.Dispose();
//                return result;
//            }
//            catch
//            {
//                return default;
//            }
//        }
//    }
//}