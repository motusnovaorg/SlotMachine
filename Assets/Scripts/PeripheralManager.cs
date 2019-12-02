
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Bolt;

public class PeripheralManager : MonoBehaviour
{
    // Required java classes to talk to other Android apps.
    private AndroidJavaClass UnityPlayer;
    private AndroidJavaObject CurrentActivity;

    public static PeripheralManager instance;
    private bool isPeripheralConnected = true;
    private bool disconnectLaunched = false;
    private bool panicLaunched = false;
    private bool isAndroid = true;
    private bool previousPowerStateOn = false;
    private bool powerStateOn = true;
    private bool panicStateOn = false;
    private GameObject gameManager;
    private int timeCounter = 1;

    private GameObject unityMCUInterfaceGO;
    private UnityMCUInterface mcuInterface;

    private void Awake()
    {

        // Insure only a single instance exists
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        //Debug.Log("DeviceMeasurementAwake");
        // Don't destroy between scenes.
        DontDestroyOnLoad(gameObject);
        /*
        UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        CurrentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        */

    }
    private void OnSceneLoad()
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
    private void Start()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            isAndroid = false;
        }

        Debug.Log("Got MCUInterface? " + mcuInterface);
        unityMCUInterfaceGO = GameObject.Find("UnityMCUInterfaceGO");
        mcuInterface = unityMCUInterfaceGO.GetComponent<UnityMCUInterface>();
        if (mcuInterface.isConnected())
        {
            mcuInterface.sendEnableCommsRequest(true);
        }



    }

    public void FixedUpdate()
    {

        if (isAndroid)
        {
            if (mcuInterface.isConnected())
            {
                mcuInterface.sendEnableCommsRequest(true);
            }
            powerStateOn = mcuInterface.isPowerState();
            if (powerStateOn)
            {
                if (!previousPowerStateOn)
                {
                    ClearPanic();
                    Debug.Log("Power just got turned on.");
                }

                if (timeCounter > 0)
                {
                    panicStateOn = mcuInterface.isPanicState();
                    if (panicStateOn && !panicLaunched)
                    {
                        if (SceneManager.GetActiveScene().name != "PanicState")
                        {
                            SceneManager.LoadScene("PanicState");
                            panicLaunched = true;
                        }
                        else
                        {
                            panicLaunched = false;
                        }

                    }
                    else
                    {
                        if (!disconnectLaunched)
                        {
                            if (!GetIsPeripheralConnected())
                            {
                                mcuInterface.setRequestedPressure(0);
                                SceneManager.LoadScene("PeripheralDisconnect");
                                
                                disconnectLaunched = true;
                            }
                        }
                        else
                        {
                            if (GetIsPeripheralConnected())
                            {
                                SceneManager.LoadSceneAsync("SplashScene");
                                disconnectLaunched = false;
                            }
                        }
                    }
                }
                else
                {
                    timeCounter++;
                }
            }
            else
            {
                timeCounter = -90;
                panicStateOn = false;
            }

            previousPowerStateOn = powerStateOn;
        }
    }

    public void SetPanicLaunched(bool panicLaunched)
    {
        this.panicLaunched = panicLaunched;
    }

    /*
    public void FixedUpdate()
    {
        if (isAndroid)
        {
            if (!disconnectLaunched)
            {
                if (!GetIsPeripheralConnected())
                {
                    
                    try
                    {
                        gameManager = GameObject.Find("GameManager");
                        VideoPlayer videoPlayer = gameManager.GetComponent<VideoPlayer>();
                        videoPlayer.Stop();
                    }
                    catch(Exception e)
                    {
                        Debug.Log("Could not find video player object.");
                        Debug.Log(e);
                    }
                    
                    SceneManager.LoadScene("PeripheralDisconnect");
                    disconnectLaunched = true;
                }
            }
            else
            {
                if (GetIsPeripheralConnected())
                {
                    SceneManager.LoadScene("MenuScene");
                    disconnectLaunched = false;
                }
            }
        }

    }
    */
    public bool GetIsPeripheralConnected()
    {
        string[] joystickNames = Input.GetJoystickNames();
        if (joystickNames.Length == 0)
        {
            isPeripheralConnected = false;
        }
        else
        {
            int counter = 0;
            foreach (string joystick in joystickNames)
            {
                if (joystick.Contains("Motus"))
                {
                    isPeripheralConnected = true;
                    break;
                }
                counter++;

            }
            if (counter == joystickNames.Length)
            {
                isPeripheralConnected = false;
            }
        }
        return isPeripheralConnected;

    }

    public void ClearPanic()
    {
        mcuInterface.clearPanicPower(true, false);

    }
    public void ClearPower()
    {
        mcuInterface.clearPanicPower(false, true);

    }

}
