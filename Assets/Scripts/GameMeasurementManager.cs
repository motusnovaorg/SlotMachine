using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Ludiq;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class GameMeasurementManager : MonoBehaviour
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
        BrickBreaker
    };

    
    private game gameSwitch;
    private string deviceID;

    public TCPMessage.GameMeasurement gameMeasurement;
    public static GameMeasurementManager instance;

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
        gameMeasurement = new TCPMessage.GameMeasurement();

        gameMeasurement.session_id = Convert.ToInt64(PlayerPrefs.GetString("sessionID", "0"));
        deviceID = PlayerPrefs.GetString("deviceID", "0");
        deviceID = Regex.Replace(deviceID, "[a-zA-z]+", "");
        gameMeasurement.device_id = Convert.ToInt64(deviceID);

        switch (gameName)
        {
            case "Thermometer":

                gameMeasurement.game = 1;
                gameMeasurement.game_id = 1;

                gameSwitch = game.Thermometer;

                break;

            case "StrongMan":

                gameMeasurement.game = 2;
                gameMeasurement.game_id = 2;

                gameSwitch = game.Strongman;

                break;

            case "StrongManUp":

                gameMeasurement.game = 3;
                gameMeasurement.game_id = 3;

                gameSwitch = game.StrongmanUp;

                break;

            case "Spaceshoot":

                gameMeasurement.game = 4;
                gameMeasurement.game_id = 4;

                gameSwitch = game.Spaceshoot;

                break;

            case "Blocks":

                gameMeasurement.game = 5;
                gameMeasurement.game_id = 5;

                gameSwitch = game.Blocks;

                break;

            case "Golf":

                gameMeasurement.game = 6;
                gameMeasurement.game_id = 6;

                gameSwitch = game.Golf;

                break;

            case "Balloon":

                gameMeasurement.game = 7;
                gameMeasurement.game_id = 7;
                Debug.Log(gameMeasurement.game = 7);
                gameSwitch = game.Balloon;

                break;

            case "BrickBreaker":
                gameMeasurement.game = 8;
                gameMeasurement.game_id = 8;

                gameSwitch = game.BrickBreaker;

                break;

        }

    }

    public void UpdateDynamicROM(float min)
    {
        gameMeasurement.dynamic_rom_min = Convert.ToSingle(Variables.Saved.Get("minAngle"));
        //gameMeasurement.dynamic_rom_min = min;
        //Debug.Log("minangleTelem: " + min);

        gameMeasurement.dynamic_rom_max = Convert.ToSingle(Variables.Saved.Get("maxAngle"));
        //Debug.Log("TelemetrySent");
    }

    public void UpdateScore(float score)
    {
        gameMeasurement.score = Convert.ToInt32(score);
        //Debug.Log("min angle telem sending score: " + gameMeasurement.dynamic_rom_min);
        TCPMessage.GetInstance().Send(gameMeasurement);
        //Debug.Log("TelemetrySent");
    }

    public void UpdateLevel(short level)
    {
        gameMeasurement.level = level;
    }

    public void UpdateLevelBalloon(string levelName)
    {
        switch (levelName)
        {
            case "mountain":

                UpdateLevel(1);

                break;

            case "rest":

                UpdateLevel(2);

                break;

            case "boss":

                UpdateLevel(3);

                break;

            case "desert":

                UpdateLevel(4);

                break;
        }
    }

        public void UpdateLevelGolf(String sceneName)
    {
        switch (sceneName)
        {
            case "Hills1":

                UpdateLevel(1);

                break;

            case "Hills2":

                UpdateLevel(2);

                break;

            case "Hills3":

                UpdateLevel(3);

                break;

            case "Hills4":

                UpdateLevel(4);

                break;

            case "Hills5":

                UpdateLevel(5);

                break;

            case "Hills6":

                UpdateLevel(6);

                break;

            case "Hills7":

                UpdateLevel(7);

                break;

            case "Hills8":

                UpdateLevel(8);

                break;

            case "Hills9":

                UpdateLevel(9);

                break;

            case "Caves1":

                UpdateLevel(10);

                break;

            case "Caves2":

                UpdateLevel(11);

                break;

            case "Caves3":

                UpdateLevel(12);

                break;

            case "Caves4":

                UpdateLevel(13);

                break;

            case "Caves5":

                UpdateLevel(14);

                break;

            case "Caves6":

                UpdateLevel(15);

                break;

            case "Caves7":

                UpdateLevel(16);

                break;

            case "Caves8":

                UpdateLevel(17);

                break;

            case "Caves9":

                UpdateLevel(18);

                break;


        }
    }

    public void UpdateLevelBrickBreaker(string levelName)
    {
        switch(levelName)
        {
            case "Rest":
                UpdateLevel(1);
                break;

            case "Level1":
                UpdateLevel(2);
                break;

            case "Level2":
                UpdateLevel(3);
                break;

            case "Level3":
                UpdateLevel(4);
                break;

        }
    }

    public void UpdateGameTime(float time)
    {
        gameMeasurement.game_time = Convert.ToInt64(time);

    }

    public int GetScore()
    {
        return gameMeasurement.score;
    }
}
