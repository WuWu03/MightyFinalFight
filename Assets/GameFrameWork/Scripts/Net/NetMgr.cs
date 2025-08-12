using GameFrameWork.Pool;
using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameFrameWork.Net
{
    public class NetMgr : BaseMgr<NetMgr>
    {
        public event GameFrameWorkAction onConnectSuccessEvent
        {
            add
            {
                m_OnConnectSuccessEvent += value;
            }
            remove
            {
                m_OnConnectSuccessEvent -= value;
            }
        }

        public event GameFrameWorkAction onConnectFailEvent
        {
            add
            {
                m_OnConnectFailEvent += value;
            }
            remove
            {
                m_OnConnectFailEvent -= value;
            }
        }

        public event GameFrameWorkAction onDisConnectEvent
        {
            add
            {
                m_OnDisConnectEvent += value;
            }
            remove
            {
                m_OnDisConnectEvent -= value;
            }
        }

        public bool isConnected
        {
            get
            {
                return m_IsConnected;
            }
        }

        protected override void OnAwake()
        {
            m_ReceiveBuffer = new byte[1024 * 512];
            m_SendQueue = new();
            m_ReceiveQueue = new();
            m_ReceiveEvents = new();
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (m_IsConnected)
            {
                CheckReceiveBuffer();
            }
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();
            Close();

            m_SendQueue.Clear();
            m_ReceiveQueue.Clear();
        }

        protected override void OnDestory()
        {
            base.OnDestory();

            m_IsConnected = false;
            m_IP = string.Empty;
            m_CheckCount = 0;
            m_Port = int.MaxValue;

            m_ReceiveBuffer = null;
            m_Socket = null;
            m_SendQueue = null;
            m_ReceiveQueue = null;
            m_OnConnectSuccessEvent = null;
            m_OnConnectFailEvent = null;
            m_OnDisConnectEvent = null;
        }

        public void Connect(string ip, int port)
        {
            m_IP = ip;
            m_Port = port;
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                m_Socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                m_ReceiveMSE = new();

                m_IsConnected = true;
                StartReceive();
                m_OnConnectSuccessEvent?.Invoke();
                Log.LogInfo("连接服务器:" + ip + "成功！");
            }
            catch (Exception e)
            {
                m_OnConnectFailEvent?.Invoke();
                Log.LogError(e.ToString());
            }
        }

        public void Close()
        {
            if (!m_IsConnected)
            {
                return;
            }

            m_IsConnected = false;

            try
            {
                if (m_Socket.Connected)
                {
                    m_Socket.Shutdown(SocketShutdown.Both);
                }

                m_Socket.Close();
                m_SendQueue.Clear();
                m_ReceiveQueue.Clear();
                m_ReceiveMSE.SetLength(0);
                m_ReceiveMSE.Close();
                m_OnDisConnectEvent?.Invoke();
            }
            catch (Exception e)
            {
                Log.LogError(e.Message);
            }
        }

        public void Send(byte[] buffer)
        {
            if (!m_IsConnected)
            {
                return;
            }

            lock (m_SendQueue)
            {
                m_SendQueue.Enqueue(buffer);
                CheckSendBuffer();
            }
        }

        public void AddReceiveEvent(ushort msgCode, GameFrameWorkAction<ushort, byte[]> receiveCall)
        {
            m_ReceiveEvents.Add(msgCode, receiveCall);
        }

        private void StartReceive()
        {
            if (!m_IsConnected)
            {
                return;
            }

            m_Socket.BeginReceive(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length, SocketFlags.None, OnReceive, m_Socket);
        }

        private void OnReceive(IAsyncResult ir)
        {
            if (!m_IsConnected)
            {
                return;
            }

            try
            {
                int length = m_Socket.EndReceive(ir);

                if (length < 1)
                {
                    Log.LogInfo("服务器断开连接");
                    Close();
                    return;
                }

                m_ReceiveMSE.Position = m_ReceiveMSE.Length;
                m_ReceiveMSE.Write(m_ReceiveBuffer, 0, length);

                if (m_ReceiveMSE.Length < 3)
                {
                    StartReceive();
                    return;
                }

                while (true)
                {
                    m_ReceiveMSE.Position = 0;
                    int msgLen = m_ReceiveMSE.ReadInt() + 2;
                    int fullLen = 4 + msgLen;

                    if (m_ReceiveMSE.Length < fullLen)
                    {
                        break;
                    }

                    byte[] msgBuffer = ArrayPool<byte>.instance.Get(msgLen);
                    m_ReceiveMSE.Position = 4;
                    m_ReceiveMSE.Read(msgBuffer, 0, msgLen);

                    lock (m_ReceiveQueue)
                    {
                        m_ReceiveQueue.Enqueue(msgBuffer);
                    }

                    int remainLen = (int)m_ReceiveMSE.Length - fullLen;

                    if (remainLen < 1)
                    {
                        m_ReceiveMSE.Position = 0;
                        m_ReceiveMSE.SetLength(0);
                        break;
                    }

                    m_ReceiveMSE.Position = fullLen;
                    byte[] remainBuffer = ArrayPool<byte>.instance.Get(remainLen);
                    m_ReceiveMSE.Read(remainBuffer, 0, remainLen);
                    m_ReceiveMSE.Position = 0;
                    m_ReceiveMSE.SetLength(0);
                    m_ReceiveMSE.Write(remainBuffer, 0, remainLen);
                    ArrayPool<byte>.instance.Put(remainBuffer);
                }
            }
            catch (Exception e)
            {
                Log.LogInfo("++服务器断开连接," + e.Message);
                Close();
                return;
            }

            StartReceive();
        }

        private void CheckSendBuffer()
        {
            lock (m_SendQueue)
            {
                if (m_SendQueue.Count > 0)
                {
                    byte[] buffer = m_SendQueue.Dequeue();
                    m_Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, m_Socket);
                }
            }
        }

        private void CheckReceiveBuffer()
        {
            if (m_ReceiveQueue.Count < 1)
            {
                m_CheckCount = 0;
                return;
            }

            lock (m_ReceiveQueue)
            {
                while (true)
                {
                    if (m_CheckCount > 5 || m_ReceiveQueue.Count < 1)
                    {
                        m_CheckCount = 0;
                        break;
                    }

                    m_CheckCount++;

                    MemoryStreamEx mse = ReferencePool.Acquire<MemoryStreamEx>();
                    byte[] buffer = m_ReceiveQueue.Dequeue();
                    byte[] msgContent = ArrayPool<byte>.instance.Get(buffer.Length - 2);
                    mse.Write(buffer, 0, buffer.Length);
                    mse.Position = 0;
                    ushort msgCode = mse.ReadUShort();

                    if(!m_ReceiveEvents.TryGetValue(msgCode,out var receiveCall))
                    {
                        Log.LogError("网络消息 [", msgCode.ToString(), "] 不存在");
                        break;
                    }

                    mse.Read(msgContent, 0, msgContent.Length);
                    receiveCall?.Invoke(msgCode, msgContent);
                    ArrayPool<byte>.instance.Put(msgContent);
                    ArrayPool<byte>.instance.Put(buffer);
                }
            }
        }

        private void SendCallback(IAsyncResult ir)
        {
            m_Socket.EndSend(ir);
            CheckSendBuffer();
        }

        private bool m_IsConnected = false;
        private string m_IP = string.Empty;
        private int m_CheckCount = 0;
        private int m_Port = int.MaxValue;

        private byte[] m_ReceiveBuffer = null;
        private Queue<byte[]> m_SendQueue = null;
        private Queue<byte[]> m_ReceiveQueue = null;
        private MemoryStreamEx m_ReceiveMSE = null;
        private Socket m_Socket = null;
        private Dictionary<ushort, GameFrameWorkAction<ushort, byte[]>> m_ReceiveEvents = null;
        private event GameFrameWorkAction m_OnConnectSuccessEvent = null;
        private event GameFrameWorkAction m_OnConnectFailEvent = null;
        private event GameFrameWorkAction m_OnDisConnectEvent = null;
    }
}