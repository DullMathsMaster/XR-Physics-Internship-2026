using UnityEngine;

public class InfoButtonUI : MonoBehaviour
{
    [Header("All Planet Canvases")]
    [Tooltip("Drag the main info canvases for ALL your planets here")]
    public Canvas[] allPlanetCanvases;

    [Header("Seasons Button Reference")]
    [Tooltip("Drag the Seasons Button object here so we can read its state")]
    public SeasonsButtonUI seasonsButton;

    public void ToggleInformation()
    {
        // 1. If seasons is currently ON, return to base state (main canvases ON)
        if (seasonsButton != null && seasonsButton.IsSeasonsOn())
        {
            seasonsButton.ReturnToBaseState();
            return;
        }

        // 2. If seasons is NOT on, toggle the main canvases normally
        if (allPlanetCanvases != null && allPlanetCanvases.Length > 0)
        {
            bool currentStatus = allPlanetCanvases[0].enabled;
            bool targetStatus = !currentStatus;

            foreach (Canvas canvas in allPlanetCanvases)
            {
                if (canvas != null)
                {
                    canvas.enabled = targetStatus;
                }
            }
        }
    }
}