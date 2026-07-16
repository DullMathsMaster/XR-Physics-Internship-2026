using UnityEngine;
using TMPro;

public class TrackingButtonUI : MonoBehaviour
{
    [Header("References")]
    public Teleport teleportScript; // Drag your GameObject with the Teleport script here

    [Header("Textbox")]
    public TextMeshProUGUI infoTextBox;
    [TextArea] public string trackingOnText;
    [TextArea] public string trackingOffText;

    private bool isTrackingActive = false;

    private void Start()
    {
        SetTrackingState(false);
    }

    // Attach this directly to your tracking button's On Click() event
    public void ToggleTracking()
    {
        SetTrackingState(!isTrackingActive);
    }

    public void ReturnToBaseState()
    {
        SetTrackingState(false);
    }

    private void SetTrackingState(bool trackingOn)
    {
        isTrackingActive = trackingOn;

        // 1. Tell the teleport script whether to start or stop tracking frame-by-frame
        if (teleportScript != null)
        {
            if (trackingOn)
                teleportScript.StartTracking();
            else
                teleportScript.StopTracking();
        }

        // 2. Update your text field display if you have one assigned
        if (infoTextBox != null)
            infoTextBox.text = trackingOn ? trackingOnText : trackingOffText;
    }
}