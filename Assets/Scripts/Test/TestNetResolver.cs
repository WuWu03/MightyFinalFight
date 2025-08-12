using client;
using GameFrameWork;
using GameFrameWork.Net;
using GameFrameWork.Pool;
using GameFrameWork.Serialize;
using ProtoBuf;

public class TestNetResolver : Singleton<TestNetResolver>
{
    public event GameFrameWorkAction<test> testReceiveEvent
    {
        add
        {
            m_TestReceiveEvent = value;
        }
        remove
        {
            m_TestReceiveEvent = null;
        }
    }

    public TestNetResolver()
    {
        m_TestProto = new();
        NetMgr.instance.AddReceiveEvent(1, ReceiveTest);
    }

    public void SendTest(string content)
    {
        MemoryStreamEx mse = ReferencePool.Acquire<MemoryStreamEx>();
        m_TestProto.content = content;
        Serializer.Serialize(mse, m_TestProto);

        byte[] buffer = ArrayPool<byte>.instance.Get((int)mse.Length + 6);

        mse.WriteInt((int)mse.Length);
        mse.WriteUShort(1);
        mse.Position = buffer.Length - 6;
        mse.Read(buffer, 0, 6);
        mse.Position = 0;
        mse.Read(buffer, 6, buffer.Length - 6);
        NetMgr.instance.Send(buffer);
        mse.Release();
        ArrayPool<byte>.instance.Put(buffer);
    }

    public void ReceiveTest(ushort msgId, byte[] buffer)
    {
        MemoryStreamEx mse = ReferencePool.Acquire<MemoryStreamEx>();
        mse.Write(buffer, 0, buffer.Length);
        mse.Position = 0;
        test result = Serializer.Deserialize<test>(mse);
        mse.Release();
        m_TestReceiveEvent?.Invoke(result);
    }

    protected override void OnDispose()
    {
        m_TestReceiveEvent = null;
        m_TestProto = null;
    }

    private GameFrameWorkAction<test> m_TestReceiveEvent = null;
    private test m_TestProto = null;
}
