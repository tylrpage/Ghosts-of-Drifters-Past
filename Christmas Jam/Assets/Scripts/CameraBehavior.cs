using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform SphereTarget;
    public Transform ModelTarget;

    public Vector3 Offset;

    private Vector3 IdealPosition => ModelTarget.position + ModelTarget.rotation * Offset;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = SphereTarget.position + Offset;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, IdealPosition, 2 * Time.deltaTime);
        //transform.position = IdealPosition;
        // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(SphereTarget.position - transform.position, Vector3.up),
        //     2 * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(ModelTarget.position - transform.position, Vector3.up);
    }
}
