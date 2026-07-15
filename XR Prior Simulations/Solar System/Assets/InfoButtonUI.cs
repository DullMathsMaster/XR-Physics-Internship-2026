using UnityEngine;
using TMPro;

public class InfoButtonUI : MonoBehaviour
{
    [Header("All 7 Main Planet Canvases")]
    public Canvas[] allPlanetCanvases;

    [Header("Seasons Reference")]
    public SeasonsButtonUI seasonsButton;

    [Header("Textbox")]
    public TextMeshProUGUI infoTextBox;
    [TextArea] public string infoShownText;
    [TextArea] public string infoHiddenText;

    public void ToggleInformation()
    {
        if (seasonsButton != null && seasonsButton.IsSeasonsOn())
        {
            seasonsButton.ReturnToBaseState();

            if (infoTextBox != null)
                infoTextBox.text = infoHiddenText;
            return;
        }

        bool currentState = GetCurrentCanvasState();
        bool newState = !currentState;

        SetAllPlanetCanvases(newState);

        if (infoTextBox != null)
            infoTextBox.text = newState ? infoShownText : infoHiddenText;
    }

    private bool GetCurrentCanvasState()
    {
        if (allPlanetCanvases == null)
            return false;

        foreach (Canvas canvas in allPlanetCanvases)
        {
            if (canvas != null)
                return canvas.enabled;
        }

        return false;
    }

    private void SetAllPlanetCanvases(bool state)
    {
        if (allPlanetCanvases == null)
            return;

        foreach (Canvas canvas in allPlanetCanvases)
        {
            if (canvas != null)
                canvas.enabled = state;
        }
    }
}