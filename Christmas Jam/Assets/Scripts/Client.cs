using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Mirror.SimpleWeb;
using NetStack.Quantization;
using NetStack.Serialization;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    [SerializeField] private GameObject GhostCarPrefab;

    [SerializeField] private float SendInterval;
    private Transform _modelTransform;
    
    private SimpleWebClient _ws;
    private Dictionary<ushort, GhostCarBehavior> _ghostCars;
    private BitBuffer _bitBuffer = new BitBuffer(1024);
    private byte[] _byteBuffer = new byte[64];
    private float _timeSinceLastSend;
    private float _myId;

    private static Client _instance;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        SceneManager.sceneLoaded += delegate(Scene arg0, LoadSceneMode mode)
        {
            if (arg0.name == "Game")
            {
                _modelTransform = GameObject.FindWithTag("Model").transform;
                Debug.Log("Scene loaded");
            }
        };
    }

    void Start()
    {
        _timeSinceLastSend = Time.time;
        _ghostCars = new Dictionary<ushort, GhostCarBehavior>();
        
        TcpConfig tcpConfig = new TcpConfig(true, 5000, 20000);
        _ws = SimpleWebClient.Create(16*1024, 1000, tcpConfig);
        
        
        _ws.onConnect += delegate
        {
            Debug.Log("Client connected");
        };
        _ws.onData += WsOnonData;
        _ws.onError += delegate(Exception exception)
        {
            Debug.Log("Error: " + exception.Message);
        };
        
        Connect(true);
    }

    private void WsOnonData(ArraySegment<byte> obj)
    {
        _bitBuffer.Clear();
        _bitBuffer.FromArray(obj.Array, obj.Count);

        ushort messageId = _bitBuffer.ReadUShort();

        switch (messageId)
        {
            case 0:
            {
                ushort id = _bitBuffer.ReadUShort();
                if (_ghostCars.ContainsKey(id))
                {
                    Destroy(_ghostCars[id].gameObject);
                    _ghostCars.Remove(id);
                }

                break;
            }
            case 1:
            {
                ushort count = _bitBuffer.ReadUShort();
                for (int i = 0; i < count; i++)
                {
                    ushort id = _bitBuffer.ReadUShort();
                    
                    QuantizedVector3 qPosition = new QuantizedVector3(_bitBuffer.ReadUInt(), _bitBuffer.ReadUInt(), _bitBuffer.ReadUInt());
                    QuantizedQuaternion qRotation = new QuantizedQuaternion(_bitBuffer.ReadUInt(), _bitBuffer.ReadUInt(), _bitBuffer.ReadUInt(), _bitBuffer.ReadUInt());
                    ushort qBestTime = _bitBuffer.ReadUShort();

                    // Ignore it if it is the transform for my own car
                    if (_myId == id || id == 0)
                    {
                        continue;
                    }
                    
                    Vector3 postion = BoundedRange.Dequantize(qPosition, Constants.WORLD_BOUNDS);
                    Quaternion rotation = SmallestThree.Dequantize(qRotation);

                    if (!_ghostCars.ContainsKey(id))
                    {
                        GameObject newCar = Instantiate(GhostCarPrefab, postion, rotation);
                        _ghostCars[id] = newCar.GetComponent<GhostCarBehavior>();
                    }
                    else
                    {
                        _ghostCars[id].UpdateTransform(postion, rotation);
                        float bestTime = HalfPrecision.Dequantize(qBestTime);
                        _ghostCars[id].UpdatebestTime(bestTime);
                    }
                }

                break;
            }
            case 2:
            {
                ushort id = _bitBuffer.ReadUShort();
                
                _myId = id;
                
                break;
            }
        }
    }

    public void Connect(bool isRemote)
    {
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
        if (_ws != null)
        {
            _ws.Disconnect();
        }
    }

    void LateUpdate()
    {
        _ws.ProcessMessageQueue(this);
        
        if (_ws.ConnectionState == ClientState.Connected)
        {
            if (Time.time - _timeSinceLastSend > SendInterval)
            {
                _timeSinceLastSend = Time.time;

                QuantizedVector3 qPosition = BoundedRange.Quantize(_modelTransform.position, Constants.WORLD_BOUNDS);
                QuantizedQuaternion qRotation = SmallestThree.Quantize(_modelTransform.rotation);
                ushort qBestTime = HalfPrecision.Quantize(GameTimer.BestTime);
                _bitBuffer.Clear();
                _bitBuffer.AddUInt(qPosition.x)
                    .AddUInt(qPosition.y)
                    .AddUInt(qPosition.z)
                    .AddUInt(qRotation.m)
                    .AddUInt(qRotation.a)
                    .AddUInt(qRotation.b)
                    .AddUInt(qRotation.c)
                    .AddUShort(qBestTime)
                    .ToArray(_byteBuffer);
                _ws.Send(new ArraySegment<byte>(_byteBuffer, 0, 28));
            }
        }
    }
}
