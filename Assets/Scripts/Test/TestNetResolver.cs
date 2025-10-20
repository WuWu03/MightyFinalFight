using client;
using GameFrameWork.Event;
using GameFrameWork.Net;

public class TestNetResolver : NetResolver
{
    public event GameFrameWorkAction<test> onReceiveTestEvent;

    public TestNetResolver(INetMgr netMgr) : base(netMgr)
    {
        AddReceiveEvent(1, ReceiveTest);
    }

    public void SendTest(string content)
    {
        test proto = new()
        {
            content = content
        };

        Send(1, proto);
    }

    private void ReceiveTest(ushort msgCode, byte[] buffer)
    {
        test proto = Deserialize<test>(buffer);
        onReceiveTestEvent?.Invoke(proto);
    }
    
    public override void Dispose()
    {
        onReceiveTestEvent = null;
    }
}
