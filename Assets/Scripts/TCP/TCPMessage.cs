using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TCPMessage : MonoBehaviour
{
    #region private members
    public static TCPMessage instance;
    #endregion

    [System.Serializable]
    public class TCPMessageData
    {
        public long time_stamp;
        public string topic;
        public string message;
        public int qos;
    }

    [System.Serializable]
    public class DeviceMeasurement
    {
        public float angle;
        public float pressure;
        public bool pump_state;
        public byte valve_state;
        public long client_timestamp;
        public long device_id;
        public long session_id;
        public byte device_activity;
        public byte patient_activity;
        public short device_activity_state;
        public short patient_activity_state;
        public string unity_log;
        public bool active_flag;
        public int ip_address;
        public bool success_cycle;
        public byte game;
        public byte game_id;
        public float acceleration_x;
        public float acceleration_y;
        public float acceleration_z;
        public float rotation_x;
        public float rotation_y;
        public float rotation_z;
        public float success_cycle_max_rom;
        public float success_cycle_min_rom;
    }

    [System.Serializable]
    public class GameMeasurement
    {
        public long timestamp;
        public float avatar_position_x;
        public float avatar_position_y;
        public float avatar_position_z;
        public float avatar_velocity_x;
        public float avatar_velocity_y;
        public float avatar_velocity_z;
        public float avatar_acceleration_x;
        public float avatar_acceleration_y;
        public float avatar_acceleration_z;
        public byte game;
        public byte game_id;
        public int score;
        public long game_time;
        public short level;
        public float dynamic_rom_min;
        public float dynamic_rom_max;
        public long device_id;
        public long session_id;
    }

    private void Awake()
    {
        // Ensure only a single instance exists
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // Don't destroy between scenes.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    public void Send(TCPMessageData data, String client)
    {
        switch (client)
        {
            case "Galileo":
                GalileoTCPClient.GetInstance().SendMessage(data);
                break;

            case "Telemetry":
                TelemetryTCPClient.GetInstance().SendMessage(data);
                break;

            case "Updater":
                UpdaterTCPClient.GetInstance().SendMessage(data);
                break;
        }
            
        
    }

    public void SendMessage(string topic, string message, string client)
    {
        TCPMessageData toSend = new TCPMessageData();

        toSend.time_stamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        toSend.topic = topic;
        toSend.message = message;
        toSend.qos = 1; // Send at least once

        Send(toSend, client);
    }
    
    public void Send(DeviceMeasurement data)
    {
        data.client_timestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

        SendMessage("DeviceMeasurement", JsonUtility.ToJson(data), "Telemetry");
    }

    public void Send(GameMeasurement data)
    {
        data.timestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

        SendMessage("GameMeasurement", JsonUtility.ToJson(data), "Telemetry");
    }

    public static TCPMessage GetInstance()
    {
        return instance;
    }
}
