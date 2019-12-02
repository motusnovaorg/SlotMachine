using System.Collections;
using System.Collections.Generic;
using System.Data;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ludiq;
using Bolt;

public class PumpManagerOld : MonoBehaviour
{
    // Required java classes to talk to other Android apps.
    private AndroidJavaClass UnityPlayer;
    private AndroidJavaObject currentActivity;

    // Singleton jazz.
    public static PumpManagerOld instance = null;

    // Preinit.
    public float startPressure;
    //public GameObject TimeUI;
    public float minAngleScript;
    public float maxAngleScript;
    public float activeTime;//based on prescription
    //AndroidJavaObject currentActivity;
    public GameObject displayDebug;
    List<float> downListAngles;
    List<float> downListPressures;
    List<float> upListAngles;
    List<float> upListPressures;
    public static bool sendData;
    //public float pressure;
    void Awake()
    {
        // Insure only a single PumpManager exists for all scenes.
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            //Debug.Log("PumpManager");
            Destroy(gameObject);
        }
        //// Don't destroy pump manager between scenes.
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        //UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    }

    public void GameManagerInit()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("PumpManager");
            Destroy(gameObject);
        }

        // Don't destroy pump manager between scenes.
        DontDestroyOnLoad(gameObject);


        // Initializecache our pipeline to Android
        if (UnityPlayer == null || currentActivity == null)
        {
            UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            GetIntentData();//was using this to check to see if the if statement was being passed successfully
        }
        SendLogJSONIntent(JsonUtility.ToJson("Unity: Awake()"));
        if (intentData != null)
        {
            //startPressure = intentData.assistPressure;
            if (GetDisplayAROM())
            {
                displayDebug = GameObject.Find("DisplayDebug");
                displayDebug.SetActive(true); //show angle info
            }
            else
            {
                displayDebug = GameObject.Find("DisplayDebug");
                displayDebug.SetActive(false); //hide angle info
            }
        }
        minAngleScript = GetAROMMinInDeg();
        maxAngleScript = GetAROMMaxInDeg();
        if (minAngleScript == -10f && maxAngleScript == 10f)
        {
            sendData = true;
        }
        //activeTime = GetActiveTime();

        //Variables.Application.Set("maxAngle", maxAngleScript);
        //Variables.Application.Set("minAngle", minAngleScript);
        Variables.Application.Set("pressureRequested", startPressure);
        //Variables.Application.Set("activeTime", activeTime);

        Debug.Log("GameManagerInit");
    }

    [System.Serializable]
    public class UnityROM
    {
        public float maxDynRom;
        public float minDynRom;

    }

    public UnityROM playerROM;

    private float prevRequestedPressure = float.NaN;
    private bool isPumpHalted = false;
    private IntentData intentData;

    // Send android intent to MentorGames to pump to given pressure.
    // Send a 0 to stop assist completely.
    public void RequestPressure(float pressure)
    {
        if (UnityPlayer == null || currentActivity == null)
        {
            UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
        if (pressure == -1f)
        {
            pressure = startPressure;
        }
        else if (pressure < 0) { return; }

        // Only send intent if the requested pressure has actually changed.
        if (pressure == prevRequestedPressure) return;
        Debug.Log("RequestPressure(" + pressure + "): Called.");
        // Don't run this function if not on android.
        if (Application.platform != RuntimePlatform.Android) return;

        string action = "com.motusnova.mentorgames.UnityPressureRequest";

        AndroidJavaObject reqIntent = new AndroidJavaObject("android.content.Intent");
        Debug.Log("RequestPressure(" + pressure + "): Called.2");

        reqIntent.Call<AndroidJavaObject>("setAction", action);
        reqIntent.Call<AndroidJavaObject>("putExtra", "GameID", "unknown"); // TODO - Replace "unknown" with scene name
        reqIntent.Call<AndroidJavaObject>("putExtra", "pressure", pressure);
        Debug.Log("RequestPressure(" + pressure + "): Called.3");
        currentActivity.Call("sendBroadcast", reqIntent);
        Debug.Log("GettingPressure");

        // We actually just assume the call succeeded.
        // There is no way to be sure since this is an ASYNC request.
        isPumpHalted = false;

        // Get rid of our trash.
        reqIntent.Dispose();

        prevRequestedPressure = pressure;
    }

    // Send android intent to MentorGames to halt the pump at current pressure.
    public void HaltPump()
    {
        Debug.Log("HaltPump(): Called.");
        // Don't run this function if not on android.
        if (Application.platform != RuntimePlatform.Android) return;

        //Debug.Log("HaltPump CALLED");

        // Don't do anything if we were already called.
        // This keeps us from sending the intent when we don't need to.
        if (isPumpHalted) return;

        string action = "com.motusnova.mentorgames.UnityHaltPump";

        AndroidJavaObject reqIntent = new AndroidJavaObject("android.content.Intent");

        reqIntent.Call<AndroidJavaObject>("setAction", action);
        reqIntent.Call<AndroidJavaObject>("putExtra", "GameID", "unknown"); // TODO - replace unknown with scene name
        currentActivity.Call("sendBroadcast", reqIntent);

        // We actually just assume the call succeeded.
        // There is no way to be sure since this is an ASYNC request.
        isPumpHalted = true;

        // Get rid of our trash.
        reqIntent.Dispose();
    }

    // Is the pump halted?
    public bool IsPumpHalted()
    {
        return isPumpHalted;
    }

    private void GetIntentData()
    {
        //AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");

        bool hasExtra = intent.Call<bool>("hasExtra", "arguments");

        //Debug.Log("I CAN HAS INTENT EXTRAS? " + hasExtra);

        if (hasExtra)
        {
            AndroidJavaObject extras = intent.Call<AndroidJavaObject>("getExtras");

            string arguments = extras.Call<string>("getString", "arguments");
            Debug.Log("Extras: " + arguments);

            intentData = JsonUtility.FromJson<IntentData>(arguments);

            //intent.Call("removeExtra", "arguments");
        }

        //UnityPlayer.Dispose();
        //currentActivity.Dispose();
        intent.Dispose();
    }

    [System.Serializable]
    public class IntentData
    {
        public float aromMin;
        public float aromMax;
        public float offset;
        public float multiplier;
        public long playTime;
        public float maxPROMPressure;
        public float assistPressure;
        public string scene;
        public bool pumpToGoal;
        public bool isDemoMode;
        public bool isDisplayAROM;
    }

    public void SendLogJSONIntent(string logJSON)
    {
        //AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (Application.platform != RuntimePlatform.Android) return;

        Debug.Log("Sending LOG Data: " + logJSON);

        string action = "com.motusnova.mentorgames.UnityGameLogPatientData";

        AndroidJavaObject reqIntent = new AndroidJavaObject("android.content.Intent");

        reqIntent.Call<AndroidJavaObject>("setAction", action);
        reqIntent.Call<AndroidJavaObject>("putExtra", "GameID", "SpaceShoot");
        reqIntent.Call<AndroidJavaObject>("putExtra", "LogJSON", logJSON);
        currentActivity.Call("sendBroadcast", reqIntent);

        //UnityPlayer.Dispose();
        //currentActivity.Dispose();
        reqIntent.Dispose();
    }

    public float GetAROMMinInDeg()
    {
        if (intentData != null)
        {
            return intentData.aromMin;
        }
        else
        {
            // Keep this here, this is so when direct executing
            // this app for testing you get the full range of the
            // controller
            return -25f; // This is the controller max.
        }
    }

    public float GetAROMMaxInDeg()
    {
        if (intentData != null)
        {
            return intentData.aromMax;
        }
        else
        {
            Debug.Log("NULL");
            // Keep this here, this is so when direct executing
            // this app for testing you get the full range of the
            // controller
            return 55f;
        }
    }

    public bool GetDisplayAROM()
    {
        if (intentData != null)
        {
            return intentData.isDisplayAROM;
        }
        else
        {
            return true;
        }

    }

    public long GetActiveTime()
    {
        if (intentData != null)
        {
            return intentData.playTime;
        }
        else
        {
            return 600L;
        }
    }

    public void SendDynROMDataIntent()
    {
        Debug.Log("DYNAMIC");

        minAngleScript = (float)Variables.Application.Get("minAngle");
        maxAngleScript = (float)Variables.Application.Get("maxAngle");


        playerROM.maxDynRom = maxAngleScript;
        playerROM.minDynRom = minAngleScript;

        Debug.Log(maxAngleScript);

        UnityROM data = playerROM;
        string dataJSON = JsonUtility.ToJson(data);
        Debug.Log("Sending : " + dataJSON);

        if (Application.platform != RuntimePlatform.Android) return;

        string action = "com.motusnova.mentorgames.UnityROMData";

        AndroidJavaObject reqIntent = new AndroidJavaObject("android.content.Intent");


        reqIntent.Call<AndroidJavaObject>("setAction", action);

        reqIntent.Call<AndroidJavaObject>("putExtra", "ROMData", dataJSON);
        reqIntent.Call<AndroidJavaObject>("putExtra", "GameID", "unknown");
        currentActivity.Call("sendBroadcast", reqIntent);
        Debug.Log("DYNAMIC ROM DATA SENT");
        displayDebug.SetActive(false); //hide angle info
        //UnityPlayer.Dispose();
        //currentActivity.Dispose();
        reqIntent.Dispose();
    }

    public static bool GetSendData() { return sendData; }
}
