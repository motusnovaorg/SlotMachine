using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;


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
}
