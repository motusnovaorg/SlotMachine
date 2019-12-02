using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * Manage each network populated button.
 */
public class NetworkWifiSelectHelper : MonoBehaviour
{
    CustomNetworkManager customNetworkManager; // Custom manager dependency.

    // Start is called before the first frame update
    void Start()
    {

        customNetworkManager = GameObject.Find("ConnectivityManagerGO").GetComponent<CustomNetworkManager>(); // Inject CustomNetwork class into wifi network class.
        GameObject wifiSSIDGO = transform.Find("wifiNameText").gameObject;
        GameObject checkMarkGO = wifiSSIDGO.transform.Find("WifiConnectedIconImage").gameObject; // Get checkmark gameobject from button child.


        Button thisButton = gameObject.GetComponent<Button>();
        string networkSSID = gameObject.GetComponentInChildren<Text>().text;

        // Set checkmark for connected network and vice versa.
        
        if (customNetworkManager.connectedNetwork.Contains(networkSSID) && !networkSSID.Equals("") && customNetworkManager.isDeviceConnected)
        {
            Debug.Log("checkMark set true");
            checkMarkGO.SetActive(true);
            checkMarkGO.transform.SetSiblingIndex(0);
        } else
        {
            checkMarkGO.SetActive(false);
        }

        // Null check for potential button without SSID set.
        if (networkSSID != null)
        {
            thisButton.onClick.AddListener(() => customNetworkManager.openConnectionDialog(networkSSID));
        }
    }

}
