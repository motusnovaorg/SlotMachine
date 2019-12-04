using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Ludiq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

public class PatientManager : MonoBehaviour
{
	private Patient patient;
	private Prescription prescription;
    private Device device;
    public static PatientManager instance;
    private Text userNameText;
    private bool isPeripheralConnected = true;
    private List<Game> games;
    private long sessionID;
    public string deviceID;
    public TCPMessage tCPMessage;
    public List<String> scenesInProject;
    public string previousScene = "";

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
		// Don't destroy between scenes.
		DontDestroyOnLoad(gameObject);   
	}

    private void Start()
    {
        Variables.Saved.Set("displayAROM", true);
        string patientString = "{\"anonId\":\"DJ Russ Sr\",\"assistPressure\":10.5,\"assistPressureHysteresis\":0.0,\"device\":\"https://one.motusnova.com/api/devices/MPC40015/\",\"maxActive\":0.0,\"maxInitialAngle\":10.0,\"minActive\":0.0,\"minInitialAngle\":-10.0,\"notes\":\"Not actually David Wu\u0027s device.\",\"setInitialROM\":false,\"url\":\"https://one.motusnova.com/api/patients/DJ%20Russ%20Sr/\"}";
        string prescriptionString = "{\"description\":\"betaTelemetry\",\"endDate\":\"2021-08-30T15:21:52.000Z\",\"isActive\":true,\"linkedGameSettings\":[{\"activeTime\":120,\"cycles\":0,\"description\":\"Thermometer for 2 minute(s)\",\"gameId\":\"com.motus.thermometer\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/76d152f9-555d-4af0-a14d-9fe948e61795/\",\"uuid\":\"76d152f9-555d-4af0-a14d-9fe948e61795\"},{\"activeTime\":600,\"cycles\":0,\"description\":\"Slot Machine for 5 minute(s)\",\"gameId\":\"com.motus.slotmachine\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one\",\"uuid\":\"76\"},{\"activeTime\":600,\"cycles\":0,\"description\":\"BrickBreaker for 5 minute(s)\",\"gameId\":\"com.motus.brickbreaker\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one/\",\"uuid\":\"76d15\"},{\"activeTime\":1200,\"cycles\":0,\"description\":\"Balloon for 20 minute(s)\",\"gameId\":\"com.motus.balloonrider\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/76d152f9-555d-4af0-a14a-9fe948e61795/\",\"uuid\":\"76d152f9-555d-4af0-a14a-9fe948e61795\"},{\"activeTime\":180,\"cycles\":0,\"description\":\"Strongman for 3 minute(s)\",\"gameId\":\"com.motus.strongman\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/d5f81fc3-f903-40b2-a3c6-ce6735d8332c/\",\"uuid\":\"d5f81fc3-f903-40b2-a3c6-ce6735d8332c\"},{\"activeTime\":180,\"cycles\":0,\"description\":\"Strongmanup for 3 minute(s)\",\"gameId\":\"com.motus.strongmanup\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/2763bbca-bf06-437c-8e1d-b6783e0df9a5/\",\"uuid\":\"2763bbca-bf06-437c-8e1d-b6783e0df9a5\"},{\"activeTime\":900,\"cycles\":0,\"description\":\"Golf for 15 minute(s)\",\"gameId\":\"com.motus.golf\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/a9b7c089-9264-4cf6-baf9-787adc07eb8c/\",\"uuid\":\"a9b7c089-9264-4cf6-baf9-787adc07eb8c\"},{\"activeTime\":90,\"cycles\":0,\"description\":\"Spaceshoot for 15 minute(s)\",\"gameId\":\"com.motus.spaceshoot\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/6d0770b9-f352-4f53-9e52-c4cd2390296d/\",\"uuid\":\"6d0770b9-f352-4f53-9e52-c4cd2390296d\"},{\"activeTime\":3600,\"cycles\":0,\"description\":\"Blocks for 60 minute(s)\",\"gameId\":\"com.motus.blocks\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/c954b1df-8500-416c-bfae-d07a6075f374/\",\"uuid\":\"c954b1df-8500-416c-bfae-d07a6075f374\"}],\"maxActive\":10.0,\"minActive\":0.0,\"notes\":\"\",\"startDate\":\"2019-10-15T15:15:44.000Z\",\"url\":\"https://one.motusnova.com/api/prescriptions/c8a44dfe-6c97-4c0a-8e38-3f169b357378/\",\"user\":\"https://one.motusnova.com/api/users/parth.patel/\",\"uuid\":\"c8a44dfe-6c97-4c0a-8e38-3f169b357378\"}";
        string deviceString = "{\"createDate\":\"2019-07-03T14:37:42.016979Z\",\"deployed\":false,\"lastActiveDate\":\"2019-10-31T14:32:03.500195Z\",\"notes\":\"\",\"patient\":\"https://one.motusnova.com/api/patients/MPC40020/\",\"serial\":\"https://one.motusnova.com/api/devices/MPC40020/\"}";

        string androidPackageVersionsMessage = "[\"com.motusnova.galileo: 1.0.0\",\"com.motusnova.telemetry: \",\"com.motus.mnupdater: 1.01\",\"com.motus.mnlocker: 1.0\"]";

        

        SetDevice(deviceString);
        SetPatient(patientString);
        SetPrescription(prescriptionString);
        LoadPatient();
        LoadDevice();
        BuildGameList();
        //SetDeviceURL();
        UpdateDynamicROM();
        
        /*
        //These are left as examples for launching Android apps.
        LaunchAndroidApp("com.motusnova.galileo");
        LaunchAndroidApp("com.motusnova.telemetry");
        LaunchAndroidApp("com.motus.locker");
        LaunchAndroidApp("com.motus.updater");
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
        

        string patientString = "{\"anonId\":\"DJ Russ Sr\",\"assistPressure\":10.5,\"assistPressureHysteresis\":0.0,\"device\":\"https://one.motusnova.com/api/devices/MPC40015/\",\"maxActive\":0.0,\"maxInitialAngle\":10.0,\"minActive\":0.0,\"minInitialAngle\":-10.0,\"notes\":\"Not actually David Wu\u0027s device.\",\"setInitialROM\":false,\"url\":\"https://one.motusnova.com/api/patients/DJ%20Russ%20Sr/\"}";
        string prescriptionString = "{\"description\":\"betaTelemetry\",\"endDate\":\"2021-08-30T15:21:52.000Z\",\"isActive\":true,\"linkedGameSettings\":[{\"activeTime\":120,\"cycles\":0,\"description\":\"Thermometer for 2 minute(s)\",\"gameId\":\"com.motus.thermometer\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/76d152f9-555d-4af0-a14d-9fe948e61795/\",\"uuid\":\"76d152f9-555d-4af0-a14d-9fe948e61795\"},{\"activeTime\":600,\"cycles\":0,\"description\":\"Slot Machine for 5 minute(s)\",\"gameId\":\"com.motus.slotmachine\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one\",\"uuid\":\"76\"},{\"activeTime\":600,\"cycles\":0,\"description\":\"BrickBreaker for 5 minute(s)\",\"gameId\":\"com.motus.brickbreaker\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one/\",\"uuid\":\"76d15\"},{\"activeTime\":1200,\"cycles\":0,\"description\":\"Balloon for 20 minute(s)\",\"gameId\":\"com.motus.balloonrider\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/76d152f9-555d-4af0-a14a-9fe948e61795/\",\"uuid\":\"76d152f9-555d-4af0-a14a-9fe948e61795\"},{\"activeTime\":180,\"cycles\":0,\"description\":\"Strongman for 3 minute(s)\",\"gameId\":\"com.motus.strongman\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/d5f81fc3-f903-40b2-a3c6-ce6735d8332c/\",\"uuid\":\"d5f81fc3-f903-40b2-a3c6-ce6735d8332c\"},{\"activeTime\":180,\"cycles\":0,\"description\":\"Strongmanup for 3 minute(s)\",\"gameId\":\"com.motus.strongmanup\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/2763bbca-bf06-437c-8e1d-b6783e0df9a5/\",\"uuid\":\"2763bbca-bf06-437c-8e1d-b6783e0df9a5\"},{\"activeTime\":900,\"cycles\":0,\"description\":\"Golf for 15 minute(s)\",\"gameId\":\"com.motus.golf\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/a9b7c089-9264-4cf6-baf9-787adc07eb8c/\",\"uuid\":\"a9b7c089-9264-4cf6-baf9-787adc07eb8c\"},{\"activeTime\":90,\"cycles\":0,\"description\":\"Spaceshoot for 15 minute(s)\",\"gameId\":\"com.motus.spaceshoot\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/6d0770b9-f352-4f53-9e52-c4cd2390296d/\",\"uuid\":\"6d0770b9-f352-4f53-9e52-c4cd2390296d\"},{\"activeTime\":3600,\"cycles\":0,\"description\":\"Blocks for 60 minute(s)\",\"gameId\":\"com.motus.blocks\",\"maxOffset\":0.0,\"minOffset\":0.0,\"restTime\":0,\"url\":\"https://one.motusnova.com/api/gamesettings/c954b1df-8500-416c-bfae-d07a6075f374/\",\"uuid\":\"c954b1df-8500-416c-bfae-d07a6075f374\"}],\"maxActive\":10.0,\"minActive\":0.0,\"notes\":\"\",\"startDate\":\"2019-10-15T15:15:44.000Z\",\"url\":\"https://one.motusnova.com/api/prescriptions/c8a44dfe-6c97-4c0a-8e38-3f169b357378/\",\"user\":\"https://one.motusnova.com/api/users/parth.patel/\",\"uuid\":\"c8a44dfe-6c97-4c0a-8e38-3f169b357378\"}";
        SetPatient(patientString);
        SetPrescription(prescriptionString);
        
        LoadPatient();
        BuildGameList();
        UpdateDynamicROM();

        BuildGameList();
        //SetDeviceURL();
        
    }
    
    public void LoadPatient()
    {
        string tempPatientString = PlayerPrefs.GetString("Patient", "No Patient.");
        if (!(tempPatientString == "No Patient."))
        {
            this.patient = JsonUtility.FromJson<Patient>(tempPatientString);
            //UpdateUsernameText();
        }
        
    }

    public void LoadDevice()
    {
        string tempDeviceString = PlayerPrefs.GetString("Device", "No Device.");
        if (!(tempDeviceString == "No Device."))
        {
            this.device = JsonUtility.FromJson<Device>(tempDeviceString);
            PlayerPrefs.SetString("deviceID", ConvertSerialToDeviceID(this.device.GetSerial()));
        }
    }

    public void LoadPrescription()
    {
        string tempPrescriptionString = PlayerPrefs.GetString("Active Prescription", "No Prescription.");
        if (!(tempPrescriptionString == "No Prescription."))
        {
            this.prescription = JsonUtility.FromJson<Prescription>(tempPrescriptionString);
        }

    }

    public void SetDynamicROM(float dynamicROMMin, float dynamicROMMax)
    {
        patient.SetDynamicROMMax(dynamicROMMin);
        patient.SetDynamicROMMin(dynamicROMMax);
        SavePatient();
    }

    public void UpdateDynamicROM()
    {
        try
        {
            float tempMinAngle = (float) Variables.Saved.Get("minAngle");
            float tempMaxAngle = (float) Variables.Saved.Get("maxAngle");
            
        }
        catch
        {
            Variables.Saved.Set("minAngle", -10);
            Variables.Saved.Set("maxAngle", 10);
        }
        if (patient.GetSetInitialROM())
        {
            Variables.Saved.Set("minAngle", patient.GetMinInitialAngle());
            Variables.Saved.Set("maxAngle", patient.GetMaxInitialAngle());
        }

    }

    public void UpdateUsernameText()
    {
        userNameText = GameObject.Find("HiText").GetComponent<Text>();
        userNameText.text = "Hello " + patient.GetAnonId() + "!";
    }

    public List<string[]> ReadPipeDelimitedFile(string filename, char delimiter)
    {

        string filePath = Path.Combine(Application.streamingAssetsPath, filename);

        string fileText = "";

        if (Application.platform == RuntimePlatform.Android)
        {
            WWW reader = new WWW(filePath);
            while (!reader.isDone) { }

            fileText = reader.text;

        }
        else
        {
            if (File.Exists(filePath))
            {

                fileText = File.ReadAllText(filePath);

            }
            else
            {
                Debug.LogError("Cannot find device URL file");
            }
        }

        string[] lines = fileText.Split("\n"[0]);

        List<string[]> outputList = new List<string[]>();

        foreach (string line in lines)
        {
            string[] column = line.Split(delimiter);
            outputList.Add(column);
        }
        return outputList;

    }

    public void SetDeviceURL()
    {
        List<string[]> device_id_dictionary = ReadPipeDelimitedFile("MainMenu/device_id_urls.txt", '|');

        deviceID = PlayerPrefs.GetString("deviceID", "0");
        deviceID = Regex.Replace(deviceID, "[a-zA-z]+", "");

        string device_url = "No URL";

        foreach (string[] device_url_pair in device_id_dictionary)
        {
            string id = device_url_pair[0];
            string url = device_url_pair[1];
            if (deviceID.Equals(id))
            {
                device_url = url;
                break;
            }
        }
        PlayerPrefs.SetString("deviceURL", device_url);
    }

    public string GetDeviceURL()
    {
        return PlayerPrefs.GetString("deviceURL");
    }

    public long GetDeviceID()
    {
        string deviceID = PlayerPrefs.GetString("deviceID", "0");
        deviceID = Regex.Replace(deviceID, "[a-zA-z]+", "");
        return Convert.ToInt64(deviceID);
    }

    public void LaunchAndroidApp(String appID)
    {
        Debug.Log("Launching app " + appID);
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

        AndroidJavaObject launchIntent = null;
        try
        {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", appID);
            ca.Call("startActivity", launchIntent);
        }
        catch (System.Exception e)
        {
            Debug.Log("Failed to launch app " + appID);
        }

        up.Dispose();
        ca.Dispose();
        packageManager.Dispose();
    }

    //Generates a random long from bytes.
    long LongRandom(long min, long max, System.Random rand)
    {
        byte[] buf = new byte[8];
        rand.NextBytes(buf);
        long longRand = BitConverter.ToInt64(buf, 0);

        return (Math.Abs(longRand % (max - min)) + min);
    }

    public void GenerateSessionID()
    {
        this.sessionID = LongRandom(0, 9223372036854775807, new System.Random());
        PlayerPrefs.SetString("sessionID", this.sessionID.ToString());
    }

    public string GetSessionID()
    {
        return PlayerPrefs.GetString("sessionID", "0");
    }

    public void SavePatient()
    {
        if (this.patient != null)
        {
            PlayerPrefs.SetString("Patient Name", this.patient.GetAnonId());
            string patientString = JsonUtility.ToJson(this.patient, true);
            PlayerPrefs.SetString("Patient", patientString);
            UpdateDynamicROM();

        }
    }
    public void SetPatient(string patientString)
    {
        if (patientString != "No Patient")
        {
            this.patient = JsonUtility.FromJson<Patient>(patientString);
            PlayerPrefs.SetString("Patient Name", patient.GetAnonId());
            PlayerPrefs.SetString("Patient", patientString);
            //UpdateUsernameText();
        }
        else
        {
            LoadPatient();
        }

    }
    public void SetDevice(string deviceString)
    {
        if (deviceString != "No Device")
        {
            this.device = JsonUtility.FromJson<Device>(deviceString);
            PlayerPrefs.SetString("deviceID", ConvertSerialToDeviceID(this.device.GetSerial()));
            PlayerPrefs.SetString("Device", deviceString);
        }
        else
        {
            LoadDevice();
        }

    }

    public string ConvertSerialToDeviceID(string serial)
    { 
        serial = serial.Replace("https://one.motusnova.com/api/devices/", "");
        serial = serial.Replace("/", "");
        return serial;
    }

    public void SetDevice(Device device)
    {
        if (device!= null)
        {
            this.device = device;
            PlayerPrefs.SetString("deviceID", ConvertSerialToDeviceID(this.device.GetSerial()));
            string deviceString = JsonUtility.ToJson(device, true);
            PlayerPrefs.SetString("Device", deviceString);
        }
        else
        {
            LoadDevice();
        }
    }
    public void SetUpdateAvailable(Boolean updateAvailable)
    {
        int updateInt = updateAvailable ? 1 : 0;
        PlayerPrefs.SetInt("Update Available", updateInt);
    }
    public Boolean GetUpdateAvailable()
    {
        int updateInt = PlayerPrefs.GetInt("Update Available", 0);

        Boolean updateAvailable = false;

        if (updateInt == 1)
        {
            updateAvailable = true;
        }

        return updateAvailable;
    }

    public void SetPatient(Patient patient)
    {
        if (patient != null)
        {
            this.patient = patient;
            PlayerPrefs.SetString("Patient Name", patient.GetAnonId());
            string patientString = JsonUtility.ToJson(patient, true);
            PlayerPrefs.SetString("Patient", patientString);
            //UpdateUsernameText();
        }
        else
        {
            LoadPatient();
        }
    }
    public Patient GetPatient()
    {
        return this.patient;
    }
    public void SetPrescription(string prescriptionString)
    {
        this.prescription = JsonUtility.FromJson<Prescription>(prescriptionString);
        PlayerPrefs.SetString("Active Prescription", prescriptionString);
        BuildGameList();
    }
    public void SetPrescription(Prescription prescription)
    {
        this.prescription = prescription;
        string prescriptionString = JsonUtility.ToJson(prescription, true);
        PlayerPrefs.SetString("Active Prescription", prescriptionString);
        BuildGameList();
    }
    public Prescription GetPrescription()
    {
        return this.prescription;
    }

    public void BuildSceneList()
    {   
        scenesInProject = new List<String>();

        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        Debug.Log("Total number of scenes: " + sceneCount);
        for (int i = 0; i < sceneCount; i++)
        {
             scenesInProject.Add(System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i)).ToString());
            Debug.Log(scenesInProject[i]);
        }
    }
    public void BuildGameList()
    {
        BuildSceneList();
        games = new List<Game>();
        if (prescription != null)
        {



            foreach (GameSetting setting in prescription.GetLinkedGameSettings())
            {
                Game tempGame = new Game();
                tempGame.SetActiveTime(setting.GetActiveTime());
                tempGame.SetRestTime(setting.GetRestTime());
                tempGame.SetCycles(setting.GetCycles());
                tempGame.SetName(setting.GetGameId());
                tempGame.SetMinAngle(patient.GetDynamicROMMin());
                tempGame.SetMaxAngle(patient.GetDynamicROMMax());
                tempGame.SetAssistPressure(patient.GetAssistPressure());
                tempGame.SetAssistPressureHysteresis(patient.GetAssistPressureHysteresis());

                if (tempGame.GetName().Contains("thermometer") || tempGame.GetName().Contains("strongman"))
                {
                    tempGame.SetLowROMGame(true);
                }
                else
                {
                    tempGame.SetLowROMGame(false);
                }
                if (tempGame.GetName().Contains("thermometer"))
                {
                    tempGame.SetPriority(1);
                    tempGame.SetScene("Thermometer");
                }
                else if (tempGame.GetName().Contains("assessment"))
                {
                    tempGame.SetPriority(2);
                    tempGame.SetScene("StrongmanAssessment");
                }
                else if (tempGame.GetName().Contains("spaceshoot"))
                {
                    tempGame.SetPriority(3);
                    tempGame.SetScene("SpaceShoot");
                }
                else if (tempGame.GetName().Contains("strongman") && !tempGame.GetName().Contains("strongmanup"))
                {
                    tempGame.SetPriority(4);
                    tempGame.SetScene("StrongMan");
                }
                else if (tempGame.GetName().Contains("strongmanup"))
                {
                    tempGame.SetPriority(4);
                    tempGame.SetScene("StrongManUp");
                }
                else if (tempGame.GetName().Contains("balloon"))
                {
                    tempGame.SetPriority(4);
                    tempGame.SetScene("Balloon");
                }
                else if (tempGame.GetName().Contains("brickbreaker"))
                {
                    tempGame.SetPriority(4);
                    tempGame.SetScene("BrickBreaker");
                }
                else if (tempGame.GetName().Contains("blocks"))
                {
                    tempGame.SetPriority(4);
                    tempGame.SetScene("Blocks");
                }
                else if (tempGame.GetName().Contains("golf"))
                {
                    tempGame.SetPriority(4);
                    tempGame.SetScene("GolfTitleScene");
                }
                else if (tempGame.GetName().Contains("pong"))
                {
                    tempGame.SetPriority(4);
                    tempGame.SetScene("Pong");
                }
                else if (tempGame.GetName().Contains("slotmachine"))
                {
                    tempGame.SetPriority(4);
                    tempGame.SetScene("SlotMachine");
                }
                else
                {
                    tempGame.SetPriority(4);
                    tempGame.SetScene("Game Not Found");
                }
                if (scenesInProject.Contains(tempGame.GetScene().ToString()))
                {
                    games.Add(tempGame);
                }

            }


            List<Game> sortedGames = games.OrderBy(o => o.priority).ToList();

            games = sortedGames;
        }
    }

    public List<Game> GetGames()
    {
        return this.games;
    }

    public int GetGameIndex(String sceneName)
    {
        int numGames = games.ToArray().Length;
        int gameIndex = -1;
        for (int i = 0; i < numGames; i++)
        {
            Debug.Log("Scene we're checking: " + sceneName);
            Debug.Log("Game scene: " + games[i].GetScene());
            if (games[i].GetScene().Contains(sceneName))
            {
                gameIndex = i;
                break;
            }
        }
        return gameIndex;
    }
    public string GetNextScene()
    {
        int nextSceneIndex;
        string currentScene = GetCurrentScene();
        Debug.Log("Current scene: " + currentScene);
        if (!(games.ToArray().Length >= 1))
        {
            return "MenuScene";
        }
        if (currentScene.Contains("MenuScene"))
        {
            return this.games[0].GetScene();
        }
        
        nextSceneIndex = GetGameIndex(currentScene) + 1;
        Debug.Log("Current game's index: " + nextSceneIndex);
        Debug.Log("Length of games array: " + games.ToArray().Length);
        if (nextSceneIndex == games.ToArray().Length)
        {
            return "MenuScene";
        }
        else
        {
            return this.games[nextSceneIndex].GetScene();
        }
    }
    public string GetCurrentScene()
    {
        String currentScene = SceneManager.GetActiveScene().name;
        if (currentScene.Contains("Hills") || currentScene.Contains("Caves"))
        {
            currentScene = "GolfTitleScene";
        }
        return currentScene;
    }

    public int GetGameTime()
    {
        int gameIndex;
        string currentScene = GetCurrentScene();
        gameIndex = GetGameIndex(currentScene);
        int gameTime = this.games[gameIndex].GetActiveTime();
        Debug.Log("Playing " + currentScene + " for " + gameTime + " seconds.");
        return gameTime;
    }

    public void StartTCPRequest(string message, string topic, string TCPServer)
    {
        object[] parameters = { message, topic, TCPServer };
        StartCoroutine("SendTCPRequest", parameters);
    }

    public IEnumerator SendTCPRequest(object[] parameters)
    {
        string message = (string) parameters[0];
        string topic = (string) parameters[1];
        string TCPServer = (string) parameters[2];
        bool messageSent = false;
        int failCount = 0;

        yield return new WaitForSeconds(3);

        while (!messageSent && failCount < 5)

        {
            try
            {

                switch (TCPServer)
                {
                    case "Galileo":
                        tCPMessage.SendMessage(topic, message, TCPServer);

                        break;

                    case "Updater":
                        tCPMessage.SendMessage(topic, message, TCPServer);

                        break;

                    case "Telemetry":
                        tCPMessage.SendMessage(topic, message, TCPServer);

                        break;
                }
                messageSent = true;
                //Debug.Log("Message sent!");
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                switch (TCPServer)
                {
                    case "Galileo":
                        GalileoTCPClient.GetInstance().ConnectToTcpServer();

                        break;

                    case "Updater":
                        UpdaterTCPClient.GetInstance().ConnectToTcpServer();

                        break;

                    case "Telemetry":
                        TelemetryTCPClient.GetInstance().ConnectToTcpServer();

                        break;
                }
                failCount++;
                //Debug.Log("Message Failed! " + failCount);
                
            }
            yield return new WaitForSeconds(5);


        }
        StopCoroutine("SendTCPRequest");
    }

    public static string[] BreakupPackageArray(string updateString)
    {
        updateString = updateString.Replace("\\\"", "");
        updateString = updateString.Replace("[", "");
        updateString = updateString.Replace("]", "");
        string[] packageList = updateString.Split(',');
        return packageList;

    }

    public static void UpdateAndroidVersions(string androidPackageVersionsMessage)
    {
        string[] packageList = BreakupPackageArray(androidPackageVersionsMessage);
        foreach (string packageAndVersion in packageList)
        {
            string[] packageVersionSplit = packageAndVersion.Split(':');
            string appName = packageVersionSplit[0].Split('.')[2];
            if (appName[0] == 'm' && appName[1] == 'n')
            {
                appName = appName.Substring(2);
            }
            appName = appName.First().ToString().ToUpper() + appName.Substring(1);
            string appVersion = packageVersionSplit[1].Replace(" ", "");
            appVersion = appVersion.Replace("\"", "");
            Variables.Saved.Set(appName + "Version", appVersion);
        }

    }

    public void SetPreviousScene(string newPreviousScene)
    {
        this.previousScene = newPreviousScene;
    }

    public string GetPreviousScene()
    {
        Debug.Log(this.previousScene);
        return this.previousScene;
    }

}
