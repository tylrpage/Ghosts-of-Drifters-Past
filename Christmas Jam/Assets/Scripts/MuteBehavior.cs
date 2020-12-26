using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteBehavior : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMute();
        }
    }

    public void ToggleMute()
    {
        AudioListener.pause = !AudioListener.pause;
    }
}
