using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private Text GameClock;
    [SerializeField] private Text HelpText;
    [SerializeField] private Text BestText;

    [SerializeField] private Transform Car;
    
    public static float LapTime { get; private set; }
    public static bool Running { get; private set; } = false;
    public static float BestTime = -1f;

    private Vector3 _initialPos;
    private Quaternion _initialRot;

    private void Start()
    {
        _initialPos = Car.position;
        _initialRot = Car.rotation;
    }

    private void Update()
    {
        if (PlayerPrefs.HasKey("Time"))
        {
            BestTime = PlayerPrefs.GetFloat("Time");
            BestText.enabled = true;
            BestText.text = "Best: " + (Mathf.Round(BestTime * 100) / 100f) + "s";
        }
        else
        {
            BestText.enabled = false;
        }
        
        if (Running)
        {
            HelpText.enabled = false;
            LapTime += Time.deltaTime;
            if (GameClock != null)
            {
                GameClock.text = (Mathf.Round(LapTime * 100) / 100f).ToString() + "s";
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Game");
            StopTimer();
        }
    }

    public static void StopTimer()
    {
        Running = false;
    }

    public static void RestartTimer()
    {
        LapTime = 0;
        Running = true;
    }

    public static void RegisterLapCompletion()
    {
        if (!PlayerPrefs.HasKey("Time") || PlayerPrefs.GetFloat("Time") > GameTimer.LapTime)
        {
            // Record!
            PlayerPrefs.SetFloat("Time", Mathf.Round(LapTime * 100) / 100f);
        }
        RestartTimer();
    }
}
