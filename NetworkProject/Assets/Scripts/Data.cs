// ==============================================================
// Cracked Database
//
// AUTHOR: Yang SeEun
// CREATED: 2020-04-27
// UPDATED: 2020-05-11
// ==============================================================


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    [Serializable]
    public class WeatherData
    {
        public float waveHeight;             //파도의 높이
        public Vector3 waveDirection;        //파도의 방향

        public WeatherData(float waveHeight, Vector3 waveDirection)
        {
            this.waveHeight = waveHeight;
            this.waveDirection = waveDirection;
        }
    }

    [Serializable]
    public class SensorData
    {
        public int rpm;              //터빈 회전 속도
        public float power;         //전력량
        public float valveAngle;    //밸브 각도

        public SensorData(int rpm, float power, float valveAngle)
        {
            this.rpm = rpm;
            this.power = power;
            this.valveAngle = valveAngle;
        }
    }

    [Serializable]
    public class WaveData
    {
        public int a;              
        public float b;


        public WaveData(int _a, float _b)
        {
            this.a = _a;
            this.b = _b;
        }
    }
}
