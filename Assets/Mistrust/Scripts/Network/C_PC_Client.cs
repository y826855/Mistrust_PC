using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;
using TMPro;

public class C_PC_Client : MonoBehaviour
{
    bool socketReady;
    TcpClient socket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;


	public TextMeshProUGUI m_TMP_CurrIP = null;
	public static string Client_IP
	{
		get
		{
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			string ClientIP = string.Empty;
			for (int i = 0; i < host.AddressList.Length; i++)
			{
				if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
				{
					ClientIP = host.AddressList[i].ToString();
				}
			}
			return ClientIP;
		}
	}

	public void ConnectToServer()
	{
		// 이미 연결되었다면 함수 무시
		if (socketReady) return;

		// 기본 호스트/ 포트번호
		string ip = Client_IP;
		int port = 7777;

		// 소켓 생성
		try
		{
			socket = new TcpClient(ip, port);
			stream = socket.GetStream();
			writer = new StreamWriter(stream);
			reader = new StreamReader(stream);
			socketReady = true;
		}
		catch (Exception e)
		{
			Debug.Log("참가 실패");
			//Chat.instance.ShowMessage($"소켓에러 : {e.Message}");
		}
	}

    private void Start()
    {
		//ConnectToServer();
	}

	public void JoinServer() 
	{
		ConnectToServer();
	}

	void Update()
	{
		if (socketReady && stream.DataAvailable)
		{
			string data = reader.ReadLine();
			if (data != null)
				OnIncomingData(data);
		}
	}

	void OnIncomingData(string _data) 
	{
		Debug.Log("받음 : " + _data);
	}

	void FunctionExecuter(string _data) 
	{
		var param = new object[] { 10 };
		this.GetType().GetMethod("dfdf").Invoke(this, param);
	}

	public void dfdf(int a) 
	{
		Debug.Log(a);
	}

	public void SendAnything() 
	{
		if (!socketReady) return;
		Debug.Log("전송시작");
		writer.WriteLine("전   송  했   다");
		writer.Flush();
	}

}
