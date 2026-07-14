using UnityEngine;
using Oculus.Interaction;

public class GrabAmplifiedTransformer : MonoBehaviour, ITransformer
{
    [Tooltip("How much faster the canvas moves compared to your hand. 1 = normal, 2 = twice as fast.")]
    public float movementMultiplier = 1.5f;

    [Tooltip("Should the menu pivot to face you while you drag it?")]
    public bool faceUserWhileDragging = true;

    private IGrabbable _grabbable;
    private Pose _previousGrabPose;
    private Transform headCamera;

    void Start()
    {
        // Automatically find the VR headset
        if (Camera.main != null)
        {
            headCamera = Camera.main.transform;
        }
    }

    public void Initialize(IGrabbable grabbable)
    {
        _grabbable = grabbable;
    }

    public void BeginTransform()
    {
        // Save the hand position when we start grabbing
        _previousGrabPose = _grabbable.GrabPoints[0];
    }

    public void UpdateTransform()
    {
        Pose currentGrabPose = _grabbable.GrabPoints[0];

        // --- 1. AMPLIFIED MOVEMENT ---
        Vector3 handMovementDelta = currentGrabPose.position - _previousGrabPose.position;
        Vector3 amplifiedMovement = handMovementDelta * movementMultiplier;
        _grabbable.Transform.position += amplifiedMovement;

        // --- 2. ROTATE TO FACE USER ---
        if (faceUserWhileDragging && headCamera != null)
        {
            // Calculate direction from headset to menu
            Vector3 lookDirection = _grabbable.Transform.position + (_grabbable.Transform.position - headCamera.position);
            
            // Keep the menu perfectly upright (don't tilt up/down)
            lookDirection.y = _grabbable.Transform.position.y;
            
            // Apply the rotation
            _grabbable.Transform.LookAt(lookDirection, Vector3.up);
        }

        // Save for the next frame
        _previousGrabPose = currentGrabPose;
    }

    public void EndTransform() { }
}