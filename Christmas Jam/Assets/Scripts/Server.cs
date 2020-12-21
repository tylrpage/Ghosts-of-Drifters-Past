using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using Mirror.SimpleWeb;

public class Server : MonoBehaviour
{
    private SimpleWebServer _webServer;
    
    // Start is called before the first frame update
    void Start()
    {
        SslConfig sslConfig;
        TcpConfig tcpConfig = new TcpConfig(true, 5000, 20000);
        if (Application.isBatchMode)
        {
            Debug.Log("Setting up secure server");
            sslConfig = new SslConfig(true, "cert.pfx", "", SslProtocols.Tls12);
        }
        else
        {  
            Debug.Log("Setting up non secure server");
            sslConfig = new SslConfig(false, "", "", SslProtocols.Tls12);
        }
        _webServer = new SimpleWebServer(10000, tcpConfig, 16*1024, 3000, sslConfig);
        _webServer.Start(Constants.GAME_PORT);
        
        Debug.Log("Server started");
        
        _webServer.onConnect += delegate(int i)
        {
            Debug.Log("Client connected");
            byte[] bytes = Encoding.UTF8.GetBytes("Hello from server");
            _webServer.SendOne(i, new ArraySegment<byte>(bytes));
        };
        
        _webServer.onData += delegate(int i, ArraySegment<byte> bytes)
        {
            var message = Encoding.UTF8.GetString(bytes.Array);
            Debug.Log("Server received: " + message);
        };
        
        _webServer.onError += delegate(int i, Exception exception)
        {
            Debug.Log("Error: " + exception.Message);
        };
        
        _webServer.onDisconnect += delegate(int i)
        {
            Debug.Log("Client disconnected");
        };
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _webServer.ProcessMessageQueue(this);
    }
}
