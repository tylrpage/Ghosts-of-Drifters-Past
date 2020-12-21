using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas : MonoBehaviour
{
    private void Start()
    {
        if (Application.isBatchMode)
        {
            Host();
        }
    }

    public void Host()
    {
        gameObject.AddComponent<Server>();
    }

    public void JoinRemote()
    {
        Client client = gameObject.AddComponent<Client>();
        client.Connect(true);
    }

    public void JoinLocal()
    {
        Client client = gameObject.AddComponent<Client>();
        client.Connect(false);
    }
}
