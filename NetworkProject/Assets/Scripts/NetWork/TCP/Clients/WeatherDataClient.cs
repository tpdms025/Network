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

public class WeatherDataClient : TCP_LocalClient
{
    public Data.WeatherData weatherData;

    protected override void Parsing(string _str)
    {
        base.Parsing(_str);

        string[] separator = new string[1] { "\r\n" };  //분리할 기준 문자열
        string[] _datas = _str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        string[] datas = _datas[0].Split('^');

        float waveHeight = float.Parse(datas[0]);
        Vector3 waveDirection = StringToVector3(datas[1]);
        weatherData = new Data.WeatherData(waveHeight, waveDirection);
    }
}
