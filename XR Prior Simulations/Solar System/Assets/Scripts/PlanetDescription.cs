using UnityEngine;
using TMPro;

public class PlanetDescription : MonoBehaviour
{
    [Header("Text object")]
    public TMP_Text descriptionText;

    [Header("Background")]
    public RectTransform backgroundRect;

    [Header("Box Settings")]
    public float verticalPadding = 40f;
    public float minimumHeight = 100f;

    [Header("Descriptions")]
    [TextArea(4, 10)]
    public string ks2Text;

    [TextArea(4, 10)]
    public string ks3Text;

    [TextArea(4, 10)]
    public string ks4Text;

    [TextArea(4, 10)]
    public string ks5Text;

    private AgeGroup lastAge;

    // Keep whatever X position you set in Unity
    private float originalTextX;

    void Start()
    {
        if (AgeManager.Instance == null)
        {
            Debug.LogError("AgeManager not found.");
            return;
        }

        originalTextX = descriptionText.rectTransform.anchoredPosition.x;

        lastAge = AgeManager.Instance.CurrentAge;
        UpdateDescription();
    }

    void Update()
    {
        if (AgeManager.Instance == null)
            return;

        if (AgeManager.Instance.CurrentAge != lastAge)
        {
            lastAge = AgeManager.Instance.CurrentAge;
            UpdateDescription();
        }
    }

    private void UpdateDescription()
    {
        switch (AgeManager.Instance.CurrentAge)
        {
            case AgeGroup.KS2:
                descriptionText.text = ks2Text;
                break;

            case AgeGroup.KS3:
                descriptionText.text = ks3Text;
                break;

            case AgeGroup.KS4:
                descriptionText.text = ks4Text;
                break;

            case AgeGroup.KS5:
                descriptionText.text = ks5Text;
                break;
        }

        ResizePanel();
    }

    private void ResizePanel()
    {
        descriptionText.ForceMeshUpdate();

        float requiredTextHeight = descriptionText.GetPreferredValues(
            descriptionText.text,
            descriptionText.rectTransform.rect.width,
            Mathf.Infinity
        ).y;

        // Resize the text rect
        descriptionText.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            requiredTextHeight
        );

        // Keep original X, centre vertically
        descriptionText.rectTransform.anchoredPosition =
            new Vector2(originalTextX, 0f);

        // Resize the background
        float backgroundHeight = Mathf.Max(
            requiredTextHeight + verticalPadding,
            minimumHeight
        );

        backgroundRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            backgroundHeight
        );
    }
}