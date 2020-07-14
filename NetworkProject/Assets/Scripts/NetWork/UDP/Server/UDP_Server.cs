// ==============================================================
// Cracked Udp 서버 구조 (송신)
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
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UDP_Server : MonoBehaviour
{

    #region DebugText용
    [Header("[DebugTest]")]
    public Text debugText;
    private void Log(string _str)
    {
        debugQueue.Enqueue(_str + "\n");
    }

    public InputField hostField = null;
    public InputField portField = null;
    public InputField sendTimeField = null;

    private Queue<string> debugQueue = new Queue<string>();

    [Space(20)]
    #endregion


    public string portName = string.Empty;
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
    public float sendIntervalTime = 3.0f;
    protected float time = 0.0f;

    public string ip = "192.168.0.19";
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


    private void Start()
    {
        //UdpStart();
    }

    protected virtual void Update()
    {
        //Debug용
        if (debugQueue.Count > 0)
        {
            debugText.text += debugQueue.Dequeue();
        }
    }

    //test용 세팅
    protected void LoginSetting()
    {
        if (hostField.GetComponent<InputField>().text != "")
        {
            ip = hostField.GetComponent<InputField>().text;
        }
        if (portField.GetComponent<InputField>().text != "")
        {
            port = int.Parse(portField.GetComponent<InputField>().text);
        }
        if (sendTimeField.GetComponent<InputField>().text != "")
        {
            sendIntervalTime = float.Parse(sendTimeField.GetComponent<InputField>().text);
        }
    }
    public void UdpStart()
    {
        try
        {
            LoginSetting();
            socket = new UdpClient();
            isApplicationQuit = false;

            if (!socketReady)
            {
                socketReady = true;
                Log("Server has been started on " + portName + " " + port.ToString());
                SendMessages();
            }
        }
        catch (Exception ex)
        {
            Log("server Socket error :" + ex.Message);
        }
    }

    #region Send
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
        try
        {
            while (!isApplicationQuit)
            {
                if (dataQueue.Count > 0)
                {
                    remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                    socket.Connect(remoteEndPoint);

                    string msg = dataQueue.Dequeue();
                    byte[] sendBuffer = Encoding.ASCII.GetBytes(msg);


                    socket.Send(sendBuffer, sendBuffer.Length);
                    Log("Send " + remoteEndPoint.Address.ToString() + " to " + msg);
                }
                Thread.Sleep(millsSec);
            }
            Debug.Log(isApplicationQuit.ToString());
        }
        catch (Exception e)
        {
            Log("sendThrad error : " + e.Message);
        }
    }

    //test Send Func
    private void SendCallback()
    {
        try
        {
            int temp = 0;
            while (!isApplicationQuit)
            {

                remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                socket.Connect(remoteEndPoint);

                string msg = temp.ToString();
                byte[] sendBuffer = Encoding.ASCII.GetBytes(msg);

                socket.Send(sendBuffer, sendBuffer.Length);
                Debug.Log("Send " + remoteEndPoint.Address.ToString() + " to " + msg);


                temp++;
                Thread.Sleep(100);
            }
            Debug.Log(isApplicationQuit.ToString());

        }
        catch (Exception e)
        {
            Debug.Log("sendThrad error : " + e.Message);
        }
    }
    #endregion

    #region Receive
    private void ReceiveMessages()
    {
        try
        {
            socket.BeginReceive(new AsyncCallback(ReceiveCallback), socket);

        }
        catch (Exception e)
        {
            Log("ReceiveThrad error : " + e.Message);
        }
    }

    private void ReceiveCallback(IAsyncResult _result)
    {
        try
        {
            receiveBuffer = socket.EndReceive(_result, ref remoteEndPoint);

            if (receiveBuffer.Length <= 0 /*|| RemoteIpEndPoint.Address.ToString().Equals(Network.player.ipAddress)*/)
            {
                Log("byte length = " + receiveBuffer.Length);
                return;
            }
            ReceiveData = Encoding.ASCII.GetString(receiveBuffer);
            Log("Receive : " + ReceiveData);

            socket.BeginReceive(new AsyncCallback(ReceiveCallback), socket);
        }
        catch (Exception e)
        {
            Log("Receive Error" + e.Message);
        }
    }
    #endregion

    #region 비동기 Send (현재 사용안함)

    private bool messageSent = false;

    private void SendData(string msg = null)
    {
        try
        {
            while (messageSent && sendIntervalTime > time)
            {
                messageSent = false;
                if (msg == null)
                {
                    //TODO:
                    msg = UnityEngine.Random.Range(0.0f, 100.0f).ToString() + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss tt");
                }
                Debug.Log("Data is : '" + msg + "' ready");

                byte[] sendBuffer = Encoding.ASCII.GetBytes(msg);
                //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.19"), port);

                socket.BeginSend(sendBuffer, sendBuffer.Length, remoteEndPoint, new AsyncCallback(SendCallback), socket);
                time += Time.deltaTime;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Send Error : " + e.Message);
        }
    }

    private void SendCallback(IAsyncResult _result)
    {
        try
        {
            UdpClient u = (UdpClient)_result.AsyncState;

            Debug.Log("server number of bytes sent: " + u.EndSend(_result).ToString());
            messageSent = true;

        }
        catch (Exception e)
        {
            Debug.Log("Sendcall Error : " + e.Message);
        }

    }

    #endregion

    //test용 button
    public void UdpStop()
    {
        CloseSocket();
        Log(portName + " Stop!");
    }

    private void CloseSocket()
    {
        isApplicationQuit = true;
        socketReady=false;

        if (socketReady)
        {
            socket.Dispose();
            socket.Close();
        }
    }

    private void OnDestroy()
    {
        CloseSocket();
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
