using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDataClient : UDP_Client
{
    public Data.WaveData waveData;

    protected override void Parsing(string _str)
    {
        base.Parsing(_str);

        string[] separator = new string[1] { "\r\n" };  //분리할 기준 문자열
        string[] _datas = _str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        string[] datas = _datas[0].Split('^');

        int a = int.Parse(datas[0]);
        float b = float.Parse(datas[1]);
        waveData = new Data.WaveData(a, b);
    }
}
