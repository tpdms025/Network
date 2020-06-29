// ==============================================================
// Cracked 포트 구조
//
// AUTHOR: Yang SeEun
// CREATED: 2020-05-11
// UPDATED: 2020-05-11
// ==============================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TCP_ListenPort : MonoBehaviour
{
    #region DebugText용
    private Queue<string> debugQueue = new Queue<string>();
    public Text debugText;
    private void Log(string _str)
    {
        debugQueue.Enqueue(_str + "\n");
    }

    #endregion

    public string portName = string.Empty;

    public List<TCP_NetClient> clients;
    public List<TCP_NetClient> disconnectList;

    public int port;
    protected TcpListener tcpListener;

    protected Thread serverThread;

    protected Queue<string> sendData = new Queue<string>();
    public float sendIntervalTime = 10.0f;
    protected float time = 0.0f;
    private void Start()
    {
        StartServer();
    }

    public void StartServer()
    {
        clients = new List<TCP_NetClient>();
        disconnectList = new List<TCP_NetClient>();

        try
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();


            //StartListening
            StartListening();

            Log("Server has been started on " + portName + " " + port.ToString());
        }
        catch (Exception ex)
        {
            Log("Socket error :" + ex.Message);
        }
    }
    private void StartListening()
    {
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClient), null);

    }
    private void AcceptTcpClient(IAsyncResult result)
    {
        //TcpListener listener = (TcpListener)result.AsyncState;
        TcpClient client = tcpListener.EndAcceptTcpClient(result);
        Log(portName + " Accept : " + client.Client.LocalEndPoint);

        clients.Add(new TCP_NetClient(client));

        tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClient), null);

        clients[clients.Count - 1].connecting = true;
        clients[clients.Count - 1].clientName = portName;
        //Send Data
        serverThread = new Thread(() => clients[clients.Count - 1].Send(sendData, clients[clients.Count - 1]));
        serverThread.IsBackground = false;
        serverThread.Start();

    }
    protected virtual void Update()
    {
        if(debugQueue.Count > 0)
        {
            debugText.text += debugQueue.Dequeue();
        }
        //clients connect check
        if (clients != null)
        {
            foreach (TCP_NetClient c in clients)
            {
                if (!IsConnected(c.socket))
                {
                    c.connecting = false;
                    c.socket.Close();
                    disconnectList.Add(c);
                    continue;
                }
            }
        }

        for (int i = 0; i < disconnectList.Count - 1; i++)
        {
            Log(disconnectList[i].clientName + " is close");
            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }

    }
    protected bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    private void CloseSocket()
    {
        tcpListener.Stop();
        foreach (TCP_NetClient c in clients)
        {
            c.connecting = false;
            c.socket.Close();
        }
    }

    private void OnDestroy()
    {
        CloseSocket();
        if (clients != null)
        {
            foreach (TCP_NetClient c in clients)
            {
                c.connecting = false;
                Log("connect End");
            }
        }
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }
}
