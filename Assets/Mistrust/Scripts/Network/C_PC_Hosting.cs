using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Net.Sockets;
using System.Net;
using System.IO;
using System;


public class C_PC_Hosting : MonoBehaviour
{
    public InputField PortInput;

    ServerClient mobile = null;
    

    TcpListener server;
    bool serverStarted = false;
    bool isMobileConnected = false;
    public TextMeshProUGUI m_TMP_CurrIP = null;


    public string Local_IP
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

    private void Start()
    {
        DontDestroyOnLoad(this);
        ServerCreate();
    }

    //서버 생성
    public void ServerCreate()
    {
        try
        {
            //TODO : 포트는 선택하게 할까?
            //int port = PortInput.text == "" ? 7777 : int.Parse(PortInput.text);
            int port = 7777;
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;
            Debug.Log("서버 생성 완료");
            //Chat.instance.ShowMessage($"서버가 {port}에서 시작되었습니다.");
            if(m_TMP_CurrIP != null) m_TMP_CurrIP.text = string.Format("{0}:{1}", Local_IP, port);
        }

        //TODO : 에러 메세지 보내자
        catch (Exception e)
        {
            Debug.Log("서버 생성 실패 " + e.Message);
            //Chat.instance.ShowMessage($"Socket error: {e.Message}");
        }
    }

    void Update()
    {
        //서버가 시작됬거나 모바일이 들어왔는지 체크
        if (serverStarted == false || isMobileConnected == false) return;

        //여전히 연결중인가?
        if (!IsConnected(mobile.tcp))
        {

        }
        else 
        {
            //클라이언트로 부터 메세지 받음
            NetworkStream s = mobile.tcp.GetStream();
            if (s.DataAvailable) 
            {
                string data = new StreamReader(s, true).ReadLine();
                if (data != null) ReciveToMobile(data);
            }
        }

        

        //foreach (ServerClient c in clients)
        //{
        //    // 클라이언트가 여전히 연결되있나?
        //    if (!IsConnected(c.tcp))
        //    {
        //        c.tcp.Close();
        //        disconnectList.Add(c);
        //        continue;
        //    }
        //    // 클라이언트로부터 체크 메시지를 받는다
        //    else
        //    {
        //        NetworkStream s = c.tcp.GetStream();
        //        if (s.DataAvailable)
        //        {
        //            string data = new StreamReader(s, true).ReadLine();
        //            if (data != null)
        //                OnIncomingData(c, data);
        //        }
        //    }
        //}

        //for (int i = 0; i < disconnectList.Count - 1; i++)
        //{
        //    Broadcast($"{disconnectList[i].clientName} 연결이 끊어졌습니다", clients);

        //    clients.Remove(disconnectList[i]);
        //    disconnectList.RemoveAt(i);
        //}
    }


    //연결 확인
    bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }



    void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }


    //모바일 클라이언트 연결
    void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        mobile = new ServerClient(listener.EndAcceptTcpClient(ar));
        StartListening();
        isMobileConnected = true;
    }

    //폰으로 정보 보냄
    void SendToMobile(string _data) 
    {
        try
        {
            StreamWriter writer = new StreamWriter(mobile.tcp.GetStream());
            writer.WriteLine(_data);
            writer.Flush();

        }
        catch (Exception e)
        {
            Debug.Log("쓰기 에러");
        }
    }

    //모바일로 부터 받은 데이터 처리
    void ReciveToMobile(string _data) 
    {
        Debug.Log("recive from mobile " + _data);
        
        //if (_data == "DoorOpen") m_Door.DoorOpen();
        SendToMobile("Recived");
    }

    void FunctionExecuter(string _data)
    {
        var param = new object[] { 10 };
        this.GetType().GetMethod("dfdf").Invoke(this, param);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("QUIT");
        server.Stop();
    }
}



public class ServerClient
{
    public TcpClient tcp;
    public string clientName;
    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}
