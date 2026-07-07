using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BillboardFaceCamera : MonoBehaviour
{
    private Transform mainCameraTransform;
    private XRBaseInteractable xrInteractable;

    void Start()
    {
        // Automatically find the VR headset camera
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        xrInteractable = GetComponent<XRBaseInteractable>();
    }

    void LateUpdate()
    {
        if (mainCameraTransform != null && xrInteractable != null && xrInteractable.isSelected)
        {
            // Make the canvas look at the camera
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                             mainCameraTransform.rotation * Vector3.up);
        }
    }
}