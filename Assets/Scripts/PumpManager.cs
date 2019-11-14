using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpManager : MonoBehaviour
{
    // Required java classes to talk to other Android apps.
    private AndroidJavaClass UnityPlayer;
    private AndroidJavaObject CurrentActivity;

    // Tracking variables.
    private float requestedPressure;
    private float haltAngleUnity;
    private bool isPumpHalted = false;
    private bool isPumpToMidPoint = false;
    private bool isPumpToMidPointComplete = false;
    private bool isPumpedToPressureOrAngle = false;
    private bool isPumpedToPressureOrAngleComplete = false;

    // Singleton jazz.
    public static PumpManager instance = null;
    
    private GameObject broadcastReceiverGO;

    private GameObject unityMCUInterfaceGO;
    private UnityMCUInterface mcuInterface;

    // Is this a C41 unit?
    private bool isC41 = true;

    // Preinit.
    void Awake()
    {
        
        // Insure only a single PumpManager exists for all scenes.
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        // Don't destroy pump manager between scenes.
        DontDestroyOnLoad(gameObject);

        // Initialize/cache our pipeline to Android
        UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        CurrentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        unityMCUInterfaceGO = GameObject.Find("UnityMCUInterfaceGO");
        mcuInterface = unityMCUInterfaceGO.GetComponent<UnityMCUInterface>();

        Debug.Log("Got MCUInterface? " + mcuInterface);
        isC41 = true;
        string devModel = SystemInfo.deviceModel;
        if (devModel.Contains("freescale"))
        {
            isC41 = true;
        }

    }

    // Use this for initialization
    void Start()
    {
        string devModel = SystemInfo.deviceModel;

        Debug.Log("DEVICE: " + devModel);
        RequestPressure(0);
    }

    private float pressureRequestTimer = 0.5f; // Only make requests every 100ms or so.
    // Update is called once per frame
    void Update()
    {
        // I really don't think I like this here...
        if (isC41 && mcuInterface.isConnected())
        {
            //I think I can remove this, because this is already done in the pump manager.
            //mcuInterface.sendEnableCommsRequest(true);
        }
    }

    public void PumpToPressureOrUnityAngle(float requestedPressure, string gameManagerState)
    {
        SetRequestedPressure(requestedPressure, gameManagerState);
        isPumpedToPressureOrAngle = true;
        isPumpedToPressureOrAngleComplete = false;
    }

    public bool IsPumpToPressureOrAngleComplete()
    {
        if (isPumpedToPressureOrAngle)
        {
            return isPumpedToPressureOrAngleComplete;
        }
        else
        {
            return false;
        }
    }
    private void DoPumpToPressureOrAngle()
    {
        if (haltAngleUnity != 0)
        {
            float pos = UnityEngine.Input.GetAxis("Horizontal");
            float UNITY0_5DEG = 0.0125f;  // 0.5 deg in Unity units

            // If we are within 0.5deg of target send halt.
            if ((pos + UNITY0_5DEG) > haltAngleUnity)
            {
                HaltPump();
                isPumpedToPressureOrAngleComplete = true;
            }
        }
    }
    
    public void PumpToMAXPromPressure(float maxPressure, string gameManagerState)
    {
        Debug.Log("PumpToMaxPROM()");
        isPumpedToPressureOrAngle = false;
        SetRequestedPressure(maxPressure, gameManagerState);
    }
    
    
    public void PumpToAssistPressure(float assistPressure)
    {
        isPumpedToPressureOrAngle = false;
        RequestPressure(assistPressure);
    }
    

    // Send message to MentorGames to set the pressure.
    
    public void SetRequestedPressure(float pressure, string gameManagerState)
    {
        Debug.Log("SetRequestedPressure(" + pressure + ")");
        requestedPressure = pressure;

        
        if (gameManagerState == "GamePlay")
        {
            RequestPressure(pressure);
        }
        
    }
    /*

    // Remove x PSI from requested pressure
    public void PressureDown(float by)
    {
        requestedPressure -= by;
        if (requestedPressure < 0)
        {
            requestedPressure = 0;
        }
        SetRequestedPressure(requestedPressure);
    }

    // Add x PSI from requested pressure
    public void PressureUp(float by)
    {
        float maxPressure = GameManager.instance.GetMAXPRomPressure();

        requestedPressure += by;
        if (requestedPressure > maxPressure)
        {
            requestedPressure = maxPressure;
        }
        SetRequestedPressure(requestedPressure);
    }
    */
    public float GetRequestedPressure()
    {
        if (isC41)
        {
            //return mcuInterface.getRequestedPressure();
            return mcuInterface.getCurrentPressure();
        }
        else
            return requestedPressure;
    }

    // Pump to previously requested pressure
    public void PumpToRequestedPressure()
    {
        Debug.Log("PumpToRequestedPRessure(): " + requestedPressure);
        RequestPressure(requestedPressure);
    }

    private float prevRequestedPressure = -100f;

    // Send android intent to MentorGames to pump to given pressure.
    // Send a 0 to stop assist completely.
    public void RequestPressure(float pressure)
    {
        // Only send intent if the requested pressure has actually changed.
        //if (pressure == prevRequestedPressure) return;
        Debug.Log("RequestPressure(" + pressure + "): Called.");
        // Don't run this function if not on android.
        if (Application.platform != RuntimePlatform.Android) return;

        Debug.Log("Requesting pressure in android");

        if (isC41)
        {
            Debug.Log("Requesting C41 Pressure: " + pressure);
            // Sending twice right now due to a bug in firmware.
            mcuInterface.setRequestedPressure(pressure);
            mcuInterface.setRequestedPressure(pressure);
            isPumpHalted = false;
            prevRequestedPressure = pressure;

            return;
        }
        /*
        else
        {

            string action = "com.motusnova.mentorgames.UnityPressureRequest";

            AndroidJavaObject reqIntent = new AndroidJavaObject("android.content.Intent");

            reqIntent.Call<AndroidJavaObject>("setAction", action);
            reqIntent.Call<AndroidJavaObject>("putExtra", "GameID", GameManager.instance.GetSceneName());
            reqIntent.Call<AndroidJavaObject>("putExtra", "pressure", pressure);
            CurrentActivity.Call("sendBroadcast", reqIntent);

            // We actually just assume the call succeeded.
            // There is no way to be sure since this is an ASYNC request.
            isPumpHalted = false;

            // Get rid of our trash.
            reqIntent.Dispose();

            prevRequestedPressure = pressure;
        }
        */
    }

    public void PressureUp(float additionalPressure, float maxPressure, string gameManagerState)
    {
        float currentPressure = mcuInterface.getCurrentPressure() + additionalPressure;
        if (currentPressure > maxPressure)
        {
            SetRequestedPressure(currentPressure, gameManagerState);
        }
        
    }
    public void PressureDown(float decrementAmount, string gameManagerState)
    {
        float currentPressure = mcuInterface.getCurrentPressure();
        if (decrementAmount < 0)
        {
            currentPressure = currentPressure + decrementAmount;
        }
        else
        {
            currentPressure = currentPressure - decrementAmount;
        }
        if(currentPressure < 0)
        {
            currentPressure = 0;
        }
        SetRequestedPressure(currentPressure, gameManagerState);
        
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
        //if (isPumpHalted) return;

        if (isC41)
        {
            // Sending twice right now due to a bug in firmware.
            mcuInterface.setRequestedPressure(0);
            mcuInterface.setRequestedPressure(0);

            isPumpHalted = true;

            return;
        }
        /*
        else
        {
            string action = "com.motusnova.mentorgames.UnityHaltPump";

            AndroidJavaObject reqIntent = new AndroidJavaObject("android.content.Intent");

            reqIntent.Call<AndroidJavaObject>("setAction", action);
            reqIntent.Call<AndroidJavaObject>("putExtra", "GameID", GameManager.instance.GetSceneName());
            CurrentActivity.Call("sendBroadcast", reqIntent);

            // We actually just assume the call succeeded.
            // There is no way to be sure since this is an ASYNC request.
            isPumpHalted = true;

            // Get rid of our trash.
            reqIntent.Dispose();
        }
        */
    }

    // Is the pump halted?
    public bool IsPumpHalted()
    {
        return isPumpHalted;
    }

    public bool IsRequestedPressureReached()
    {
        /*
        if (broadcastReceiver != null)
        {
            return broadcastReceiver.IsTargetPressureReached();
        }
        else
        {
            return true;
        }
        */
        return true;
    }
}