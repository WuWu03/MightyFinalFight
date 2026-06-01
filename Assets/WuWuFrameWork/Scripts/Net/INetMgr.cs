using WuWuFramework.Event;

namespace WuWuFramework.Net
{
    public interface INetMgr
    {
        public event WuWuFrameworkAction onConnectSuccessEvent;
        public event WuWuFrameworkAction onConnectFailEvent;
        public event WuWuFrameworkAction onDisConnectEvent;
        public bool isConnected { get; }
        public void Connect(string ip, int port);
        public void Close();
        public void Send(byte[] buffer);
        public void AddReceiveEvent(ushort msgCode, WuWuFrameworkAction<ushort, byte[]> receiveCall);
    }
}