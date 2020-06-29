// ==============================================================
// Cracked 센서데이터를 관리하는 포트
//
// AUTHOR: Yang SeEun
// CREATED: 2020-04-27
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



public class SensorDataPort : TCP_ListenPort
{

    private void Awake()
    {
        portName = "SensorDataPort";
    }

    protected override void Update()
    {
        base.Update();
        time += Time.deltaTime;
        if ((time >= sendIntervalTime) && clients.Count != 0)
        {
            int rpm = UnityEngine.Random.Range(1, 61);
            float power = UnityEngine.Random.Range(0.0f, 100.0f);
            float valveAngle = UnityEngine.Random.Range(0.0f, 100.0f);
            
            string data = rpm +"^"+ power + "^"+ valveAngle;
            sendData.Enqueue(data);

            time = 0.0f;
        }
    }


}
