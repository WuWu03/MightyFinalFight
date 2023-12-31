using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameFrameWork.Net
{
    public class SocketMgr : BaseMgr<SocketMgr>
    {
        public Action<ushort, byte[]> onReceiveEvent = null;
        public Action onConnectSuccessEvent = null;
        public Action onConnectFailEvent = null;
        public Action onDisConnectEvent = null;

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
            m_SendQueue = new Queue<byte[]>();
            m_ReceiveQueue = new Queue<byte[]>();
            m_OnEventCallQueue = new Queue<Action>();
        }

        public void Connect(string ip, int port)
        {
            m_IP = ip;
            m_Port = port;
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                m_Socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                m_ReceiveStream = new MemoryStreamEx();
                m_IsConnected = true;
                StartReceive();
                onConnectSuccessEvent?.Invoke();
                Log.LogInfo("连接服务器:" + ip + "成功！");
            }
            catch (Exception e)
            {
                onConnectFailEvent?.Invoke();
                Log.LogInfo(e.ToString());
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
                m_Socket.Shutdown(SocketShutdown.Both);
            }
            catch 
            {

            }

            m_Socket.Close();
            m_SendQueue.Clear();
            m_ReceiveQueue.Clear();
            m_ReceiveStream.SetLength(0);
            m_ReceiveStream.Close();

            m_Socket = null;
            m_ReceiveStream = null;
            m_OnEventCallQueue.Enqueue(onDisConnectEvent);
        }

        public void Send(ushort msgCode, byte[] buffer)
        {
            if (!m_IsConnected)
            {
                return;
            }

            byte[] sendMsgBuffer = null;

            using (MemoryStreamEx mse = new MemoryStreamEx())
            {
                int msgLen = buffer.Length;
                mse.WriteUShort((ushort)msgLen);
                mse.WriteUShort(msgCode);
                mse.Write(buffer, 0, msgLen);
                sendMsgBuffer = mse.ToArray();
            }

            lock (m_SendQueue)
            {
                m_SendQueue.Enqueue(sendMsgBuffer);
                CheckSendBuffer();
            }
        }

        protected override void OnUpdate()
        {
            if (m_IsConnected)
            {
                CheckReceiveBuffer();
            }

            if (m_OnEventCallQueue.Count > 0)
            {
                Action a = m_OnEventCallQueue.Dequeue();
                a?.Invoke();
            }
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

                m_ReceiveStream.Position = m_ReceiveStream.Length;
                m_ReceiveStream.Write(m_ReceiveBuffer, 0, length);

                if (m_ReceiveStream.Length < 3)
                {
                    StartReceive();
                    return;
                }

                while (true)
                {
                    m_ReceiveStream.Position = 0;
                    int msgLen = m_ReceiveStream.ReadUShort() + 2;
                    int fullLen = 2 + msgLen;

                    if (m_ReceiveStream.Length < fullLen)
                    {
                        break;
                    }

                    byte[] msgBuffer = new byte[msgLen];
                    m_ReceiveStream.Position = 2;
                    m_ReceiveStream.Read(msgBuffer, 0, msgLen);

                    lock (m_ReceiveQueue)
                    {
                        m_ReceiveQueue.Enqueue(msgBuffer);
                    }

                    int remainLen = (int)m_ReceiveStream.Length - fullLen;

                    if (remainLen < 1)
                    {
                        m_ReceiveStream.Position = 0;
                        m_ReceiveStream.SetLength(0);
                        break;
                    }

                    m_ReceiveStream.Position = fullLen;
                    byte[] remainBuffer = new byte[remainLen];
                    m_ReceiveStream.Read(remainBuffer, 0, remainLen);
                    m_ReceiveStream.Position = 0;
                    m_ReceiveStream.SetLength(0);
                    m_ReceiveStream.Write(remainBuffer, 0, remainLen);
                    remainBuffer = null;
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
            while (true)
            {
                if (m_CheckCount > 5)
                {
                    m_CheckCount = 0;
                    break;
                }

                m_CheckCount++;

                lock (m_ReceiveQueue)
                {
                    if (m_ReceiveQueue.Count < 1)
                    {
                        break;
                    }

                    byte[] buffer = m_ReceiveQueue.Dequeue();
                    byte[] msgContent = new byte[buffer.Length - 2];
                    ushort msgCode = 0;

                    using (MemoryStreamEx mse = new MemoryStreamEx(buffer))
                    {
                        msgCode = mse.ReadUShort();
                        mse.Read(msgContent, 0, msgContent.Length);
                    }

                    onReceiveEvent?.Invoke(msgCode, msgContent);
                }
            }
        }

        private void SendCallback(IAsyncResult ir)
        {
            m_Socket.EndSend(ir);
            CheckSendBuffer();
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();

            Close();
            m_SendQueue = null;
            m_ReceiveQueue = null;
            m_ReceiveStream = null;
            m_ReceiveBuffer = null;

            m_OnEventCallQueue.Clear();
            m_OnEventCallQueue = null;
        }

        private Queue<Action> m_OnEventCallQueue = null;
        private Queue<byte[]> m_SendQueue = null;
        private Queue<byte[]> m_ReceiveQueue = null;
        private MemoryStreamEx m_ReceiveStream = null;
        private byte[] m_ReceiveBuffer = null;
        private bool m_IsConnected = false;
        private string m_IP = string.Empty;
        private int m_CheckCount = 0;
        private int m_Port = int.MaxValue;
        private Socket m_Socket = null;
    }
}