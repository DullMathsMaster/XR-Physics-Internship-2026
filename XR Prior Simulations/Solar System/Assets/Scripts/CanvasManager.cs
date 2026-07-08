using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public Canvas earthUICanvas;
    public Canvas earthSeasonsCanvas;
    public Slider targetSlider;

    public void ToggleCanvases()
    {
        if (earthUICanvas != null)
        {
            earthUICanvas.enabled = !earthUICanvas.enabled;
        }

        if (earthSeasonsCanvas != null)
        {
            earthSeasonsCanvas.enabled = !earthSeasonsCanvas.enabled;

            // Check if earthSeasonsCanvas was just turned OFF (false)
            if (!earthSeasonsCanvas.enabled && targetSlider != null)
            {
                targetSlider.value = 500f; 
            }
        }
    }
}