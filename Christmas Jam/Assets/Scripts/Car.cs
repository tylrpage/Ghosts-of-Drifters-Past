using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private Rigidbody Sphere;
    [SerializeField] private Transform Model;
    public float ForceMultMax;
    public float RotSpeed;
    public float Drag;
    public float DriftDrag;
    
    private readonly Vector3 _initialSphereOffset = new Vector3(0, 2f, 0);

    private Vector3 _inputDir;
    private float _speed;
    private float _forceMult;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (Input.GetKey(KeyCode.Space))
        {
            Sphere.drag = DriftDrag;
            _forceMult = ForceMultMax / 3f;
            transform.RotateAround(Sphere.position, Vector3.up, _inputDir.x * RotSpeed * Sphere.velocity.magnitude * Time.deltaTime);
        }
        else
        {
            Sphere.drag = Drag;
            _forceMult = ForceMultMax;
            transform.RotateAround(Sphere.position, Vector3.up, _inputDir.x * RotSpeed * _speed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        Sphere.AddForce(transform.forward * _inputDir.z * _forceMult, ForceMode.Force);
        _speed = Vector3.Dot(transform.forward, Sphere.velocity);
    }

    private void LateUpdate()
    {
        Model.position = Vector3.Lerp(Model.position, Sphere.position - _initialSphereOffset, 10f * Time.deltaTime);
        //Model.position = Sphere.position - _initialSphereOffset;
    }
}
