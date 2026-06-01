using System;
using System.Collections.Generic;
using WuWuFramework.Event;
using WuWuFramework.Utils;

namespace WuWuFramework.Net
{
    public class NetDispatcher : IDisposable
    {
        private readonly Dictionary<ushort, WuWuFrameworkAction<ushort, byte[]>> m_SendEvents;
        public NetDispatcher()
        {
            m_SendEvents = new();
        }

        public void Add(ushort msgCode, WuWuFrameworkAction<ushort, byte[]> receiveCall)
        {
            m_SendEvents.Add(msgCode, receiveCall);
        }

        public void Dispatch(ushort msgCode, byte[] msgContent)
        {
            if (!m_SendEvents.TryGetValue(msgCode, out WuWuFrameworkAction<ushort, byte[]> receiveCall))
            {
                throw new WuWuFrameworkException(StringUtil.Append("网络消息 [", msgCode.ToString(), "] 不存在"));
            }

            receiveCall?.Invoke(msgCode, msgContent);
        }

        public void Dispose()
        {
            m_SendEvents.Clear();
        }
    }
}
