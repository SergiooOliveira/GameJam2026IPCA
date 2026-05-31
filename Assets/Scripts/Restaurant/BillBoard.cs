using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Camera mainCam;

    private void Start()
    {
        // Find the camera once at the start to save performance
        mainCam = Camera.main;
    }

    // We use LateUpdate so the canvas rotates AFTER the rat and camera have finished moving!
    private void LateUpdate()
    {
        if (mainCam != null)
        {
            // Force the canvas to face the exact same direction the camera is looking
            transform.forward = mainCam.transform.forward;
        }
    }
}
