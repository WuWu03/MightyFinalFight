using System;
using System.Collections.Generic;
using GameFrameWork.Event;
using GameFrameWork.Utils;

namespace GameFrameWork.Net
{
    public class NetDispatcher : IDisposable
    {
        private readonly Dictionary<ushort, GameFrameWorkAction<ushort, byte[]>> m_SendEvents;
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
            if (!m_SendEvents.TryGetValue(msgCode, out GameFrameWorkAction<ushort, byte[]> receiveCall))
            {
                throw new GameFrameWorkException(StringUtil.Append("网络消息 [", msgCode.ToString(), "] 不存在"));
            }

            receiveCall?.Invoke(msgCode, msgContent);
        }

        public void Dispose()
        {
            m_SendEvents.Clear();
        }
    }
}
