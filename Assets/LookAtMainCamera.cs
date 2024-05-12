using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Find the main camera in the scene
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            //Debug.LogError("Main camera not found in the scene!");
        }
    }

    void Update()
    {
        if (mainCamera != null)
        {
            // Calculate the direction to the main camera only on the y-axis
            Vector3 direction = mainCamera.transform.position - transform.position;
            direction.y = 0; // Ignore the y component

            // Rotate the object to look at the main camera only on the y-axis
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
        }
    }
}

}
