using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PortalTeleportor : MonoBehaviour
{
    public Transform Reciever;

    private GameObject player;
    private bool playerIsOverlapping = false;

	// Use this for initialization
	void Start ()
	{
	    player = null;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (playerIsOverlapping)
	    {
	        Transform playerTransform = player.transform;

            // Use the dot product to determine exactly when we finish going through
            // and to make sure we are entering in the correct direction
            Vector3 portalToPlayer = playerTransform.position - transform.position;
	        float dotProduct = Vector3.Dot(transform.up.normalized, portalToPlayer.normalized);
            // If this is true the player has moved across the portal
            if (dotProduct < 0)
	        {
                // Teleport!
                Debug.Log("Teleport");
                Transform parent = player.transform.parent.parent.parent;
                Transform model = player.transform.parent.parent;
                Transform sphere = player.transform.parent.parent.parent.Find("Sphere");

                // Calculate the rotation difference and apply it
	            // Get the difference in rotation from exit to entrance
	            Quaternion portalRotationDifference = Reciever.rotation * Quaternion.Inverse(transform.rotation);
	            // Rotate that angle 180 degrees on the up axis because my portals face each other
	            portalRotationDifference = portalRotationDifference * Quaternion.AngleAxis(180f, Vector3.up);
                //player.Rotate(Vector3.up, 180f - rotationDiff);
                //model.rotation = model.rotation * portalRotationDifference;
                RotateQuaternionAround(parent, model.position, portalRotationDifference);

                Vector3 positionOffset = portalRotationDifference * (model.position - transform.position);
                Vector3 sourceToReceiver = Reciever.position - transform.position;

                Transform cameraTransform = GameObject.Find("Camera").transform;
                Vector3 currentCameraOffset = cameraTransform.position - model.position;
                
                model.position = Reciever.transform.position + positionOffset;
                sphere.position = Reciever.transform.position + positionOffset + new Vector3(0, 1.5f, 0);
                cameraTransform.position = model.position + currentCameraOffset;
                
                Debug.Log("Model: " + model.position);
                Debug.Log("Sphere: " + sphere.position);
                Debug.Log("Camera: " + cameraTransform.position);

                GameTimer.RegisterLapCompletion();

                playerIsOverlapping = false;
            }  
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
	        player = other.gameObject;
            // Only set the overlapping if they entered from the correct side
            Vector3 portalToPlayer = player.transform.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up.normalized, portalToPlayer.normalized);
            if (dotProduct > 0)
            {
                playerIsOverlapping = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsOverlapping = false;
        }
    }
    
    void RotateQuaternionAround (Transform transform, Vector3 pivotPoint, Quaternion rot)
    {
	    transform.position = rot * (transform.position - pivotPoint) + pivotPoint;
	    transform.rotation = rot * transform.rotation;
    }
}
