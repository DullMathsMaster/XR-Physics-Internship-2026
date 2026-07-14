using UnityEngine;
using UnityEngine.UI;

public class SeasonsButtonUI : MonoBehaviour
{
    [Header("Canvases")]
    public Canvas earthMainCanvas;
    public Canvas earthSeasonsCanvas;
    public Canvas[] otherPlanetCanvases;

    [Header("Slider")]
    public Slider targetSlider;

    private bool isSeasonsActive = false;

    // Check if seasons is active
    public bool IsSeasonsOn()
    {
        return isSeasonsActive;
    }

    public void ToggleSeasons()
    {
        if (isSeasonsActive)
        {
            if (earthMainCanvas != null && earthMainCanvas.enabled)
            {
                ReturnToBaseState();
            }
            else
            {
                ReturnToBaseStateNoTextBoxes();
            }
            return;
        }

        isSeasonsActive = true;
        PausePlanets(true);

        if (earthSeasonsCanvas != null)
        {
            earthSeasonsCanvas.enabled = true;
        }

        if (earthMainCanvas != null && !earthMainCanvas.enabled)
        {
            SetAllOtherCanvases(false);
        }
    }

    public void ReturnToBaseState()
    {
        isSeasonsActive = false;
        PausePlanets(false);

        if (earthSeasonsCanvas != null)
        {
            earthSeasonsCanvas.enabled = false;
        }

        if (targetSlider != null)
        {
            targetSlider.value = 500f;
        }

        // Turn all main planet canvases ON
        if (earthMainCanvas != null)
        {
            earthMainCanvas.enabled = true;
        }
        SetAllOtherCanvases(true);
    }

    // ALTERNATIVE BASE STATE: Planets moving, all text boxes OFF, seasons OFF
    private void ReturnToBaseStateNoTextBoxes()
    {
        isSeasonsActive = false;
        PausePlanets(false);

        if (earthSeasonsCanvas != null)
        {
            earthSeasonsCanvas.enabled = false;
        }

        if (targetSlider != null)
        {
            targetSlider.value = 500f;
        }

        // Turn all main planet canvases OFF
        if (earthMainCanvas != null)
        {
            earthMainCanvas.enabled = false;
        }
        SetAllOtherCanvases(false);
    }

    private void SetAllOtherCanvases(bool state)
    {
        if (otherPlanetCanvases != null)
        {
            foreach (Canvas canvas in otherPlanetCanvases)
            {
                if (canvas != null)
                {
                    canvas.enabled = state;
                }
            }
        }
    }

    private void PausePlanets(bool shouldPause)
    {
        Debug.Log(shouldPause ? "Planets Paused" : "Planets Resumed");
    }
}