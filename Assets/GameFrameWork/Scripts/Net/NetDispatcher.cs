using System;
using System.Collections.Generic;

namespace GameFrameWork.Net
{
    public class NetDispatcher : IDisposable
    {
        public NetDispatcher()
        {
            m_SendEvents = new();
        }

        public void Add(ushort msgCode, GameFrameWorkAction<ushort, byte[]> receiveCall)
        {
            m_SendEvents.Add(msgCode, receiveCall);
        }

        public void Dispatch(ushort msgCode, byte[] msgContent)
        {
            if (m_SendEvents.TryGetValue(msgCode, out GameFrameWorkAction<ushort, byte[]> receiveCall))
            {
                receiveCall?.Invoke(msgCode, msgContent);
                return;
            }

            Log.LogError("网络消息 [", msgCode.ToString(), "] 不存在");
        }

        public void Dispose()
        {
            m_SendEvents.Clear();
        }

        private Dictionary<ushort, GameFrameWorkAction<ushort, byte[]>> m_SendEvents = null;
    }
}
