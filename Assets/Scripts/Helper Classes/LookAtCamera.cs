using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour
{
    public Camera targetCamera;
    public float delay = 0.5f; // Time delay between updates

    private void Start()
    {
        if (!targetCamera)
        {
            targetCamera = Camera.main;
        }

        // Start the coroutine to periodically look at the camera
        StartCoroutine(LookAtCameraWithDelay());
    }

    private IEnumerator LookAtCameraWithDelay()
    {
        while (true)
        {
            if (targetCamera)
            {
                // Make the object look at the camera
                transform.LookAt(targetCamera.transform);
            }

            // Wait for the specified delay before the next update
            yield return new WaitForSeconds(delay);
        }
    }
}