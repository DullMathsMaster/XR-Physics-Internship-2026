using UnityEngine;
using UnityEngine.UI;

public class SeasonsButtonUI : MonoBehaviour
{
    [Header("Canvases")]
    public Canvas earthMainCanvas;
    public Canvas earthSeasonsCanvas;

    [Header("Optional")]
    public Canvas[] otherPlanetCanvases;
    public Slider targetSlider;

    private bool isSeasonsActive;

    private void Start()
    {
        ApplyState(false);
    }

    public void ToggleSeasons()
    {
        ApplyState(!isSeasonsActive);
    }

    public void ReturnToBaseState()
    {
        ApplyState(false);
    }

    private void ApplyState(bool seasonsOn)
    {
        isSeasonsActive = seasonsOn;

        if (earthMainCanvas != null)
            earthMainCanvas.enabled = !seasonsOn;

        if (earthSeasonsCanvas != null)
            earthSeasonsCanvas.enabled = seasonsOn;

        if (otherPlanetCanvases != null)
        {
            foreach (Canvas canvas in otherPlanetCanvases)
            {
                if (canvas != null)
                    canvas.enabled = !seasonsOn;
            }
        }

        if (!seasonsOn && targetSlider != null)
            targetSlider.value = 500f;

        PausePlanets(seasonsOn);
    }

    private void PausePlanets(bool shouldPause)
    {
        Debug.Log(shouldPause ? "Planets Paused" : "Planets Resumed");
    }

    public bool IsSeasonsOn()
    {
        return isSeasonsActive;
    }
}