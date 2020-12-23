using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public Transform Portal1;
    public Transform Portal2;
    public Transform Player;
	
	// Update is called once per frame
	void Update ()
    {
        if (Player != null)
        {
            // Get the offset from player 1 to the entrance portal
            Vector3 playerOffsetFromPortal1 = Player.position - Portal1.position;

            // Get the difference in rotation from exit to entrance
            Quaternion portalRotationDifference = Portal2.rotation * Quaternion.Inverse(Portal1.rotation);
            // Rotate that angle 180 degrees on the up axis because my portals face each other
            portalRotationDifference = portalRotationDifference * Quaternion.AngleAxis(180f, Vector3.up);
            // Calculate the new direction of the camera
            Vector3 newCameraDirection = portalRotationDifference * Player.forward;

            // Set the position to my offset from the entrance with the rotational difference applied
            transform.position = Portal2.position + portalRotationDifference * playerOffsetFromPortal1;
            // Have the camera look in the direction of the calculated direction
            transform.rotation = Quaternion.LookRotation(newCameraDirection);
        }
        else
        {
            //Player = GameLogicRef.PlayerCamera.transform;
            Player = Camera.main.transform;
        }
	}
}
