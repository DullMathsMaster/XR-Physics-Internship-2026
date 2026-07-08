using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public Canvas earthUICanvas;
    public Canvas earthSeasonsCanvas;

    public void ToggleCanvases()
    {
        if (earthUICanvas != null)
        {
            // If it's enabled, this disables it. If disabled, it enables it.
            earthUICanvas.enabled = !earthUICanvas.enabled;
        }

        if (earthSeasonsCanvas != null)
        {
            earthSeasonsCanvas.enabled = !earthSeasonsCanvas.enabled;
        }
    }
}