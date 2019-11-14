using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TelemetryTCPClient : MonoBehaviour
{
    private int port = 8052;
    private int buflen = 1024*2;

    #region private members     
    private TcpClient socketConnection;
    private Thread clientReceiveThread;

    private static TelemetryTCPClient instance; // Singleton.
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
    void Start()
    {
        ConnectToTcpServer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Setup socket connection.    
    public void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    // Runs in background clientReceiveThread; Listens for incoming data.     
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("127.0.0.1", port);

            Byte[] bytes = new Byte[buflen];
            while (true)
            {
                // Get a stream object for reading              
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incoming stream into byte arrary.
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incomingData = new byte[length];
                        Array.Copy(bytes, 0, incomingData, 0, length);
                        // Convert byte array to string message.                        
                        string serverMessage = Encoding.ASCII.GetString(incomingData);
                        Debug.Log("server message received as: " + serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    // Send message to server using socket connection.     
    public void SendMessage(TCPMessage.TCPMessageData message)
    {
        if (socketConnection == null)
        {
            ConnectToTcpServer();
            return;
        }
        try
        {
            // Get a stream object for writing.             
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string data = JsonUtility.ToJson(message) + "\n"; // We must send whole lines...
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);

                stream.Write(dataBytes, 0, dataBytes.Length);
                stream.Flush();
                
            } // TODO - Need an else clause, if we can't write what do we do?
        }
        catch (SocketException socketException)
        {
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

    public static TelemetryTCPClient GetInstance()
    {
        return instance;
    }
}