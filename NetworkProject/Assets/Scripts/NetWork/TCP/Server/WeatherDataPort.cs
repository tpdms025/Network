// ==============================================================
// Cracked 환경정보를 관리하는 포트
//
// AUTHOR: Yang SeEun
// CREATED: 2020-04-27
// UPDATED: 2020-05-11
// ==============================================================


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.IO;
using System.Threading;

public class WeatherDataPort : TCP_ListenPort
{

    private void Awake()
    {
        portName = "WeatherDataPort";
    }
    protected override void Update()
    {
        base.Update();
        time += Time.deltaTime;
        if ((time >= sendIntervalTime) && clients.Count != 0)
        {
            //TODO:
            float waveHeight = UnityEngine.Random.Range(0.0f, 2.3f);
            Vector3 waveDirection = new Vector3(UnityEngine.Random.Range(0.1f, 1.0f), UnityEngine.Random.Range(0.0f, 0.7f), UnityEngine.Random.Range(0.7f, 0.9f));
           
            string data = waveHeight + "^" + waveDirection;
            sendData.Enqueue(data);

            time = 0.0f;
        }
    }
    //public List<NetClient> clients;
    //public List<NetClient> disconnectList;

    //public int port;
    //private TcpListener tcpListener;

    //private Thread serverThread;

    //private List<string> weatherData = new List<string>();
    //public float sendIntervalTime = 10.0f;
    //float time = 0.0f;

    //private void Start()
    //{
    //    StartServer();
    //}
    //public void StartServer()
    //{
    //    clients = new List<NetClient>();
    //    disconnectList = new List<NetClient>();

    //    try
    //    {
    //        tcpListener = new TcpListener(IPAddress.Any, port);
    //        tcpListener.Start();

    //        //StartListening
    //        StartListening();

    //        Debug.Log("Server has been started on port " + port.ToString());
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.Log("Socket error :" + ex.Message);
    //    }
    //}

    //private void StartListening()
    //{
    //    tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClient), null);
    //}
    //private void AcceptTcpClient(IAsyncResult result)
    //{

    //    //TcpListener listener = (TcpListener)result.AsyncState;
    //    TcpClient client = tcpListener.EndAcceptTcpClient(result);
    //    Debug.Log("WeatherDataPort Accept : " + client.Client.LocalEndPoint);

    //    clients.Add(new NetClient(client));

    //    tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClient), null);

    //    clients[clients.Count - 1].connecting = true;

    //    //Send WeatherInformation
    //    serverThread = new Thread(() => clients[clients.Count - 1].Send(weatherData, clients[clients.Count - 1]));
    //    serverThread.IsBackground = false;
    //    serverThread.Start();
    //}



    ////test send
    //public void Send()
    //{
    //    try
    //    {
    //        while (clients[clients.Count - 1].socket.Connected && clients[clients.Count - 1].connecting)
    //        {
    //            if (weatherData.Count != 0)
    //            {
    //                //data
    //                clients[clients.Count - 1].SendData(weatherData[0]);
    //                weatherData.RemoveAt(0);
    //            }

    //            Thread.Sleep(100);
    //        }
    //        clients[clients.Count - 1].connecting = false;
    //        Debug.Log("WeatherDataPort connect End");
    //    }
    //    catch (SocketException e)
    //    {
    //        Debug.Log("WeatherDataPort Write error : " + e.Message + " to client " + clients[clients.Count - 1].clientName);
    //    }
    //}


    //private void CloseSocket()
    //{
    //    tcpListener.Stop();
    //}

    //private void OnDestroy()
    //{
    //    CloseSocket();
    //    if (clients != null)
    //    {
    //        foreach (NetClient c in clients)
    //        {
    //            c.connecting = false;
    //            Debug.Log("connect End");
    //        }
    //    }
    //}

    //private void OnApplicationQuit()
    //{
    //    CloseSocket();
    //}
    //private void OnDisable()
    //{
    //    CloseSocket();
    //}

}
