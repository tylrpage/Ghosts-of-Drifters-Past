using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using Mirror.SimpleWeb;
using NetStack.Quantization;
using NetStack.Serialization;

public class Server : MonoBehaviour
{
    [SerializeField] private float SendInterval;
    
    private SimpleWebServer _webServer;
    private Dictionary<ushort, uint[]> _playerTransforms;
    private BitBuffer _bitBuffer = new BitBuffer(1024);
    private byte[] _smallBuffer = new byte[10];
    private byte[] _bigBuffer = new byte[2000];
    private float _timeSinceLastSend;
    private List<int> _connectionIds;
     
    // Start is called before the first frame update
    void Start()
    {
        int targetFps = (int)Mathf.Ceil(1 / SendInterval);
        Application.targetFrameRate = targetFps;
        Debug.Log($"Targeting fps of: {targetFps}");
        
        _timeSinceLastSend = Time.time;
        _playerTransforms = new Dictionary<ushort, uint[]>();
        _connectionIds = new List<int>();

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
        
        _webServer.onConnect += WebServerOnonConnect;
        
        _webServer.onData += WebServerOnonData;
        
        _webServer.onError += delegate(int i, Exception exception)
        {
            Debug.Log("Error: " + exception.Message);
        };
        
        _webServer.onDisconnect += WebServerOnonDisconnect;
    }

    private void WebServerOnonConnect(int id)
    {
        _connectionIds.Add(id);
        _playerTransforms[(ushort)id] = null;
        
        _bitBuffer.Clear();
        _bitBuffer.AddUShort(2); // id notification message
        _bitBuffer.AddUShort((ushort) id);
        _bitBuffer.ToArray(_smallBuffer);
        _webServer.SendOne(id, new ArraySegment<byte>(_smallBuffer, 0, 4));
        
        Debug.Log($"Player connected, player count: {_playerTransforms.Count}");
    }

    private void WebServerOnonDisconnect(int id)
    {
        _connectionIds.Remove(id);
        
        ushort shortId = (ushort) id;
        if (_playerTransforms.ContainsKey(shortId))
        {
            _playerTransforms.Remove(shortId);
        }
        
        _bitBuffer.Clear();
        _bitBuffer.AddUShort(0); // send a disconnect message
        _bitBuffer.AddUShort(shortId);
        _bitBuffer.ToArray(_smallBuffer);
        _webServer.SendAll(_connectionIds, new ArraySegment<byte>(_smallBuffer));
        
        Debug.Log($"Player disconnected, player count: {_playerTransforms.Count}");
    }

    private void WebServerOnonData(int id, ArraySegment<byte> data)
    {
        ushort shortId = (ushort) id;
        if (_playerTransforms[shortId] == null)
        {
            _playerTransforms[shortId] = new uint[8];
        }
        _bitBuffer.Clear();
        _bitBuffer.FromArray(data.Array, data.Count);

        for (int i = 0; i < 8; i++)
        {
            _playerTransforms[shortId][i] = _bitBuffer.ReadUInt();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _webServer.ProcessMessageQueue(this);

        if (Time.time - _timeSinceLastSend > SendInterval)
        {
            _timeSinceLastSend = Time.time;
            ushort validCount = 0; //incremented every loop where player transform isnt null

            _bitBuffer.Clear();
            _bitBuffer.AddUShort(1); // state message
            _bitBuffer.AddUShort((ushort) _playerTransforms.Count);
            foreach (var player in _playerTransforms)
            {
                if (player.Value != null)
                {
                    validCount++;
                    _bitBuffer.AddUShort(player.Key);
                    for (int i = 0; i < 8; i++)
                    {
                        _bitBuffer.AddUInt(player.Value[i]);
                    }
                }
            }
            
            _bitBuffer.ToArray(_bigBuffer);
            _webServer.SendAll(_connectionIds, new ArraySegment<byte>(_bigBuffer, 0, 6 + 30 * validCount));
        }
    }

    private void OnDestroy()
    {
        _webServer.Stop();
    }
}
