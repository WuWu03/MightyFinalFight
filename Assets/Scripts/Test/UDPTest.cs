using client;
using GameFrameWork.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UDPTest : MonoBehaviour 
{
	public Button button;
	public TMP_InputField inputField;

    // Use this for initialization
    void Start () 
	{
		button.onClick.AddListener(onClick);
		inputField.gameObject.SetActive(false);

		NetMgr.instance.onConnectSuccessEvent += delegate () 
		{
			inputField.gameObject.SetActive(true);
			button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "发送";
		};

		NetMgr.instance.onDisConnectEvent += delegate ()
		{
			inputField.gameObject.SetActive(false);
			button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "连接";
		};

        button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "连接";
        TestNetResolver.instance.testReceiveEvent += OnReceive;
	}

	private void OnReceive(test test)
	{
		Debug.Log("收到服务端消息：" + test.content);
    }

	private void onClick()
	{
		if(!NetMgr.instance.isConnected)
		{
			NetMgr.instance.Connect("127.0.0.1", 8888);
			return;
		}

		TestNetResolver.instance.SendTest(inputField.text);
	}
}
