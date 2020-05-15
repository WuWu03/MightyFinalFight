using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SocketMgr : MonoBehaviour
{
    public static SocketMgr Instance = null;
    public Action<ushort, byte[]> onReceive = null;
    public Action OnConnectSuccess = null;
    public Action OnConnectFail = null;
    public Action OnDisConnect = null;
    public bool IsConnected
    {
        get
        {
            return m_IsConnected;
        }
    }

    private void Awake()
    {
        Instance = this;
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
            m_ReceiveStream = new MemoryStream();
            m_IsConnected = true;
            StartReceive();

            if (OnConnectSuccess != null)
            {
                OnConnectSuccess();
            }

            Debug.Log("连接服务器:" + ip + "成功！");
        }
        catch (Exception e)
        {
            if (OnConnectFail != null)
            {
                OnConnectFail();
            }

            Debug.Log(e.ToString());
        }
    }

    public void Close()
    {
        if (!m_IsConnected) return;

        m_IsConnected = false;

        try { m_Socket.Shutdown(SocketShutdown.Both); }
        catch { }

        m_Socket.Close();
        m_SendQueue.Clear();
        m_ReceiveQueue.Clear();
        m_ReceiveStream.SetLength(0);
        m_ReceiveStream.Close();

        m_Socket = null;
        m_ReceiveStream = null;
        m_OnEventCallQueue.Enqueue(OnDisConnect);
    }

    public void Send(ushort msgCode, byte[] buffer)
    {
        if (!m_IsConnected) return;
        byte[] sendMsgBuffer = null;

        using (MemoryStream ms = new MemoryStream())
        {
            int msgLen = buffer.Length;
            byte[] lenBuffer = BitConverter.GetBytes((ushort)msgLen);
            byte[] msgCodeBuffer = BitConverter.GetBytes(msgCode);
            ms.Write(lenBuffer, 0, lenBuffer.Length);
            ms.Write(msgCodeBuffer, 0, msgCodeBuffer.Length);
            ms.Write(buffer, 0, msgLen);
            sendMsgBuffer = ms.ToArray();
        }

        lock (m_SendQueue)
        {
            m_SendQueue.Enqueue(sendMsgBuffer);
            CheckSendBuffer();
        }
    }

    private void Update()
    {
        if (m_IsConnected)
            CheckReceiveBuffer();

        if(m_OnEventCallQueue.Count > 0)
        {
            Action a = m_OnEventCallQueue.Dequeue();
            if (a != null) a();
        }
    }

    private void StartReceive()
    {
        if (!m_IsConnected) return;
        m_Socket.BeginReceive(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length, SocketFlags.None, OnReceive, m_Socket);
    }

    private void OnReceive(IAsyncResult ir)
    {
        if (!m_IsConnected) return;
        try
        {
            int length = m_Socket.EndReceive(ir);

            if (length < 1)
            {
                Debug.Log("服务器断开连接");
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
                byte[] msgLenBuffer = new byte[2];
                m_ReceiveStream.Read(msgLenBuffer, 0, 2);
                int msgLen = BitConverter.ToUInt16(msgLenBuffer, 0) + 2;
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
        catch(Exception e)
        {
            Debug.Log("++服务器断开连接," + e.Message);
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

                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    byte[] msgCodeBuffer = new byte[2];
                    ms.Read(msgCodeBuffer, 0, msgCodeBuffer.Length);
                    msgCode = BitConverter.ToUInt16(msgCodeBuffer, 0);
                    ms.Read(msgContent, 0, msgContent.Length);
                }

                if (onReceive != null)
                {
                    onReceive(msgCode, msgContent);
                }
            }
        }
    }

    private void SendCallback(IAsyncResult ir)
    {
        m_Socket.EndSend(ir);
        CheckSendBuffer();
    }

    private void OnDestroy()
    {
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
    private MemoryStream m_ReceiveStream = null;
    private byte[] m_ReceiveBuffer = null;
    private bool m_IsConnected = false;
    private string m_IP = string.Empty;
    private int m_CheckCount = 0;
    private int m_Port = int.MaxValue;
    private Socket m_Socket = null;
}