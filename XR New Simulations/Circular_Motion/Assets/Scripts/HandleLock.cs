using UnityEngine;

public class HandleLock : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private ForceVisualiser forceVisualiser;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private Vector3 lockedPosition;
    private Quaternion lockedRotation;
    private bool isLocked = false;

    private void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (forceVisualiser == null)
        {
            forceVisualiser = GetComponentInParent<ForceVisualiser>();
        }
    }

    private void Update()
    {
        if (forceVisualiser == null) return;

        bool cruiseActive = forceVisualiser.IsCruiseControl;

        // Handle State Transitions
        if (cruiseActive && !isLocked)
        {
            LockHandle();
        }
        else if (!cruiseActive && isLocked)
        {
            UnlockHandle();
        }

        // Maintain fixed position while locked
        if (isLocked)
        {
            transform.position = lockedPosition;
            transform.rotation = lockedRotation;
        }
    }

    private void LockHandle()
    {
        isLocked = true;
        lockedPosition = transform.position;
        lockedRotation = transform.rotation;

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        if (grabInteractable != null)
        {
            if (grabInteractable.isSelected)
            {
                grabInteractable.interactionManager.SelectExit(
                    (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)grabInteractable.firstInteractorSelecting,
                    (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)grabInteractable
                );
            }
            grabInteractable.enabled = false;
        }
    }

    private void UnlockHandle()
    {
        isLocked = false;

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        if (grabInteractable != null)
        {
            grabInteractable.enabled = true;
        }
    }
}