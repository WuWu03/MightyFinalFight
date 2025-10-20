using GameFrameWork.Event;

namespace GameFrameWork.Net
{
    public interface INetMgr
    {
        public event GameFrameWorkAction onConnectSuccessEvent;
        public event GameFrameWorkAction onConnectFailEvent;
        public event GameFrameWorkAction onDisConnectEvent;
        public bool isConnected { get; }
        public void Connect(string ip, int port);
        public void Close();
        public void Send(byte[] buffer);
        public void AddReceiveEvent(ushort msgCode, GameFrameWorkAction<ushort, byte[]> receiveCall);
    }
}