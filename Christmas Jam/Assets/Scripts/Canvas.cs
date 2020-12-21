using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas : MonoBehaviour
{
    public void Host()
    {
        gameObject.AddComponent<Server>();
    }

    public void Join()
    {
        gameObject.AddComponent<Client>();
    }
}
