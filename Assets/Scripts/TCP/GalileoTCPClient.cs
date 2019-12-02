using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class GalileoTCPClient : MonoBehaviour {
    private int port = 8051;
    private int buflen = 1024*10;
    private Boolean isConnected = false;
    public PatientManager patientManager;

	#region private members 	
	private TcpClient socketConnection; 	
	private Thread clientReceiveThread;

    private static GalileoTCPClient instance; // Singleton.
	#endregion

    void Awake()
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

	// Use this for initialization 	
	void Start () {
		ConnectToTcpServer();     
	}

	void Update () {
	}

	// Setup socket connection. 	
	public void ConnectToTcpServer () { 		
		try {  			
			clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
			clientReceiveThread.IsBackground = true; 			
			clientReceiveThread.Start();
            isConnected = true;
		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		} 	
	}

	
	// Runs in background clientReceiveThread; Listens for incoming data. 	
	private void ListenForData() { 		
		try { 			
			socketConnection = new TcpClient("127.0.0.1", port);
            
			Byte[] bytes = new Byte[buflen];             
			while (true) { 				
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream()) { 					
					int length; 					

                    // Read incoming stream into byte arrary.

					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {

                        var incomingData = new byte[length]; 						
						Array.Copy(bytes, 0, incomingData, 0, length);

                        // Convert byte array to string message. 						
                        
                        string serverMessage = Encoding.ASCII.GetString(incomingData);
                        Debug.Log("server message received as: " + serverMessage);
                        
                         TCPMessage.TCPMessageData messageObject = JsonUtility.FromJson<TCPMessage.TCPMessageData>(serverMessage);
                        switch (messageObject.topic)
                        {
                            case "Device Verified":
                                if (messageObject.message == "No registration.")
                                {
                                    //I'm not sure what I want to do here.
                                    Debug.Log("Device is NOT registered.");
                                }
                                else
                                {
                                    TCPMessage.GetInstance().SendMessage("Get Device", "I would like my device, please.", "Galileo");
                                }
                                break;

                            case "Device":
                                if (messageObject.message == "No device.")
                                {
                                    Debug.Log("No device was found....");
                                }
                                else
                                {
                                    patientManager = GameObject.Find("PatientManagerGO").GetComponent<PatientManager>();
                                    
                                    Device device = JsonUtility.FromJson<Device>(messageObject.message);
                                    patientManager.SetDevice(device);
                                    TCPMessage.GetInstance().SendMessage("Get Patient", "I would like my patient, please.", "Galileo");
                                }
                                break;

                            case "Patient":
                                if (messageObject.message == "No patient")
                                {
                                    Debug.Log("No patient was found....");
                                    TCPMessage.GetInstance().SendMessage("Get Prescription", "I would like my prescriptions, please.", "Galileo");
                                }
                                else
                                {
                                    patientManager = GameObject.Find("PatientManagerGO").GetComponent<PatientManager>();
                                    Patient patient = JsonUtility.FromJson<Patient>(messageObject.message);
                                    patientManager.SetPatient(patient);
                                    TCPMessage.GetInstance().SendMessage("Get Prescription", "I would like my prescriptions, please.", "Galileo");
                                }
                                break;

                            case "Get Patient":

                                TCPMessage.GetInstance().SendMessage("Patient", PlayerPrefs.GetString("Patient", "No patient."), "Galileo");

                                break;

                            case "Prescription":
                                if (messageObject.message == "No prescription.")
                                {
                                    Debug.Log("No prescription was found....");
                                }
                                else
                                {
                                    //This is just left as an example of how to unpack a prescription.
                                    Prescription prescription = JsonUtility.FromJson<Prescription>(messageObject.message);
                                    PlayerPrefs.SetString("Active Prescription", messageObject.message);
                                    patientManager.SetPrescription(prescription);
                                }

                                break;

                        }
                    } 				
				} 			
			}         
		}         
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}

	public void SendMessage(TCPMessage.TCPMessageData message) {
        Debug.Log("Calling client send message method.");
        if (socketConnection == null) {

            Debug.Log("Socket Connecton Null");
            ConnectToTcpServer();

            return;         
		}  		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {
                //You have to append a new line character for the input stream to read it.
                string data = JsonUtility.ToJson(message) + "\n";
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                stream.Write(dataBytes, 0, dataBytes.Length);
                stream.Flush();
                
                Debug.Log(message);
                
			} // TODO - Need an else clause, if we can't write what do we do?
		} 		
		catch (SocketException socketException) {

            Debug.Log("Socket exception: " + socketException);         
		}
        catch (InvalidOperationException operationException)
        {
            //This exception seems to be thrown when it tries to get a socket stream that doesn't exist.
            //Catching the exception and adding the debug log prevent the games from behaving strangely,
            //and data still sends when the connection is re-established.
            Debug.Log("Invalid Operation Exception:" + operationException);
        }
	}

    public static GalileoTCPClient GetInstance()
    {
        return instance;
    }
    public Boolean IsGalileoClientConnected()
    {
        return this.isConnected;
    }
}
