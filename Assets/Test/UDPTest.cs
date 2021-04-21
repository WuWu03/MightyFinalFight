using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Net.Security;
using GameFrameWork.Net;
using client;

public class UDPTest : MonoBehaviour 
{
	public Button button;
	public InputField inputField;
	// Use this for initialization
	void Start () {
	
		button.onClick.AddListener(onClick);
		inputField.gameObject.SetActive(false);

		SocketMgr.Instance.OnConnectSuccess = delegate () 
		{
			inputField.gameObject.SetActive(true);
			button.transform.Find("Text").GetComponent<Text>().text = "发送";
		};

		SocketMgr.Instance.OnDisConnect = delegate ()
		{
			inputField.gameObject.SetActive(false);
			button.transform.Find("Text").GetComponent<Text>().text = "连接";
		};

		SocketMgr.Instance.onReceive = OnReceive;
	}

	private void OnReceive(ushort arg1, byte[] arg2)
	{

		inputField.text = arg1 + "," + Encoding.UTF8.GetString(arg2);
	}

	private void onClick()
	{
		if(!SocketMgr.Instance.IsConnected)
		{
			//IPAddress[] address = Dns.GetHostAddresses("st14818931.iask.in");
			SocketMgr.Instance.Connect("127.0.0.1", 8888);
			return;
		}

		client.test sendContent = new client.test();
		sendContent.content = inputField.text;
		byte[] buffer = ProtoBufUtil.ObjectToBytes<test>(sendContent);
		SocketMgr.Instance.Send(1, buffer);
	}
}
