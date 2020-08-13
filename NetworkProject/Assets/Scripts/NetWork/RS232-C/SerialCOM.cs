using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System;
using System.IO;

public class SerialCOM : MonoBehaviour
{
    private SerialPort stream;
    public enum NetworkConnection { None, Connecting, Success, Fail };
    public NetworkConnection state = NetworkConnection.None;

    [SerializeField] private bool m_runThread = true;
    private Thread sendThread;
    private Thread receiveThread;
    [SerializeField] protected bool portReady = false;

    //receive
    //protected byte[] receiveBuffer;
    //protected int receiveBufferSize = 4096;
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
    public float sendIntervalTime = 1.0f;
    protected float time = 0.0f;

    //info
    [Header("Info")]
    public bool isSend = false;
    public string port =string.Empty;
    public int baudrate = 9600;

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


    public void Open()
    {
        //이미 연결했다면 무시
        if (portReady) return;

        state = NetworkConnection.Connecting;

        try
        {
            stream = new SerialPort(port, baudrate, Parity.None, 8, StopBits.One);
            stream.ReadTimeout = 1000;
            stream.WriteTimeout = 1000;

            stream.Open();
            portReady = true;
            m_runThread = true;

            Debug.Log("SerialPort was opened succesfully");
            state = NetworkConnection.Success;

            if (isSend)
            {
                SendMessages();
            }
            else
            {
                ReceiveMessages();
            }
        }
        catch (Exception ex)                //TimeoutException  IOException  Exception(NULL..)
        {
            Close();
            Debug.Log(ex.Message);
        }
    }

    private void SendMessages()
    {
        if (stream.IsOpen)
        {
            Debug.Log("Send ready");
            sendThread = new Thread(SendCallback);
            sendThread.IsBackground = false;
            sendThread.Start();
        }
           

    }
    private void SendCallback(object datas)
    {
        Queue<string> dataQueue = (Queue<string>)datas;
        int millsSec = (int)(sendIntervalTime * 1000);
        try
        {
            while (m_runThread)
            {
                if (dataQueue.Count > 0)
                {
                    string msg = dataQueue.Dequeue();
                    byte[] sendBuffer = Encoding.ASCII.GetBytes(msg);

                    stream.Write(sendBuffer, 0, sendBuffer.Length);
                    Debug.Log("Send to " + msg);
                }
                Thread.Sleep(millsSec);
            }
        }
        catch (Exception e)
        {
            Debug.Log("sendThrad error : " + e.Message);
        }
    }
    /*
    //test Send Func
    private void SendCallback()
    {
        int temp = 0;
        while (m_runThread)
        {
            string msg = "test" + temp++;
            byte[] sendData = Encoding.ASCII.GetBytes(msg);

            stream.Write(sendData, 0, sendData.Length);
            Debug.Log("Write : " + msg);

            Thread.Sleep(1000);
        }
    }
    */

    private void ReceiveMessages()
    {
        if (stream.IsOpen)
        {
            Debug.Log("SerialPort Receive ready");
            receiveThread = new Thread(ReceiveCallback);
            receiveThread.IsBackground = false;
            receiveThread.Start();
        }
    }

    private void ReceiveCallback()
    {
        try
        {
            //receiveBuffer = new byte[receiveBufferSize];
            while (m_runThread)
            {
                int bytelength = stream.BytesToRead;
                if (stream.BytesToRead > 0)
                {
                    //stream.Read(receiveBuffer, 0, bytelength);
                    //byte[] buff = new byte[bytelength];
                    //Array.Copy(receiveBuffer, buff, bytelength);
                    //receiveData = Encoding.ASCII.GetString(buff);


                    byte[] buff = new byte[bytelength];
                    stream.Read(buff, 0, bytelength);
                    receiveData = Encoding.ASCII.GetString(buff);
                    Debug.Log("receive data : " + receiveData);
                    //stream.ReadTimeout = 30;
                }
                Thread.Sleep(100);
            }
        }
        catch (Exception ex)
        {
#if UNITY_EDITOR
            Debug.Log("Receive Error" + ex.Message);
#endif
            Close();
        }
    }

    private void Close()
    {
        m_runThread = false;
        if (portReady && stream.IsOpen)
        {
            stream.Close();
            //Debug.Log("SerialPort Close");
        }
        portReady = false;
        state = NetworkConnection.Fail;

    }

    void OnApplicationQuit()
    {
        Close();
    }
    private void OnDisable()
    {
        Close();
    }
    private void OnDestroy()
    {
        Close();
    }
}
