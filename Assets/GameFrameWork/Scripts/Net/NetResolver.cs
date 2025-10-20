using System;
using GameFrameWork.Event;
using GameFrameWork.Pool;
using GameFrameWork.Serialize;
using ProtoBuf;

namespace GameFrameWork.Net
{
    public abstract class NetResolver : IDisposable
    {
        private INetMgr m_NetMgr;

        public NetResolver(INetMgr netMgr)
        {
            m_NetMgr = netMgr;
        }

        protected void AddReceiveEvent(ushort msgCode, GameFrameWorkAction<ushort, byte[]> receiveEvent)
        {
            m_NetMgr.AddReceiveEvent(msgCode, receiveEvent);
        }

        protected void Send<P>(ushort msgCode, P proto) where P : IExtensible, new()
        {
            MemoryStreamEx mse = ReferencePool.Acquire<MemoryStreamEx>();
            Serializer.Serialize(mse, proto);
            byte[] buffer = ArrayPool<byte>.instance.Get((int)mse.Length + 6);
            mse.WriteInt((int)mse.Length);
            mse.WriteUShort(msgCode);
            mse.Position = buffer.Length - 6;
            mse.Read(buffer, 0, 6);
            mse.Position = 0;
            mse.Read(buffer, 6, buffer.Length - 6);
            m_NetMgr.Send(buffer);
            mse.Release();
            ArrayPool<byte>.instance.Put(buffer);
        }

        protected P Deserialize<P>(byte[] buffer) where P : IExtensible, new()
        {
            MemoryStreamEx mse = ReferencePool.Acquire<MemoryStreamEx>();
            mse.Write(buffer, 0, buffer.Length);
            mse.Position = 0;
            P result = Serializer.Deserialize<P>(mse);
            mse.Release();
            return result;
        }

        public abstract void Dispose();
    }
}