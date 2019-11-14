using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class GameDeviceMeasurement : MonoBehaviour
{
    private enum game
    {
        Spaceshoot,
        Blocks,
        Golf,
        Balloon,
        Thermometer,
        Strongman,
        StrongmanUp,
        BrickBreaker,
        Assessment
    };

    private game gameSwitch;
    public static GameDeviceMeasurement instance;
    private GameObject gameManagerGO;
    //private GameManager gameManager;

    //public GameManager.GameManagerState managerState;

    public static byte patient_activity = 1;
    public static bool gameActive = false;
    public static bool gameStart = true;
    private string deviceID;

    public TCPMessage.DeviceMeasurement deviceMeasurement;

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
    }

    public void SetGame(string gameName)
    {
        gameManagerGO = GameObject.Find("GameManagerGO");
        //gameManager = gameManagerGO.GetComponent<GameManager>();

        deviceMeasurement = new TCPMessage.DeviceMeasurement();
        deviceMeasurement.session_id = Convert.ToInt64(PlayerPrefs.GetString("sessionID", "0"));
        
        deviceID = PlayerPrefs.GetString("deviceID", "0");
        deviceID = Regex.Replace(deviceID, "[a-zA-z]+", "");
        deviceMeasurement.device_id = Convert.ToInt64(deviceID);
        Debug.Log(gameName);

        switch (gameName)
        {
            case "Thermometer":

                deviceMeasurement.game = 1;
                deviceMeasurement.game_id = 1;

                gameSwitch = game.Thermometer;

                break;

            case "StrongMan":

                deviceMeasurement.game = 2;
                deviceMeasurement.game_id = 2;

                gameSwitch = game.Strongman;

                break;

            case "StrongManUp":

                deviceMeasurement.game = 3;
                deviceMeasurement.game_id = 3;

                gameSwitch = game.StrongmanUp;

                break;

            case "Spaceshoot":

                deviceMeasurement.game = 4;
                deviceMeasurement.game_id = 4;

                gameSwitch = game.Spaceshoot;

                break;

            case "Blocks":

                deviceMeasurement.game = 5;
                deviceMeasurement.game_id = 5;

                gameSwitch = game.Blocks;

                break;

            case "Golf":

                deviceMeasurement.game = 6;
                deviceMeasurement.game_id = 6;

                gameSwitch = game.Golf;

                break;

            case "Balloon":

                deviceMeasurement.game = 7;
                deviceMeasurement.game_id = 7;
                gameSwitch = game.Balloon;

                break;

            case "BrickBreaker":
                deviceMeasurement.game = 8;
                deviceMeasurement.game_id = 8;

                gameSwitch = game.BrickBreaker;
                break;
            
            case "Assessment":
                deviceMeasurement.game = 9;
                deviceMeasurement.game_id = 9;

                gameSwitch = game.BrickBreaker;
                break;
        }

    }



    public void UpdateGameDeviceMeasurement(float pos)
    {

        //float pos = Input.GetAxis("Horizontal");
        deviceMeasurement.angle = pos;
        //Variables.Scene(SceneManager.GetActiveScene()).Get("patient_acvitiy");
        deviceMeasurement.patient_activity = patient_activity;
        //Debug.Log(patient_activity);
        if (gameActive)
        {
            //Debug.Log("ANGLE");
            TCPMessage.GetInstance().Send(deviceMeasurement);
        }
    }

    public void UpdateGameDeviceMeasurementState(String gameState)
    {
        Debug.Log("CHANGE STATE");
        switch (gameState)
        {
            case "Gameplay":

                if (gameStart)
                {
                    patient_activity = 2;

                    deviceMeasurement.patient_activity = patient_activity;
                    deviceMeasurement.device_activity = 2;
                    deviceMeasurement.active_flag = false;

                    TCPMessage.GetInstance().Send(deviceMeasurement);
                    gameStart = false;

                }

                deviceMeasurement.active_flag = true;
                deviceMeasurement.device_activity = 1;
                patient_activity = 1;

                gameActive = true;

                break;

            case "Pause":

                if (gameActive)
                {
                    patient_activity = 2;

                    deviceMeasurement.patient_activity = patient_activity;
                    deviceMeasurement.device_activity = 7;
                    deviceMeasurement.active_flag = false;

                    TCPMessage.GetInstance().Send(deviceMeasurement);

                    gameActive = false;
                }
                break;

            case "Game Over":

                //Send game over telemetry data.
                deviceMeasurement.device_activity = 10;
                deviceMeasurement.patient_activity = 4;
                deviceMeasurement.active_flag = false;

                TCPMessage.GetInstance().Send(deviceMeasurement);

                gameActive = false;

                break;

            case "Game Quit":

                //Send game quit telemetry data.
                deviceMeasurement.device_activity = 8;
                deviceMeasurement.patient_activity = 5;
                deviceMeasurement.active_flag = false;

                TCPMessage.GetInstance().Send(deviceMeasurement);

                gameActive = false;

                break;

            case "Level Transition":
                Debug.Log("TRANSITION");
                deviceMeasurement.device_activity = 9;
                deviceMeasurement.patient_activity = 1;
                deviceMeasurement.active_flag = true;

                TCPMessage.GetInstance().Send(deviceMeasurement);

                gameActive = true;

                break;

            case "Boss Appears":
                deviceMeasurement.device_activity = 5;
                deviceMeasurement.patient_activity = 1;
                deviceMeasurement.active_flag = true;

                TCPMessage.GetInstance().Send(deviceMeasurement);

                gameActive = true;

                break;

        }
    }

    //In the original Unity-Telemetry, all of the game-specific UpdateManagerState
    //Functions are combined into one with a switch.
    public void UpdateBalloonManagerState(string state)
    {
        switch (state)
        {
            case "Level":
                patient_activity = 1;

                Debug.Log("SENDING Active");

                break;

            case "Rest":
                Debug.Log("SENDING REST");

                patient_activity = 3;

                break;
        }
    }

    //In the original Unity-Telemetry, all of the game-specific UpdateManagerState
    //Functions are combined into one with a switch.
    public void UpdateBrickBreakerManagerState(string state)
    {
        switch (state)
        {
            case "Level":
                patient_activity = 1;

                break;

            case "Rest":

                patient_activity = 3;

                break;
        }
    }

    public void UpdateThermometerManagerState(string thermometerState)
    {
        switch (thermometerState)
        {
            case "Start":

                patient_activity = 3;

                break;

            case "Active":

                patient_activity = 1;

                break;

            case "Rest":

                patient_activity = 3;

                break;

            case "Stretch":

                break;

            case "Done":

                patient_activity = 3;

                break;

        }
    }

    public void UpdateStrongManManagerState(string strongmanState)
    {
        switch (strongmanState)
        {
            case "Init":

                patient_activity = 3;

                break;

            case "Start":

                patient_activity = 3;

                break;

            case "Down":

                patient_activity = 1;

                break;

            case "Up":

                patient_activity = 1;

                break;

            case "Rest":

                patient_activity = 3;

                break;

            case "Goal":

                patient_activity = 3;

                break;

            case "Stretch":

                patient_activity = 1;

                break;
        }
    }

    public void UpdateGameSpecificState(string gameState)
    {

        string gameName = ByteToGameString(deviceMeasurement.game);

        if (gameName.Equals("StrongManUp"))
        {
            gameName = "StrongMan";
        }

        switch (gameName)
        {
            case "Thermometer":

                switch (gameState)
                {
                    case "Start":

                        patient_activity = 3;

                        break;

                    case "Active":

                        patient_activity = 1;

                        break;

                    case "Rest":

                        patient_activity = 3;

                        break;

                    case "Stretch":

                        break;

                    case "Done":

                        patient_activity = 3;

                        break;

                }

                break;

            case "Strongman":

                switch (gameState)
                {
                    case "Init":

                        patient_activity = 3;

                        break;

                    case "Start":

                        patient_activity = 3;

                        break;

                    case "Down":

                        patient_activity = 1;

                        break;

                    case "Up":

                        patient_activity = 1;

                        break;

                    case "Rest":

                        patient_activity = 3;

                        break;

                    case "Goal":

                        patient_activity = 3;

                        break;

                    case "Stretch":

                        patient_activity = 1;

                        break;
                }

                break;

            case "Golf":

                switch (gameState)
                {
                    case "Aiming":

                        patient_activity = 1;

                        break;

                    case "Charging":

                        patient_activity = 1;

                        break;

                    case "Motion":

                        patient_activity = 3;

                        break;

                }

                break;

            case "BrickBreaker":

                switch(gameState)
                {
                    case "Rest":

                        patient_activity = 3;

                        break;

                    default:
                        patient_activity = 1;
                        break;

                }

                break;
        }
    }

    public String ByteToGameString(byte game)
    {
        String gameName = "";
        switch (game)
        {
            case 1:

                gameName = "Thermometer";

                break;

            case 2:

                gameName = "StrongMan";

                break;

            case 3:

                gameName = "StrongManUp";

                break;

            case 4:

                gameName = "Spaceshoot";

                break;

            case 5:

                gameName = "Blocks";

                break;

            case 6:

                gameName = "Golf";

                break;

            case 7:

                gameName = "Balloon";

                break;

            case 8:
                gameName = "BrickBreaker";
                break;

            case 9:
                gameName = "BrickBreaker";
                break;

        }

        return gameName;

    }
}

