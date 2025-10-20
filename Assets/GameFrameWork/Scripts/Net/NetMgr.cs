using GameFrameWork.Pool;
using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using GameFrameWork.Event;

namespace GameFrameWork.Net
{
    public class NetMgr : GameFrameWorkModule , INetMgr
    {
        private event GameFrameWorkAction m_OnConnectSuccessEvent;
        private event GameFrameWorkAction m_OnConnectFailEvent;
        private event GameFrameWorkAction m_OnDisConnectEvent;
        private readonly NetDispatcher m_NetDispatcher;
        private readonly Queue<byte[]> m_SendQueue;
        private readonly Queue<byte[]> m_ReceiveQueue;
        private readonly byte[] m_ReceiveBuffer;
        private MemoryStreamEx m_ReceiveMse;
        private Socket m_Socket;
        private string m_IP;
        private int m_Port;
        private bool m_IsConnected;
        private int m_CheckCount;
        
        public NetMgr()
        {
            m_ReceiveBuffer = new byte[1024 * 512];
            m_SendQueue = new();
            m_ReceiveQueue = new();
            m_NetDispatcher = new();
        }
        
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
        
        public override void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            if (m_IsConnected)
            {
                CheckReceiveBuffer();
            }
        }

        public override void Shutdown()
        {
            Close();
            m_NetDispatcher.Dispose();
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
                m_ReceiveMse = ReferencePool.Acquire<MemoryStreamEx>();
                m_IsConnected = true;
                StartReceive();
                m_OnConnectSuccessEvent?.Invoke();
            }
            catch (Exception e)
            {
                m_OnConnectFailEvent?.Invoke();
                throw new GameFrameWorkException(e.Message);
            }
        }

        public void Close()
        {
            if (!m_IsConnected)
            {
                return;
            }

            m_IsConnected = false;
            m_CheckCount = 0;
            m_IP = string.Empty;
            m_Port = 0;
            
            try
            {
                if (m_Socket.Connected)
                {
                    m_Socket.Shutdown(SocketShutdown.Both);
                }
                
                m_Socket.Close();
                
                lock (m_SendQueue)
                {
                    m_SendQueue.Clear();
                }
                
                m_ReceiveQueue.Clear();
                m_ReceiveMse.SetLength(0);
                m_ReceiveMse.Close();
                m_ReceiveMse.Release();
                m_ReceiveMse = null;
                m_OnDisConnectEvent?.Invoke();
            }
            catch (Exception e)
            {
                throw new GameFrameWorkException(e.Message);
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
            m_NetDispatcher.Add(msgCode, receiveCall);
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
                    Close();
                    return;
                }

                m_ReceiveMse.Position = m_ReceiveMse.Length;
                m_ReceiveMse.Write(m_ReceiveBuffer, 0, length);

                if (m_ReceiveMse.Length < 3)
                {
                    StartReceive();
                    return;
                }

                while (true)
                {
                    m_ReceiveMse.Position = 0;
                    int msgLen = m_ReceiveMse.ReadInt() + 2;
                    int fullLen = 4 + msgLen;

                    if (m_ReceiveMse.Length < fullLen)
                    {
                        break;
                    }

                    byte[] msgBuffer = ArrayPool<byte>.instance.Get(msgLen);
                    m_ReceiveMse.Position = 4;
                    m_ReceiveMse.Read(msgBuffer, 0, msgLen);

                    lock (m_ReceiveQueue)
                    {
                        m_ReceiveQueue.Enqueue(msgBuffer);
                    }

                    int remainLen = (int)m_ReceiveMse.Length - fullLen;

                    if (remainLen < 1)
                    {
                        m_ReceiveMse.Position = 0;
                        m_ReceiveMse.SetLength(0);
                        break;
                    }

                    m_ReceiveMse.Position = fullLen;
                    byte[] remainBuffer = ArrayPool<byte>.instance.Get(remainLen);
                    m_ReceiveMse.Read(remainBuffer, 0, remainLen);
                    m_ReceiveMse.Position = 0;
                    m_ReceiveMse.SetLength(0);
                    m_ReceiveMse.Write(remainBuffer, 0, remainLen);
                    ArrayPool<byte>.instance.Put(remainBuffer);
                }
            }
            catch (Exception e)
            {
                Close();
                throw new GameFrameWorkException(e.Message);
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
                    mse.Read(msgContent, 0, msgContent.Length);
                    mse.Release();
                    m_NetDispatcher.Dispatch(msgCode, msgContent);
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
    }
}