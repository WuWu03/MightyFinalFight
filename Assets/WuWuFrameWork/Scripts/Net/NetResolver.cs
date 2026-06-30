using System;
using WuWuFramework.Event;
using WuWuFramework.Pool;
using WuWuFramework.Serialize;
using ProtoBuf;

namespace WuWuFramework.Net
{
    public abstract class NetResolver : IDisposable
    {
        private INetMgr m_NetMgr;

        public NetResolver(INetMgr netMgr)
        {
            m_NetMgr = netMgr;
        }

        protected void AddReceiveEvent(ushort msgCode, WuWuFrameworkAction<ushort, byte[]> receiveEvent)
        {
            m_NetMgr.AddReceiveEvent(msgCode, receiveEvent);
        }

        protected void Send<P>(ushort msgCode, P proto) where P : IExtensible, new()
        {
            using MemoryStreamEx mse = new();
            Serializer.Serialize(mse, proto);
            byte[] buffer = ArrayPool<byte>.instance.Get((int)mse.Length + 6);
            mse.WriteInt((int)mse.Length);
            mse.WriteUShort(msgCode);
            mse.Position = buffer.Length - 6;
            mse.Read(buffer, 0, 6);
            mse.Position = 0;
            mse.Read(buffer, 6, buffer.Length - 6);
            m_NetMgr.Send(buffer);
            ArrayPool<byte>.instance.Put(buffer);
        }

        protected O Deserialize<O>(byte[] buffer) where O : IExtensible, new()
        {
            using MemoryStreamEx mse = new(buffer);
            O result = Serializer.Deserialize<O>(mse);
            return result;
        }

        public abstract void Dispose();
    }
}