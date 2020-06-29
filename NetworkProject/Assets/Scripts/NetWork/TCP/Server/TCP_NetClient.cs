// ==============================================================
// Cracked 네트워크 클라이언트
//
// AUTHOR: Yang SeEun
// CREATED: 2020-04-27
// UPDATED: 2020-05-11
// ==============================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TCP_NetClient
{
    public TcpClient socket;
    public string clientName;
    public bool connecting = false;

    public TCP_NetClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        socket = clientSocket;
    }

    //public void Receive()
    //{
    //    try
    //    {
    //        NetworkStream stream; /*= socket.GetStream();*/
    //        Debug.Log("receive");

    //        while (true)
    //        {
    //            stream = socket.GetStream();

    //            if (!IsConnected(socket))
    //            {
    //                socket.Close();
    //                break;
    //                //disconnectList.Add(c);
    //                //for (int i = 0; i < disconnectList.Count - 1; i++)
    //                //{
    //                //    Broadcast(disconnectList[i].clientName + " has disconnected", clients);

    //                //    clients.Remove(disconnectList[i]);
    //                //    disconnectList.RemoveAt(i);
    //                //}
    //            }
    //            else if (stream.DataAvailable)
    //            {
    //                StreamReader reader = new StreamReader(stream, true);
    //                string data = reader.ReadLine();

    //                if (data != null)
    //                {
    //                    Debug.Log(this.clientName + "has sent the following message : " + data);
    //                    SendData(data);
    //                }
    //            }

    //            Thread.Sleep(10);
    //        }

    //        Debug.Log("Receive End : ");
    //    }
    //    catch (SocketException socketException)
    //    {
    //        Debug.Log("Receive error : " + socketException.Message);

    //    }
    //}

    public void SendData(string data)
    {
        if (socket == null) { return; }
        try
        {
            StreamWriter writer = new StreamWriter(socket.GetStream());

            writer.WriteLine(data);
            writer.Flush();

        }
        catch (Exception e)
        {
            Debug.Log("Write error : " + e.Message + " to client " + clientName);
        }

    }

    public void Send(object datas, object _client)
    {
        Queue<string> dataList = (Queue<string>)datas;
        TCP_NetClient client = (TCP_NetClient)_client;

        try
        {
            while (client.socket.Connected && client.connecting)
            {
                if (dataList.Count != 0)
                {
                    //data
                    client.SendData(dataList.Dequeue());
                }

                Thread.Sleep(100);
            }

            //close
            Debug.Log("Server Send End");
            
        }
        catch (SocketException e)
        {
            Debug.Log("Write error : " + e.Message + " to client " + client.clientName);
        }
    }



    
}
