using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

public class Game : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        Debug.Log(e.Data);
        Send("Hello from server");
    }

    protected override void OnOpen()
    {
        Debug.Log("Client connected");
    }
}

public class Server : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        WebSocketServer wssv;
        if (Application.isBatchMode)
        {
            Debug.Log("Setting up secure server");
            wssv = new WebSocketServer(9001, true);
            wssv.SslConfiguration.EnabledSslProtocols = SslProtocols.None;
            wssv.SslConfiguration.ServerCertificate = new X509Certificate2("cert.pks");
        }
        else
        {  
            Debug.Log("Setting up non secure server");
            wssv = new WebSocketServer(9001);
        }
        wssv.AddWebSocketService<Game>("/christmasGame");
        wssv.Start();
        
        Debug.Log("Server started");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
