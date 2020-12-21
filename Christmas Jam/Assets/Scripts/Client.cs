using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Mirror.SimpleWeb;

public class Client : MonoBehaviour
{
    private SimpleWebClient _ws;
    // Start is called before the first frame update
    void Start()
    {
        TcpConfig tcpConfig = new TcpConfig(true, 5000, 20000);
        _ws = SimpleWebClient.Create(16*1024, 1000, tcpConfig);
        Uri serverAddress = new Uri("ws://localhost:9001");
        _ws.Connect(serverAddress);
        _ws.onConnect += delegate
        {
            Debug.Log("Client connected");
            byte[] bytes = Encoding.UTF8.GetBytes("Hello from client");
            _ws.Send(new ArraySegment<byte>(bytes));
        };
        _ws.onData += delegate(ArraySegment<byte> bytes)
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes.Array);
            Debug.Log(message);
        };
        _ws.onError += delegate(Exception exception)
        {
            Debug.Log("Error: " + exception.Message);
        };
    }

    private void OnDestroy()
    {
        _ws.Disconnect();
    }

    void LateUpdate()
    {
        _ws.ProcessMessageQueue(this);
    }
}
