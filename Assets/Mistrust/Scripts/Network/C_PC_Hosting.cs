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

    //���� ����
    public void ServerCreate()
    {
        try
        {
            //TODO : ��Ʈ�� �����ϰ� �ұ�?
            //int port = PortInput.text == "" ? 7777 : int.Parse(PortInput.text);
            int port = 7777;
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;
            Debug.Log("���� ���� �Ϸ�");
            //Chat.instance.ShowMessage($"������ {port}���� ���۵Ǿ����ϴ�.");
            if(m_TMP_CurrIP != null) m_TMP_CurrIP.text = string.Format("{0}:{1}", Local_IP, port);
        }

        //TODO : ���� �޼��� ������
        catch (Exception e)
        {
            Debug.Log("���� ���� ���� " + e.Message);
            //Chat.instance.ShowMessage($"Socket error: {e.Message}");
        }
    }

    void Update()
    {
        //������ ���ۉ�ų� ������� ���Դ��� üũ
        if (serverStarted == false || isMobileConnected == false) return;

        //������ �������ΰ�?
        if (!IsConnected(mobile.tcp))
        {

        }
        else 
        {
            //Ŭ���̾�Ʈ�� ���� �޼��� ����
            NetworkStream s = mobile.tcp.GetStream();
            if (s.DataAvailable) 
            {
                string data = new StreamReader(s, true).ReadLine();
                if (data != null) ReciveToMobile(data);
            }
        }

        

        //foreach (ServerClient c in clients)
        //{
        //    // Ŭ���̾�Ʈ�� ������ ������ֳ�?
        //    if (!IsConnected(c.tcp))
        //    {
        //        c.tcp.Close();
        //        disconnectList.Add(c);
        //        continue;
        //    }
        //    // Ŭ���̾�Ʈ�κ��� üũ �޽����� �޴´�
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
        //    Broadcast($"{disconnectList[i].clientName} ������ ���������ϴ�", clients);

        //    clients.Remove(disconnectList[i]);
        //    disconnectList.RemoveAt(i);
        //}
    }


    //���� Ȯ��
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


    //����� Ŭ���̾�Ʈ ����
    void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        mobile = new ServerClient(listener.EndAcceptTcpClient(ar));
        StartListening();
        isMobileConnected = true;
    }

    //������ ���� ����
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
            Debug.Log("���� ����");
        }
    }

    //����Ϸ� ���� ���� ������ ó��
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
