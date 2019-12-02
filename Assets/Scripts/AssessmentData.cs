using System.Collections.Generic;
using UnityEngine;
using Ludiq;
using Bolt;
using System.Data;
using System.Linq;

public class AssessmentData : MonoBehaviour
{
    List<float> downListAngles;
    List<float> downListPressures;
    List<float> upListAngles;
    List<float> upListPressures;
    private AndroidJavaClass UnityPlayer;
    private AndroidJavaObject currentActivity;
    public static AssessmentData instance;
    void Awake()
    {
        // Insure only a single AssessmentData exists for all scenes.
        if (instance == null)
        {
            instance = this;
            downListAngles = new List<float>();
            downListPressures = new List<float>();
            upListAngles = new List<float>();
            upListPressures = new List<float>();
        }
        else if (instance != this)
        {
            //Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }

    //public void AddToDwnList(float angle, float pressure)
    //{
    //    downListAngles.Add(angle);
    //    downListPressures.Add(pressure);
    //}

    //public void AddToUpList(float angle, float pressure)
    //{
    //    upListAngles.Add(angle);
    //    upListPressures.Add(pressure);
    //}

    //public void GetROMAndPressure()
    //{
    //    float percentCut = .05f;
    //    //reorder DOWN lists based on angle order
    //    var orderedZipDown = downListAngles.Zip(downListPressures, (x, y) => new { x, y })
    //                  .OrderBy(pair => pair.x)
    //                  .ToList();
    //    downListAngles = orderedZipDown.Select(pair => pair.x).ToList();
    //    downListPressures = orderedZipDown.Select(pair => pair.y).ToList();

    //    int tempLengthDown = downListAngles.Count;
    //    int portionDown = (int)(tempLengthDown * percentCut);
    //    List<float> filteredDownListAngles = new List<float>();
    //    List<float> filteredDownListPressures = new List<float>();
    //    //create new list with total 10% cut for DOWN
    //    for (int i = portionDown; i < tempLengthDown - portionDown - 1; i++)
    //    {
    //        filteredDownListAngles.Add(downListAngles[i]);
    //        filteredDownListPressures.Add(downListPressures[i]);
    //    }

    //    int pressuresCount = filteredDownListPressures.Count - 1;
    //    List<float> downAnglesFinal = new List<float>();
    //    List<float> downPressuresFinal = new List<float>();
    //    int count = 0;

    //    //create new lists with the global min for angle and respective pressure
    //    for (int j = 0;  j < pressuresCount; j++)
    //    {
    //        bool pressureExists = downPressuresFinal.Contains(filteredDownListPressures[j]);
    //        float currPressure = filteredDownListPressures[j];
    //        float currAngle = filteredDownListAngles[j];
    //        int finalAngleCountDown = downAnglesFinal.Count - 1;

    //        if (pressureExists)
    //        {
    //            float existingAngle = downAnglesFinal[finalAngleCountDown];
    //            if (currAngle < existingAngle)
    //            {
    //                downAnglesFinal[finalAngleCountDown] = currAngle;
    //            }
    //        }
    //        else
    //        {
    //            count++;
    //            downAnglesFinal.Add(currAngle);
    //            downPressuresFinal.Add(currPressure);
    //        }
    //    }
    //    Debug.Log("COUNTdown:" + count);

    //    //reorder UP lists based on angle order
    //    var orderedZipUp = upListAngles.Zip(upListPressures, (x, y) => new { x, y })
    //          .OrderBy(pair => pair.x)
    //          .ToList();
    //    upListAngles = orderedZipUp.Select(pair => pair.x).ToList();
    //    upListPressures = orderedZipUp.Select(pair => pair.y).ToList();

    //    int tempLengthUp = upListAngles.Count;
    //    int portionUp = (int)(tempLengthUp * percentCut);
    //    List<float> filteredUpListAngles = new List<float>();
    //    List<float> filteredUpListPressures = new List<float>();
    //    //create new list with total 10% cut for UP
    //    for (int i = portionUp; i < tempLengthUp - portionUp - 1; i++)
    //    {
    //        filteredUpListAngles.Add(upListAngles[i]);
    //        filteredUpListPressures.Add(upListPressures[i]);
    //    }

    //    int pressuresCountUp = filteredUpListPressures.Count - 1;
    //    List<float> upAnglesFinal = new List<float>();
    //    List<float> upPressuresFinal = new List<float>();
    //    count = 0;

    //    //create new lists with the global min for angle and respective pressure
    //    for (int j = 0; j < pressuresCountUp; j++)
    //    {
    //        bool pressureExists = upPressuresFinal.Contains(filteredUpListPressures[j]);
    //        float currPressure = filteredUpListPressures[j];
    //        float currAngle = filteredUpListAngles[j];
    //        int finalAngleCountUp = upAnglesFinal.Count - 1;

    //        if (pressureExists)
    //        {
    //            float existingAngle = upAnglesFinal[finalAngleCountUp];
    //            if (currAngle < existingAngle)
    //            {
    //                upAnglesFinal[finalAngleCountUp] = currAngle;
    //            }
    //        }
    //        else
    //        {
    //            count++;
    //            downAnglesFinal.Add(currAngle);
    //            downPressuresFinal.Add(currPressure);
    //        }
    //    }

    //    Debug.Log("COUNTup:" + count);

    //    int countVals = 0;
    //    float maxROM = 0;
    //    float optimalPressure = 0;
    //    //
    //    foreach (float x in downPressuresFinal)
    //    {
    //        countVals = 0;
    //        foreach (float y in upPressuresFinal)
    //        {
    //            if (x == y)
    //            {
    //                float tempROM = upAnglesFinal[countVals] - downAnglesFinal[countVals];
    //                if (tempROM > maxROM)
    //                {
    //                    maxROM = tempROM;
    //                    optimalPressure = downAnglesFinal[countVals];
    //                }
    //            }
    //            countVals++;
    //        }
    //    }
    //    if (maxROM <= 0f)
    //    {
    //        int downLength = downAnglesFinal.Count - 1;
    //        int upLength = upAnglesFinal.Count - 1;
    //        maxROM = upAnglesFinal[upLength] - downAnglesFinal[downLength];
    //        if (downPressuresFinal[downLength] > upPressuresFinal[upLength])
    //        {
    //            optimalPressure = downPressuresFinal[downLength];
    //        }
    //        else
    //        {
    //            optimalPressure = upPressuresFinal[upLength];
    //        }
    //    }
    //    Debug.Log("VALS: " + countVals);
    //    Debug.Log(maxROM);
    //    Debug.Log(optimalPressure);
    //}

    public void AddToDownList(float angle, float pressure)
    {
        bool pressureExists = downListPressures.Contains(pressure);
        int anglesCount = downListAngles.Count - 1;
        int pressuresCount = downListPressures.Count - 1;
        if (pressureExists)
        {
            float existingAngle = downListAngles[anglesCount];
            if (angle < existingAngle)
            {
                downListAngles[anglesCount] = angle;
                downListPressures[pressuresCount] = pressure;
            }
        }
        else
        {
            downListAngles.Add(angle);
            downListPressures.Add(pressure);
        }
    }

    public void AddToUpList(float angle, float pressure)
    {
        bool pressureExists = upListPressures.Contains(pressure);
        int anglesCount = upListAngles.Count - 1;
        int pressuresCount = upListPressures.Count - 1;
        if (pressureExists)
        {
            float existingAngle = upListAngles[anglesCount];
            if (angle > existingAngle)
            {
                upListAngles[anglesCount] = angle;
                upListPressures[pressuresCount] = pressure;
            }
        }
        else
        {
            upListAngles.Add(angle);
            upListPressures.Add(pressure);
        }
    }

    public void GetROMAndPressure()
    {
        int count = 0;
        int index = 0;
        float ROM = 0;
        float optimalPressure = 0;
        float minROM = -10;
        float maxROM = 10;

        foreach (float x in downListPressures)
        {
            Debug.Log(x);
            count = 0;
            foreach (float y in upListPressures)
            {
                if (x == y)
                {
                    float tempROM = upListAngles[count] - downListAngles[index];
                    Debug.Log("tempROM: " + tempROM + " ROM: " + ROM);
                    if (tempROM > ROM)
                    {
                        ROM = tempROM;
                        optimalPressure = downListPressures[index];
                        maxROM = upListAngles[count];
                        minROM = downListAngles[index];
                    }
                }
                count++;
            }
            index++;
        }
        if (ROM <= 0f)
        {
            Debug.Log("ROM is 0");
            int downLength = downListAngles.Count - 1;
            int upLength = upListAngles.Count - 1;
            ROM = upListAngles[upLength] - downListAngles[downLength];
            minROM = upListAngles[upLength];
            maxROM = downListAngles[downLength];
            if (downListPressures[downLength] > upListPressures[upLength])
            {
                optimalPressure = downListPressures[downLength];
            }
            else
            {
                optimalPressure = upListPressures[upLength];
            }
        }
        //foreach (float x in upListAngles)
        //{
        //    Debug.Log(x);
        //}
        //foreach (float x in upListPressures)
        //{
        //    Debug.Log(x);
        //}
        //foreach (float x in downListAngles)
        //{
        //    Debug.Log(x);
        //}
        //foreach (float x in downListPressures)
        //{
        //    Debug.Log(x);
        //}
        //Debug.Log(ROM);
        minROM += Mathf.Abs(minROM * .1f);
        maxROM -= Mathf.Abs(maxROM * .1f);
        if (minROM >= maxROM)
        {
            minROM = -10;
            maxROM = 10;
        }
        //Debug.Log(minROM);
        //Debug.Log(maxROM);
        //Debug.Log(optimalPressure);
        //CHANGE THIS BACK
        /*
        if (PumpManager.GetSendData()) 
        {
            SendDynROMDataIntent(minROM, maxROM, optimalPressure);
        }
        */
    }
    [System.Serializable]
    public class UnityROM
    {
        public float maxDynRom;
        public float minDynRom;
        public float optimalPressure;

    }

    public UnityROM playerROM;

    public void SendDynROMDataIntent(float min, float max, float pressure)
    {

        playerROM.maxDynRom = max;
        playerROM.minDynRom = min;
        playerROM.optimalPressure = pressure;

        if (UnityPlayer == null || currentActivity == null)
        {
            UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        UnityROM data = playerROM;
        string dataJSON = JsonUtility.ToJson(data);
        Debug.Log("Sending : " + dataJSON);

        if (Application.platform != RuntimePlatform.Android) return;

        string action = "com.motusnova.mentorgames.UnityROMData";

        AndroidJavaObject reqIntent = new AndroidJavaObject("android.content.Intent");


        reqIntent.Call<AndroidJavaObject>("setAction", action);

        reqIntent.Call<AndroidJavaObject>("putExtra", "ROMData", dataJSON);
        reqIntent.Call<AndroidJavaObject>("putExtra", "GameID", "unknown");
        UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("sendBroadcast", reqIntent);
        reqIntent.Dispose();
    }
}
