using UnityEngine;
using System.Collections;

[System.Serializable]
public class Patient : System.Object
{
	    public string anonId;
        public string device;
        public float minInitialAngle;
        public float maxInitialAngle;
        public float assistPressure;
        public float assistPressureHysteresis;
        public bool setInitialROM;
        public float dynamicROMMin;
        public float dynamicROMMax;

    public string GetAnonId()
    {
        return this.anonId;
    }
    public void SetAnonId(string anonId)
    {
        this.anonId = anonId;
    }
    public string SetDevice()
    {
        return this.device;
    }
    public void SetDevice(string device)
    {
        this.device = device;
    }
    public float GetMinInitialAngle()
    {
        return this.minInitialAngle;
    }
    public void SetMinInitialAngle(float minInitialAngle)
    {
        this.minInitialAngle = minInitialAngle;

    }
    public float GetMaxInitialAngle()
    {
        return this.maxInitialAngle;
    }
    public void SetMaxInitialAngle(float maxInitialAngle)
    {
        this.maxInitialAngle = maxInitialAngle;
    }
    public float GetAssistPressure()
    {
        return this.assistPressure;
    }
    public void SetAssistPressure(float assistPressure)
    {
        this.assistPressure = assistPressure;
    }
    public float GetAssistPressureHysteresis()
    {
        return this.assistPressureHysteresis;
    }
    public void SetAssistPressureHysteresis(float assistPressureHysteresis)
    {
        this.assistPressureHysteresis = assistPressureHysteresis;
    }
    public bool GetSetInitialROM()
    {
        return this.setInitialROM;
    }
    public void SetSetInitialROM(bool setInitialROM)
    {
        this.setInitialROM = setInitialROM;
    }
    public void SetDynamicROMMin(float dynamicROMMin)
    {
        this.dynamicROMMin = dynamicROMMin;
    }
    public void SetDynamicROMMax(float dynamicROMMax)
    {
        this.dynamicROMMax = dynamicROMMax;
    }
    public float GetDynamicROMMin()
    {
        return this.dynamicROMMin;
    }
    public float GetDynamicROMMax()
    {
        return this.dynamicROMMax;
    }
}