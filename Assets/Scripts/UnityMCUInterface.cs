using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMCUInterface : MonoBehaviour
{
    private AndroidJavaClass unityClass;
    private AndroidJavaObject unityActivity;
    private AndroidJavaObject unityMCUInterface;
    private static UnityMCUInterface instance = null;

    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        /*
        // Insure only a single PumpManager exists for all scenes.
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        */

        // Don't destroy pump manager between scenes.
        //DontDestroyOnLoad(gameObject);
    }
        // Start is called before the first frame update
        void Start()
    {
        unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        unityMCUInterface = new AndroidJavaObject("com.motus.usbserialservice.UnityMCUInterface");

        if (unityMCUInterface != null)
        {
            Debug.Log("Got handle to Java UnityMCUInterface.  Initializing.");
            unityMCUInterface.Call("init", unityActivity);
            unityMCUInterface.Call("start");
//            unityMCUInterface.Call("sendStartRequest");
        }
    }
    void OnSceneLoad()
    {
            if (instance)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
    }
    // Update is called once per frame
    void Update()
    {
        //if (isConnected())
        //{
        //    //Debug.Log("Connected!");
        //    sendEnableCommsRequest(true);
        //    float cp = getCurrentPressure();
        //    Debug.Log("Current Pressure: " + cp);
        //}
        //else
        //{
        //    Debug.Log("Not Connected!");
        //}
    }

    // Checks to see if the connection thread in the android library is running.
    public bool isConnected()
    {
        if (unityMCUInterface != null)
        {
            return unityMCUInterface.Call<bool>("isConnected");
        }
        else
        {
            return false;
        }
    }

    private bool isCommsEnabled = false;
    // This function sends the command to the MCU to start/stop sending status updates.
    public void sendEnableCommsRequest(bool enabled)
    {
        if (unityMCUInterface != null)
        {
            if (isCommsEnabled != enabled) // Ensure one shot sending...
            {
                unityMCUInterface.Call("sendEnableCommsRequest", enabled);
                isCommsEnabled = enabled;
            }
        }
    }

    // This function gets the current pressure as read from the sensor
    // If it can't read the value it returns NaN which is an illegal pressure value.
    public float getCurrentPressure()
    {
        if (unityMCUInterface != null)
        {
            return unityMCUInterface.Call<float>("getCurrentPressure");
        }
        else
        {
            return Single.NaN;
        }
    }

    // This function gets the current MCU temperature as read from the MCU temperature sensor.
    // If it can't read the value the function returns NaN which is an illegal Temperature value.
    public float getCurrentTemperature()
    {
        if (unityMCUInterface != null)
        {
            return unityMCUInterface.Call<float>("getCurrentTemperature");
        }
        else
        {
            return Single.NaN;
        }
    }

    // Get the current panic state of the system.  True for panicked False for not.
    // If it can't read the Panic state it defaults to panicked.
    public bool isPanicState()
    {
        if (unityMCUInterface != null)
        {
            return unityMCUInterface.Call<bool>("isPanicState");
        }
        else
        {
            return true;
        }
    }

    // Get the current power state of the system.  True for powered False for not.
    // This should actually always return true.  If you see it return false there is an issue
    // with the system.
    public bool isPowerState()
    {
        if (unityMCUInterface != null)
        {
            return unityMCUInterface.Call<bool>("isPowerState");
        }
        else
        {
            return false;
        }
    }

    // Get the pressure set with setRequestedPressure
    // Returns NaN if there is an error.
    public float getRequestedPressure()
    {
        if (unityMCUInterface != null)
        {
            double ret = unityMCUInterface.Call<float>("getRequestedPressure");
            return (float)ret;
        }
        else
        {
            return Single.NaN;
        }
    }

    // Set the requested pressure for the system.
    // Use the getRequestedPressure function to verify the pressure
    // is set to what you requested, as if you try to set it to
    // an out of range value it won't change.
    public void setRequestedPressure(float pressure)
    {
        if (unityMCUInterface != null)
        {
            unityMCUInterface.Call("setRequestedPressure", pressure);
        }
    }

    // Use to clear the panic/power status.  
    //
    // Set the variable to true you want to clear, IE if you want to reset the panic flag set panic to true.
    public void clearPanicPower(bool panic, bool power)
    {
        if (unityMCUInterface != null)
        {
            unityMCUInterface.Call("clearPanicPower", panic, power);
        }
    }

    private void OnApplicationQuit()
    {
        if (unityMCUInterface != null)
        {
            unityMCUInterface.Call("stop");
        }
    }
}
