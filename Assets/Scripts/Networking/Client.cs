using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client : NetCoreServer.UdpClient
{
    public Client(string address, int port) : base(address, port) { }

    public void DisconnectAndStop()
    {
        _stop = true;
        Disconnect();
        while (IsConnected)
            Thread.Yield();
    }

    protected override void OnConnected()
    {
        Debug.Log($"Echo UDP client connected a new session with Id {Id}");

        // Start receive datagrams
        ReceiveAsync();
    }

    protected override void OnDisconnected()
    {
        Debug.Log($"Echo UDP client disconnected a session with Id {Id}");

        // Wait for a while...
        Thread.Sleep(1000);

        // Try to connect again
        if (!_stop)
            Connect();
    }

    protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
    {
        Debug.Log("Here");
        string incoming = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        Debug.Log("Data Recieved: " + incoming);
        GameControl.instance.sensorValue = float.Parse(incoming);
        GameControl.instance.ServerIP.text = "Sensor Value: " + incoming;
        ReceiveAsync();
    }

    protected override void OnSent(EndPoint endpoint, long sent)
    {
        // Continue receive datagrams
        ReceiveAsync();
    }


    protected override void OnError(SocketError error)
    {
        Debug.Log($"Echo UDP client caught an error with code {error}");
    }

    private bool _stop;
}