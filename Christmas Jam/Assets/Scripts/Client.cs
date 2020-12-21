using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class Client : MonoBehaviour
{
    private WebSocket _ws;
    // Start is called before the first frame update
    void Start()
    {
        _ws = new WebSocket("ws://localhost:9001");
        _ws.OnOpen += () =>
        {
            Debug.Log("Client connected");
            _ws.SendText("Hello from client");
        };
        _ws.OnMessage += delegate(byte[] data)
        {
            var message = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log(message);
        };
    }

    private async void OnDestroy()
    {
        await _ws.Close();
    }

    // Update is called once per frame
    void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            _ws.DispatchMessageQueue();
        #endif
    }
}
