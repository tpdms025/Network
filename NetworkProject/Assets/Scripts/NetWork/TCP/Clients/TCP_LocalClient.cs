// ==============================================================
// Cracked 클라이언트 구조
//
// AUTHOR: Yang SeEun
// CREATED: 2020-05-11
// UPDATED: 2020-05-12
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

public class TCP_LocalClient : MonoBehaviour
{
    public enum PortType { WeatherData, SensorDataClient };
    public enum NetworkConnection { None, Connecting, Success, Fail };

    public PortType _portType;
    public NetworkConnection state = NetworkConnection.None;


    protected bool socketReady=false;
    protected TcpClient socket;
    protected NetworkStream stream;

    //receive
    protected byte[] receiveBuffer;
    protected int dataBufferSize = 4096;
    protected Thread receiveThread;

    private string stringData;
    public string StringData
    {
        get { return stringData; }
        set
        {
            stringData = value;
            Parsing(stringData);
        }
    }

    //info
    public string hostIP; //175.115.182.120
    public int port;

    #region Parsing
    protected virtual void Parsing(string _str)
    {
        //string[] separator = new string[1] { "\r\n" };  //분리할 기준 문자열
        //string[] _datas = _str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        //string[] datas = _datas[0].Split('^');

        //if (_portType == portType.SensorDataClient)
        //{
        //    int rpm = int.Parse(datas[0]);
        //    float power =float.Parse(datas[1]);
        //    float valveAngle = float.Parse(datas[2]);
        //    sensorData = new Data.SensorData(rpm, power, valveAngle);

        //}
        //else if(_portType == portType.WeatherData)
        //{
        //    float waveHeight = float.Parse(datas[0]);
        //    Vector3 waveDirection = StringToVector3(datas[1]);
        //    weatherData = new Data.WeatherData(waveHeight, waveDirection);
        //}
    }

    protected Vector3 StringToVector3(string sVector)
    {
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        string[] sArray = sVector.Split(',');

        Vector3 result = new Vector3(float.Parse(sArray[0]),float.Parse(sArray[1]),float.Parse(sArray[2]));

        return result;
    }
    #endregion

    #region TcpClient
    public void ConnectedToServer()
    {
        //이미 연결했다면 무시
        if (socketReady) return;

        if (hostIP == string.Empty || port == 0)
        {
            state = NetworkConnection.Fail;
            return;
        }
        socket = new TcpClient();
        Connect();
    }

    private void Connect()
    {
        try
        {
            state = NetworkConnection.Connecting;

            socket.BeginConnect(hostIP, port, ConnectCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Connect error : " + ex.Message);
            state = NetworkConnection.Fail;

        }
    }


    private void ConnectCallback(IAsyncResult _result)
    {
        try
        {
            state = NetworkConnection.Success;

            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                state = NetworkConnection.Fail;
                return;
            }

            //Connect후로 꼭 넣기
            socketReady = true;

            //receiveThread = new Thread(new ThreadStart(Receive));
            //receiveThread.IsBackground = false;
            //receiveThread.Start();


            stream = socket.GetStream();

            receiveBuffer = new byte[dataBufferSize];
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }
        catch (SocketException ex)
        {
            Debug.Log("Connect error : " + ex.Message);
            CloseSocket();
        }
    }


    private void ReceiveCallback(IAsyncResult _result)
    {
        try
        {
            int bytelength = stream.EndRead(_result);
            if (bytelength <= 0)
            {
                Debug.Log("byte length = " + bytelength);
                CloseSocket();
                return;
                
            }
            byte[] data = new byte[bytelength];
            Array.Copy(receiveBuffer, data, bytelength);
            StringData = Encoding.ASCII.GetString(data);


            Debug.Log("Receive : " + StringData);
          
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.Log("Receive Error : " + e.Message);
            CloseSocket();
        }
    }

    #region 이전버전 Receive
    private void Receive()
    {
        int bytelength;

        stream = socket.GetStream();

        try
        {
            //접속이 연결되어 있으면
            while (socket.Connected && (bytelength = stream.Read(receiveBuffer, 0, dataBufferSize)) > 0)
            {
                var incommingData = new byte[bytelength];
                Array.Copy(receiveBuffer, 0, incommingData, 0, bytelength);
                string data3 = Encoding.ASCII.GetString(incommingData);
                Debug.Log("receive: " + data3);
            }
            Debug.Log("Client read End");
        }
        catch (Exception e)
        {
            Debug.Log("Receive Error" + e.Message);
        }
    }

    #endregion

    private void CloseSocket()
    {
        if (socketReady)
        {
            stream.Close();
            socket.Close();
        }
        socketReady = false;
        state = NetworkConnection.Fail;
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }
    private void OnDestroy()
    {
        CloseSocket();
    }
    #endregion
}
