using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private Rigidbody Sphere;
    [SerializeField] private Transform Model;
    public float ForceMult;
    public float RotSpeed;
    
    private readonly Vector3 _initialSphereOffset = new Vector3(0, 0.5f, 0);

    private Vector3 _inputDir;
    private float _speed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.RotateAround(Sphere.position, Vector3.up, _inputDir.x * RotSpeed * _speed * Time.deltaTime);

        Model.position = Sphere.position - _initialSphereOffset;
        //Model.forward = Sphere.velocity;
    }

    private void FixedUpdate()
    {
        Sphere.AddForce(transform.forward * _inputDir.z * ForceMult, ForceMode.Force);
        _speed = Sphere.velocity.magnitude;
    }
}
