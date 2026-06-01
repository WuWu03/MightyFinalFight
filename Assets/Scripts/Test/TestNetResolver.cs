using client;
using WuWuFramework.Event;
using WuWuFramework.Net;

public class TestNetResolver : NetResolver
{
    public event WuWuFrameworkAction<test> onReceiveTestEvent;

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
        onReceiveTestEvent?.Invoke(Deserialize<test>(buffer));
    }
    
    public override void Dispose()
    {
        onReceiveTestEvent = null;
    }
}
