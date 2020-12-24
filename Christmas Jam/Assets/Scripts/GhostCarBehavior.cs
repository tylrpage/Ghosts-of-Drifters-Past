using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostCarBehavior : MonoBehaviour
{
    [SerializeField] private Transform TimeTextTransform;
    private TextMesh _textMesh;
    private Vector3 _nextPosition;
    private Quaternion _nextRotation;
    private Transform _cameraTransform;

    private readonly float _lerpSpeed = 5f;
    private readonly float _maxDistance = 10f;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        _cameraTransform = GameObject.Find("Camera").transform;
        SceneManager.sceneLoaded += delegate(Scene arg0, LoadSceneMode mode)
        {
            _cameraTransform = GameObject.Find("Camera").transform;
        };
        _textMesh = TimeTextTransform.GetComponent<TextMesh>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _nextPosition = transform.position;
        _nextRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - _nextPosition).magnitude > _maxDistance)
        {
            transform.position = _nextPosition;
            transform.rotation = _nextRotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _nextPosition, _lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _nextRotation, _lerpSpeed * Time.deltaTime);
        }

        TimeTextTransform.forward = TimeTextTransform.position - _cameraTransform.position;
    }

    public void UpdateTransform(Vector3 position, Quaternion rotation)
    {
        _nextPosition = position;
        _nextRotation = rotation;
    }

    public void UpdatebestTime(float bestTime)
    {
        if (bestTime > 0)
        {
            _textMesh.gameObject.SetActive(true);
            _textMesh.text = (Mathf.Round(bestTime * 100) / 100f) + "s";
        }
        else
        {
            _textMesh.gameObject.SetActive(false);
        }
    }
}
