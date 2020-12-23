using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTextureSetup : MonoBehaviour
{
    public Camera PortalCameraA;
    public Camera PortalCameraB;
    public Material CameraMatA;
    public Material CameraMatB;

    // Use this for initialization
    public void Start () {
        // Get rid of existing texture
	    if (PortalCameraA.targetTexture != null)
	    {
            PortalCameraA.targetTexture.Release();
	    }
        // Set the texture of the material to be our screensize so that the image is clear
        PortalCameraA.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
	    CameraMatA.mainTexture = PortalCameraA.targetTexture;

        // Camera B setup
        if (PortalCameraB.targetTexture != null)
        {
            PortalCameraB.targetTexture.Release();
        }
        // Set the texture of the material to be our screensize so that the image is clear
        PortalCameraB.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        CameraMatB.mainTexture = PortalCameraB.targetTexture;
    }
}
