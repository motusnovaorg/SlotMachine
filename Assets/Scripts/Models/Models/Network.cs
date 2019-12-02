using System;

[Serializable]
public class Network
{
    public String BSSID;
    public String SSID;
    public String capabilities;
    public String level;
    
}

[Serializable]
public class NetworksCollection
{
    public Network[] wifiNetworks;
}
