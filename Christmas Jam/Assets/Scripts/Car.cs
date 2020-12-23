using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private Rigidbody Sphere;
    [SerializeField] private Transform Model;
    public float ForceMultMax;
    public float RotSpeed;
    public float Drag;
    public float DriftDrag;

    private readonly Vector3 _initialSphereOffset = new Vector3(0, 0.75f, 0);

    private Vector3 _inputDir;
    private float _speed;
    private float _forceMult;
    private float _driftRotMult = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        float rotationMult = RotSpeed * Sphere.velocity.magnitude;

        if (Input.GetKey(KeyCode.Space))
        {
            _driftRotMult = Mathf.Clamp(_driftRotMult + Time.deltaTime * 0.5f, 1f, 1.5f);
            Sphere.drag = DriftDrag;
            _forceMult = ForceMultMax / 3f;
            transform.RotateAround(Sphere.position, Vector3.up, (_inputDir.x * _driftRotMult) * Mathf.Clamp(rotationMult, 0, 70f) * Time.deltaTime);
        }
        else
        {
            _driftRotMult = 1;
            Sphere.drag = Drag;
            _forceMult = ForceMultMax;
            transform.RotateAround(Sphere.position, Vector3.up, _inputDir.x * Mathf.Clamp(rotationMult, 0, 70f) * Time.deltaTime);
        }

        RaycastHit info;
        if (Physics.Raycast(Sphere.position, Vector3.down, out info, 5f, 1 << 9))
        {
            Model.rotation = Quaternion.Lerp(Model.rotation, Quaternion.FromToRotation(Vector3.up, info.normal) * transform.rotation, 5f * Time.deltaTime);
            //Model.rotation = Quaternion.Lerp(Model.rotation, alongSurface, Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        Sphere.AddForce(Model.forward * _inputDir.z * _forceMult, ForceMode.Force);
        Sphere.AddForce(Vector3.down * 5f, ForceMode.Force);
        _speed = Vector3.Dot(transform.forward, Sphere.velocity);
    }

    private void LateUpdate()
    {
        Model.position = Vector3.Lerp(Model.position, Sphere.position - _initialSphereOffset, 10f * Time.deltaTime);
        //Model.position = Sphere.position - _initialSphereOffset;
    }
}
