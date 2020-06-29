
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
public class SensorDataClient : TCP_LocalClient
{
    public Data.SensorData sensorData;

    protected override void Parsing(string _str)
    {
        base.Parsing(_str);

        string[] separator = new string[1] { "\r\n" };  //분리할 기준 문자열
        string[] _datas = _str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        string[] datas = _datas[0].Split('^');

        int rpm = int.Parse(datas[0]);
        float power = float.Parse(datas[1]);
        float valveAngle = float.Parse(datas[2]);
        sensorData = new Data.SensorData(rpm, power, valveAngle);
    }
}
