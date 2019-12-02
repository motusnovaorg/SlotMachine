using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

/**
 * Manage networks in unity.
 */
public class CustomNetworkManager : MonoBehaviour
{
    private static CustomNetworkManager instance;
    private GameObject passwordText;
    private GameObject checkMarkGO; // Check mark GO for currently selected network.
    private GameObject cancelButton;
    private GameObject connectButton;
    private int numberOfConnectionChecks = 0;

    // Objects aqcuired through unity inspector.
    [SerializeField] Button mainMenuWifiButton;
    [SerializeField] GameObject connectionPanel;
    [SerializeField] GameObject wifiSelectButton;
    [SerializeField] GameObject wifiScrollViewContent;
    [SerializeField] GameObject PleaseWaitText;
    [SerializeField] GameObject networkConnectionDialogPanel;
    [SerializeField] GameObject networkSelectionPanel;
    [SerializeField] GameObject retryButton;
    [SerializeField] GameObject connectNetworkDialogPanel;
    [SerializeField] GameObject disconnectNetworkDialogPanel;
    [SerializeField] GameObject networksVerticalScrollbar;
    [SerializeField] GameObject wifiTitleText;
    [SerializeField] GameObject wifiNetworksTitle;




    public bool isDeviceConnected = false;
    bool isScan = false;

    string wifiJson;
    string selectedWifiSSID;
    private string passwordString;
    public string connectedNetwork;



    // Android classes. Instantiated inside Start method.
    public AndroidJavaObject wifiConfigJavaObject;
    AndroidJavaObject context;


    private void Awake()
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
        // Insure only a single instance exists
        /*
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        */
        //Debug.Log("DeviceMeasurementAwake");
        // Don't destroy between scenes.
        //DontDestroyOnLoad(gameObject);
        if (Application.platform == RuntimePlatform.Android)
        {
            getAvailableNetworksPermission();
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            context = activity.Call<AndroidJavaObject>("getApplicationContext");
            wifiConfigJavaObject = new AndroidJavaObject("com.kalela.philipkalela.wificonfigmodule.WifiConfig", context);
            StartCoroutine("getConnectedNetwork");

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

        String currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "MenuScene")
        {
            StartCoroutine("InitialConnectionCheck");
        }
    }
    public void SetGameObjects()
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.name == "WifiButton")
            {
                mainMenuWifiButton = go.GetComponent<Button>();
            }
            if (go.name == "DisplayConnectionPanel")
            {
                connectionPanel = go;
            }
            if (go.name == "WifiSelectButtonMM")
            {
               wifiSelectButton = go;
            }
            if (go.name == "NetworksAvailableContent")
            {
                wifiScrollViewContent = go;
            }
            if (go.name == "PleaseWaitTextUI")
            {
                PleaseWaitText = go;
            }
            if (go.name == "NetworkConnectionDialogPanel")
            {
                networkConnectionDialogPanel = go;
            }
            if (go.name == "NetworkSelectionPanel")
            {
                networkSelectionPanel = go;
            }
            if (go.name == "RetryButton")
            {
                retryButton = go;
            }
            if (go.name == "ConnectNetworkPanel")
            {
                connectNetworkDialogPanel = go;
            }
            if (go.name == "DisconnectNetworkPanel")
            {
                disconnectNetworkDialogPanel = go;
            }
            if (go.name == "NetworksVerticalScrollbar")
            {
                networksVerticalScrollbar = go;
            }
            if (go.name == "WifiTitleText")
            {
                wifiTitleText = go;
            }
            if (go.name == "WifiNetworksTitle")
            {
                wifiNetworksTitle = go;
            }
        }
        connectionPanel.SetActive(true);
        PleaseWaitText.SetActive(true);
        retryButton.SetActive(false);
        retryButton.GetComponent<Button>().onClick.AddListener(getAvailableNetworks);
        StartCoroutine("GetAPs");
    }
    // Start is called before the first frame update
    void Start()
    {
        String currentScene = SceneManager.GetActiveScene().name;
        if(currentScene == "MenuScene")
        {
            StartCoroutine("InitialConnectionCheck");
        }
        
    }

    // Get location permissions on android to allow collection of available wifi.
    private void getAvailableNetworksPermission()
    {
        if (Application.platform == RuntimePlatform.Android)
        {

            if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
            {
                // We do not have permission to use the users location.
                // Ask for permission or proceed without the functionality enabled.
                Permission.RequestUserPermission(Permission.CoarseLocation);
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }

        }
    }

    // Get currently connected network.
    public IEnumerator getConnectedNetwork()
    {
        connectedNetwork = wifiConfigJavaObject.Call<string>("getConnectedWifi");

        yield return new WaitForSeconds(2); // Approximate wait time before data is recieved from android.

        Debug.Log("connectedNetwork " + connectedNetwork);

        StopCoroutine("getConnectedNetwork");
    }

    // Start get available networks coroutine and update the UI.
    public void getAvailableNetworks()
    {
        connectionPanel.SetActive(true);
        PleaseWaitText.SetActive(true);
        retryButton.SetActive(false);

        StartCoroutine("GetAPs");

    }

    // Get available networks from android. Wait for data to be populated.
    IEnumerator GetAPs()
    {
        Debug.Log("get ap called");
        while (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
        {

            Debug.Log("Did not have appropriate permissions to get AP");

            yield return null;
        }

        while (!isScan && (Application.platform == RuntimePlatform.Android)) // Retry scanning for available networks every 3 seconds
                                                                             //if collecting networks is not successful.
        {
            Debug.Log("Scanning for networks.");
            isScan = wifiConfigJavaObject.Call<bool>("scan");
            yield return new WaitForSeconds(3);
        }
        Debug.Log("Successfully scanned for networks.");

        if (Application.platform != RuntimePlatform.Android) // Unity editor test data.
        {
            yield return new WaitForSeconds(3); // Simulate data retrieval wait time.
            wifiJson =
            "{\"wifiNetworks\":[" +
                "{\"BSSID\":\"02:15:b2:00:01:00\",\"SSID\":\"MotusWifi1\",\"level\":\"-50\",\"capabilities\":\"[ESS]\"}," +
            "{\"BSSID\":\"02:15:b2:00:01:00\",\"SSID\":\"AndroidWifi\",\"level\":\"-50\",\"capabilities\":\"[ESS]\"}," +
            "{\"BSSID\":\"02:15:b2:00:01:00\",\"SSID\":\"MotusWifi2\",\"level\":\"-50\",\"capabilities\":\"[ESS]\"}]}";
        }
        else
        {
            Debug.Log("Calling Java getAPs.");
            wifiConfigJavaObject.Call("getAPs");
            Debug.Log("Getting networks as JSON.");
            wifiJson = wifiConfigJavaObject.Call<string>("getWifiNetworksAsJson");
            yield return new WaitUntil(() => wifiJson.Length > 0); // Wait for data to be retrieved from android class.
            Debug.Log("Setting connected network.");
            connectedNetwork = wifiConfigJavaObject.Call<string>("getConnectedWifi");
            yield return new WaitForSeconds(5); // Approximate wait time before data is recieved from android.
        }
        convertWifiNetworksJsonToObject();

        PleaseWaitText.SetActive(false);
        Debug.Log("Connected Network: " + connectedNetwork);
        Debug.Log("isDeviceConnected: " + isDeviceConnected);
        StopCoroutine("GetAPs");
    }

    // Convert wifi json to C# object and populate List in scene.
    private void convertWifiNetworksJsonToObject()
    {
        Debug.Log("Running convert wifinetworks to json");
        NetworksCollection networks = new NetworksCollection();
        networks = JsonUtility.FromJson<NetworksCollection>(wifiJson);

        if (networks.wifiNetworks.Length == 0)
        {
            wifiConfigJavaObject.Call("showToast", context,
                        "We could not find any available network.");
            retryButton.SetActive(true);
        }

        List<int> networkLevelList = new List<int>();


        foreach (Network network in networks.wifiNetworks)
        {
            networkLevelList.Add(int.Parse(network.level)); // Create list of network levels.
        }

        networkLevelList.Sort(); // Sort list in ascending order

        List<Network> sortedNetworkList = new List<Network>();

        for (int i = networkLevelList.Count - 1; i >= 0; i--) // Get network levels in descending order
        {
            for (int j = 0; j < networks.wifiNetworks.Length; j++)
            {
                if (int.Parse(networks.wifiNetworks[j].level) == networkLevelList[i])
                {
                    sortedNetworkList.Add(networks.wifiNetworks[j]);
                }
            }

        }
        List<Network> newSortedNetworkList = sortedNetworkList.GroupBy(elem => elem.SSID).Select(group => group.First()).ToList();

        foreach (Network network in newSortedNetworkList.Distinct().ToArray())
        {
            if (!network.SSID.Equals(""))
            {
                GameObject wifiButtonClone = Instantiate(wifiSelectButton); // Instantiate clickable button.
                wifiButtonClone.transform.SetParent(wifiScrollViewContent.transform, false);
                wifiButtonClone.GetComponentInChildren<Text>().text = network.SSID; // Set text on clickable button to found network SSID.

            }

        }
        UpdateCheckMark();
    }

    // Open dialog for login into a selected wifi network.
    public void openConnectionDialog(string networkSSID)
    {

           wifiNetworksTitle.SetActive(false);
           wifiTitleText.SetActive(false);



        networkConnectionDialogPanel.SetActive(true);
        networkSelectionPanel.SetActive(false);
        

        if (connectedNetwork.Contains(networkSSID))
        {
            disconnectNetworkDialogPanel.SetActive(true);
            connectNetworkDialogPanel.SetActive(false);

        }
        else
        {
            disconnectNetworkDialogPanel.SetActive(false);
            connectNetworkDialogPanel.SetActive(true);

            passwordText = GameObject.Find("PasswordInputField");
        }
  
        Text wifiText = GameObject.Find("WifiNameTitleText").GetComponent<Text>();
        wifiText.text = networkSSID;
        selectedWifiSSID = networkSSID;
    }

    // Close wifi connection dialog.
    public void closeConnectionDialog() // Handled by unity now
    {
        wifiNetworksTitle.SetActive(true);
        wifiTitleText.SetActive(true);
        networkConnectionDialogPanel.SetActive(false);
        networkSelectionPanel.SetActive(true);
    }

    // Ping google.com and ensure the right html is found for connection to be available.
    private void CheckDeviceConnection()
    {
        string HtmlText = GetHtmlFromUri("http://google.com");
        Debug.Log("HTML Text: " + HtmlText);
        if (HtmlText == "")
        {
            isDeviceConnected = false;
        }
        else if (!HtmlText.Contains("schema.org/WebPage"))
        {
            //Redirecting since the beginning of googles html contains that 
            //phrase and it was not found
            isDeviceConnected = false;
        }
        else
        {
            isDeviceConnected = true;
        }
        numberOfConnectionChecks++;
        if (!isDeviceConnected && numberOfConnectionChecks < 4)
        {
            Invoke("CheckDeviceConnection", 5);
        }
        else
        {
            if (!isDeviceConnected)
            {
                OnConnectionFailed();
            }
            else
            {
                StartCoroutine(nameof(getConnectedNetwork));
                OnSuccessfulConnection();
            }
            cancelButton.SetActive(true);
            connectButton.SetActive(true);
        }
    }
    //Coroutine for checking whether or not the network is connected on boot.
    IEnumerator InitialConnectionCheck()
    {
        string HtmlText = GetHtmlFromUri("http://google.com");
        Debug.Log("HTML Text: " + HtmlText);
        if (HtmlText == "")
        {
            isDeviceConnected = false;
        }
        else if (!HtmlText.Contains("schema.org/WebPage"))
        {
            //Redirecting since the beginning of googles html contains that 
            //phrase and it was not found
            isDeviceConnected = false;
        }
        else
        {
            isDeviceConnected = true;
        }

        yield return null;

        StopCoroutine("InitialConnectionCheck");
    }
    
    // Called through unity button. Deactivates connect and cancel buttons and runs the connectAP function.
    public void connectWifiNetwork()
    {
        cancelButton = GameObject.Find("CancelConnectWifiButton");
        connectButton = GameObject.Find("ConnectWifiButton");
        connectButton.SetActive(false);
        cancelButton.SetActive(false);
        PleaseWaitText.SetActive(true);
        numberOfConnectionChecks = 0;
        connectAP();
        try
        {
            passwordString = passwordText.GetComponent<InputField>().text;
            passwordText.GetComponent<InputField>().text = "";
        }
        catch (NullReferenceException e)
        {
            passwordString = "";
            Debug.Log(e);
        }
        Debug.Log(passwordString);
    }

    // Called through unity button. Disconnects the network.
    public void disconnectWifiNetwork()
    {
        disconnectAP();
    }

    // Connect to a selected wifi network.
    public void connectAP()
    {
        
        wifiConfigJavaObject.Call("connectToAP", selectedWifiSSID, passwordString);
        isDeviceConnected = false;
        Invoke("CheckDeviceConnection", 5);

    }

    // Disconnect from wifi network.
    public void disconnectAP()
    {

        bool disconnected = wifiConfigJavaObject.Call<bool>("disconnectWifiNetwork");

        if (!disconnected)
        {
            OnDisconnectionFailed();
        }
        else
        {
            OnSuccessfulDisconnection();
        }

    }

    public void UpdateCheckMark()
    {
        Debug.Log("Calling Update Checkmark Method");
        List<GameObject> networkSSIDsUnity = new List<GameObject>();

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.name.Equals("wifiNameText"))
            {
                networkSSIDsUnity.Add(go);
            }
        }
        Debug.Log(networkSSIDsUnity.ToString());


        Debug.Log("length " + networkSSIDsUnity.Count);

        foreach (GameObject networkSSIDUnity in networkSSIDsUnity)
        {
            if (!isDeviceConnected)
            {
                networkSSIDUnity.transform.Find("WifiConnectedIconImage").gameObject.SetActive(false);

            }
            else
            {
                if (connectedNetwork.Contains(networkSSIDUnity.GetComponent<Text>().text))
                {
                    networkSSIDUnity.transform.Find("WifiConnectedIconImage").gameObject.SetActive(true);
                }
                else
                {
                    networkSSIDUnity.transform.Find("WifiConnectedIconImage").gameObject.SetActive(false);
                }
            }
        }


    }

    // Call when the user successfully connects to the internet. Updates UI appropriately.
    private void OnSuccessfulConnection()
    {
        PleaseWaitText.SetActive(false);
        connectedNetwork = wifiConfigJavaObject.Call<string>("getConnectedWifi");

        UpdateCheckMark();

        networkConnectionDialogPanel.SetActive(false);
        networkSelectionPanel.SetActive(true);
        connectionPanel.SetActive(true);
        closeConnectionDialog();
        wifiConfigJavaObject.Call("showToast", context,
                "Network Connected");
    }

    // Call when the user successfully disconnects to the internet. Updates UI appropriately.
    private void OnSuccessfulDisconnection()

    {
        connectedNetwork = "";
        UpdateCheckMark();
        networkConnectionDialogPanel.SetActive(false);
        networkSelectionPanel.SetActive(true);
        connectionPanel.SetActive(true);
        closeConnectionDialog();
        wifiConfigJavaObject.Call("showToast", context,
                "Network Disconnected");

    }
    // Call when wifi login is unsuccessful. Updates UI appropriately.
    private void OnConnectionFailed()
    {
        UpdateCheckMark();
        networkConnectionDialogPanel.SetActive(true);
        networkSelectionPanel.SetActive(false);
        connectionPanel.SetActive(true);
        PleaseWaitText.SetActive(false);
        wifiConfigJavaObject.Call("showToast", context,
                        "Check password and try again");
    }

    // Call when wifi disconnection is unsuccessful. Updates UI appropriately.
    private void OnDisconnectionFailed()
    {
        UpdateCheckMark();
        networkConnectionDialogPanel.SetActive(true);
        networkSelectionPanel.SetActive(false);
        connectionPanel.SetActive(true);
        PleaseWaitText.SetActive(false);
        wifiConfigJavaObject.Call("showToast", context,
                        "Could not discconnect from selected network, Please try again");
    }

    // Get html from pinged resource.
    public string GetHtmlFromUri(string resource)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //Limiting the array to 80 so we don't have
                        //to parse the entire html.
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
        return html;
    }
}