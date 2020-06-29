// ==============================================================
// Cracked UDP 클라이언트 구조 (수신)
//
// AUTHOR: Yang SeEun
// CREATED: 2020-06-09
// UPDATED: 2020-06-09
// ==============================================================


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class UDP_Client : MonoBehaviour
{
    public enum UDP_portType { };
    protected bool socketReady = false;
    protected UdpClient socket;

    //receive
    protected byte[] receiveBuffer;
    private string receiveData;
    public string ReceiveData
    {
        get { return receiveData; }
        set
        {
            receiveData = value;
            Parsing(receiveData);
        }
    }

    public string hostIP = "192.168.0.19"; //175.115.182.120
    public int port;
    private IPEndPoint remoteEndPoint;

    #region Parsing

    protected virtual void Parsing(string _str)
    {
    }

    protected Vector3 StringToVector3(string sVector)
    {
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        string[] sArray = sVector.Split(',');

        Vector3 result = new Vector3(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]));

        return result;
    }
    #endregion

    #region UdpClient

    public void ConnectedToServer()
    {
        //이미 연결했다면 무시
        if (socketReady) return;
        socketReady = true;

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(hostIP), port);

        socket = new UdpClient(remoteEndPoint);
        //socket.Connect(listenIpEndPoint);

        ReceiveMessages();

    }

    protected void ReceiveMessages()
    {
        try
        {
            socket.BeginReceive(new AsyncCallback(ReceiveCallback), socket);

        }
        catch (Exception e)
        {
            Debug.Log("ReceiveThrad error : " + e.Message);
        }
    }
    protected void ReceiveCallback(IAsyncResult _result)
    {
        try
        {
            receiveBuffer = socket.EndReceive(_result, ref remoteEndPoint);

            if (receiveBuffer.Length <= 0 /*|| RemoteIpEndPoint.Address.ToString().Equals(Network.player.ipAddress)*/)
            {
                Debug.Log("byte length = " + receiveBuffer.Length);
                return;
            }
            ReceiveData = Encoding.ASCII.GetString(receiveBuffer);
            Debug.Log("Receive : " + ReceiveData);

            socket.BeginReceive(new AsyncCallback(ReceiveCallback), socket);
        }
        catch (Exception e)
        {
            Debug.Log("Receive Error" + e.Message);
        }
    }




    protected void CloseSocket()
    {
        if (!socketReady)
        {
            return;
        }

        socket.Dispose();
        socket.Close();
        Debug.Log("socket close");
        socketReady = false;

    }

    protected void OnApplicationQuit()
    {
        CloseSocket();
    }
    protected void OnDisable()
    {
        CloseSocket();
    }
    protected void OnDestroy()
    {
        CloseSocket();
    }
    #endregion
}
