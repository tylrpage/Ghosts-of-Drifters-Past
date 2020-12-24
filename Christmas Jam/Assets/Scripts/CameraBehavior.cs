using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform ModelTarget;
    public Rigidbody sphereRigidbody;

    public Vector3 Offset;

    private Vector3 IdealPosition => ModelTarget.position + ModelTarget.rotation * Offset * (1 + 0.05f * sphereRigidbody.velocity.magnitude);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Slerp(transform.position - ModelTarget.position,
            IdealPosition - ModelTarget.position, 2 * Time.deltaTime) + ModelTarget.position;
        transform.position = Vector3.Lerp(transform.position, IdealPosition, 1 * Time.deltaTime);
        //transform.position = IdealPosition;
        // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(SphereTarget.position - transform.position, Vector3.up),
        //     2 * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(ModelTarget.position - transform.position, Vector3.up);
    }
}
