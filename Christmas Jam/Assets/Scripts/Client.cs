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

    public void Connect(bool isRemote)
    {
        TcpConfig tcpConfig = new TcpConfig(true, 5000, 20000);
        _ws = SimpleWebClient.Create(16*1024, 1000, tcpConfig);
        UriBuilder builder;
        
        if (isRemote)
        {
            builder = new UriBuilder()
            {
                Scheme = "wss",
                Host = "tylrpage.com",
                Port = Constants.GAME_PORT
            };
        }
        else
        {
            builder = new UriBuilder()
            {
                Scheme = "ws",
                Host = "localhost",
                Port = Constants.GAME_PORT
            };
        }
        Debug.Log("Connecting to " + builder.Uri);
        _ws.Connect(builder.Uri);
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
