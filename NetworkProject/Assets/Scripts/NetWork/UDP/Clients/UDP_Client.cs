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
    public enum NetworkConnection { None, Connecting, Success, Fail };
    public NetworkConnection state = NetworkConnection.None;

    protected bool socketReady = false;
    protected UdpClient socket;

    protected Thread sendThread;
    private bool isApplicationQuit = false;

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

    //send
    protected Queue<string> sendData = new Queue<string>();
    private float sendIntervalTime = 1.0f;
    protected float time = 0.0f;

    //info
    public string hostIP = string.Empty;
    public int port = 0;
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

        if (hostIP == string.Empty || port == 0)
        {
            state = NetworkConnection.Fail;
            return;
        }
        state = NetworkConnection.Connecting;

        try
        {


            remoteEndPoint = new IPEndPoint(IPAddress.Parse(hostIP), port);
            //receive일땐
            socket = new UdpClient(remoteEndPoint);

            //Send일땐
            //socket = new UdpClient();
            //socket.Connect(listenIpEndPoint);  


            isApplicationQuit = false;
            socketReady = true;

            //SendMessages();
            ReceiveMessages();
        }  
        catch (SocketException ex)
        {
            Debug.Log(ex.Message);
        }

    }

    #region Receive
    protected void ReceiveMessages()
    {
        try
        {
            socket.BeginReceive(new AsyncCallback(ReceiveCallback), socket);

        }
        catch (Exception e)
        {
            Debug.Log("ReceiveThrad error : " + e.Message);
            CloseSocket();
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
                CloseSocket();
                return;
            }
            byte[] data = new byte[receiveBuffer.Length];
            Array.Copy(receiveBuffer, data, receiveBuffer.Length);

            ReceiveData = Encoding.ASCII.GetString(receiveBuffer);
            Debug.Log("Receive : " + ReceiveData);

            socket.BeginReceive(new AsyncCallback(ReceiveCallback), socket);
        }
        catch (Exception e)
        {
            Debug.Log("Receive Error" + e.Message);
            CloseSocket();
        }
    }
    #endregion

    #region Send
    public void OnIncomingData(InputField input)
    {
        if (socketReady)
        {
            sendData.Enqueue(input.text);
        }
    }

    private void SendMessages()
    {
        sendThread = new Thread(() => SendCallback(sendData));
        sendThread.IsBackground = false;
        sendThread.Start();
    }

    private void SendCallback(object datas)
    {
        Queue<string> dataQueue = (Queue<string>)datas;
        int millsSec = (int)(sendIntervalTime * 1000);

        //DateTime nextLoop = DateTime.Now;
        //TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);

        try
        {
            while (!isApplicationQuit)
            {

                if (dataQueue.Count > 0)
                {
                    //remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                    //socket.Connect(remoteEndPoint);

                    string msg = dataQueue.Dequeue();
                    byte[] sendBuffer = Encoding.ASCII.GetBytes(msg);


                    socket.Send(sendBuffer, sendBuffer.Length);
                    Debug.Log("Send " + remoteEndPoint.Address.ToString() + " to " + msg);
                }

                Thread.Sleep(100);
            }
            //Debug.Log(isApplicationQuit.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("sendThrad error : " + e.Message);
        }
    }
    #endregion


    protected void CloseSocket()
    {
        if (socketReady)
        {

            socket.Dispose();
            socket.Close();
        }

        socketReady = false;
        state = NetworkConnection.Fail;
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
