using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _windSource;
    [SerializeField] private AudioSource _engineSource;
    [SerializeField] private AudioSource _driftSource;
    [SerializeField] private AudioClip[] _accelerateClips;
    [SerializeField] private Rigidbody _car;
    [SerializeField] private Transform _model;
    public float MaxRange;
    public float MaxEngineVolume;
    public float MaxDriftVolume;
    public float DotForMaxDriftVolume;
    public float RequiredDriftSpeed;
    public float DotToStartDrift;

    private float min = 1;

    private void Update()
    {
        float speed = _car.velocity.magnitude;
        
        // wind volume
        _windSource.volume = Mathf.Min(Mathf.Pow(speed / MaxRange, 2), 1f);

        _engineSource.volume = Mathf.Min(Mathf.Pow(speed / MaxRange, 2), 1f) * MaxEngineVolume;
        float dot = Mathf.Abs(Vector3.Dot(_model.forward, _car.velocity.normalized));

        if (_driftSource.isPlaying)
        {
            if (speed < RequiredDriftSpeed)
            {
                _driftSource.Stop();
            }
            else
            {
                _driftSource.volume = Mathf.Min((1 - dot) / DotForMaxDriftVolume, 1f) * Mathf.Min(speed / MaxRange, 1f) * MaxDriftVolume;
            }
        }
        else if (dot > 0.1f && dot < DotToStartDrift && speed > 21f)
        {
            AudioClip randomClip = _accelerateClips[Random.Range(0, _accelerateClips.Length)];
            _driftSource.clip = randomClip;
            _driftSource.Play();
        }
    }
}
