using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorClient : MonoBehaviour
{
    public string ip;
    public int port;

    private Client client;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"UDP server address: {ip}");
        Debug.Log($"UDP server port: {port}");

        // Create a new TCP chat client
        client = new Client(ip, port);


        // Connect the client
        Debug.Log("Client connecting...");
        client.Connect();
        Debug.Log("Done!");
    }

    private void DisconnectClient()
    {
        Debug.Log("Client disconnecting...");
        client.Disconnect();
        Debug.Log("Done!");
    }

    private void StopClient()
    {
        Debug.Log("Client disconnecting...");
        client.DisconnectAndStop();
        Debug.Log("Done!");
    }

    private void OnApplicationQuit()
    {
        StopClient();
    }
}
