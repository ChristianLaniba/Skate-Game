using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cameraTransform;

    void LateUpdate()
    {
        if (cameraTransform == null)
        {
            if (Camera.main == null) return;
            cameraTransform = Camera.main.transform;
        }

        // Face the camera
        transform.rotation = Quaternion.LookRotation(
            transform.position - cameraTransform.position
        );
    }
}