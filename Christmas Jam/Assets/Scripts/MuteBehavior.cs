using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteBehavior : MonoBehaviour
{
    [SerializeField] private AudioListener _listener;

    public void ToggleMute()
    {
        _listener.enabled = !_listener.enabled;
    }
}
