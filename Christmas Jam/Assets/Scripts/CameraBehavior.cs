using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform ModelTarget;
    public Rigidbody sphereRigidbody;

    public Vector3 Offset;

    private bool _freeMode = false;

    private Vector3 IdealPosition => ModelTarget.position + ModelTarget.rotation * Offset * (1 + 0.05f * sphereRigidbody.velocity.magnitude);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            _freeMode = !_freeMode;
            if (_freeMode)
            {
                GameObject.Find("Car").GetComponent<Car>().enabled = false;
                GameObject.Find("Canvas").SetActive(false);
                transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, 0f));
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                GameObject.Find("Car").GetComponent<Car>().enabled = true;
                GameObject.Find("Canvas").SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (_freeMode)
        {
            Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 
                (Input.GetKey(KeyCode.Space) ? 1 : 0) - (Input.GetKey(KeyCode.LeftShift) ? 1 : 0),
            Input.GetAxis("Vertical"));
            float roll = ((Input.GetKey(KeyCode.Q) ? 1 : 0) - (Input.GetKey(KeyCode.E) ? 1 : 0)) * Time.deltaTime * 32f;
            float mult = (Input.GetMouseButton(0)) ? 3f : 1;
            Vector2 mouseDir = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime;
            transform.position += transform.rotation * dir * Time.deltaTime * 10f * mult;
            transform.Rotate(Vector3.left, mouseDir.y * 50f);
            transform.Rotate(Vector3.up, mouseDir.x * 50);
            transform.Rotate(Vector3.forward, roll);
        }
        else
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
}
