using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDataPort : UDP_Server
{

    protected override void Update()
    {
        base.Update();

        time += Time.deltaTime;
        if ((time >= sendIntervalTime))
        {
            //TODO:
            float waveHeight = UnityEngine.Random.Range(0.0f, 2.3f);
            Vector3 waveDirection = new Vector3(UnityEngine.Random.Range(0.1f, 1.0f), UnityEngine.Random.Range(0.0f, 0.7f), UnityEngine.Random.Range(0.7f, 0.9f));

            string data = waveHeight + "^" + waveDirection;
            sendData.Enqueue(data);
            time = 0.0f;
        }
    }
}
