using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeasonsButtonUI : MonoBehaviour
{
    [Header("Earth Canvases")]
    public Canvas earthMainCanvas;
    public Canvas earthSeasonsCanvas;

    [Header("Other Planet Main Canvases")]
    public Canvas[] otherPlanetCanvases;

    [Header("Optional")]
    public Slider targetSlider;

    [Header("Textbox")]
    public TextMeshProUGUI infoTextBox;
    [TextArea] public string seasonsOnText;
    [TextArea] public string seasonsOffText;

    private bool isSeasonsActive = false;

    private void Start()
    {
        SetSeasonsState(false);
    }

    public void ToggleSeasons()
    {
        SetSeasonsState(!isSeasonsActive);
    }

    public void ReturnToBaseState()
    {
        SetSeasonsState(false);
    }

    public bool IsSeasonsOn()
    {
        return isSeasonsActive;
    }

    private void SetSeasonsState(bool seasonsOn)
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

        if (infoTextBox != null)
            infoTextBox.text = seasonsOn ? seasonsOnText : seasonsOffText;

        PausePlanets(seasonsOn);
    }

    private void PausePlanets(bool shouldPause)
    {
        Debug.Log(shouldPause ? "Planets Paused" : "Planets Resumed");
    }
}