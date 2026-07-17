using UnityEngine;
using TMPro;

public class PlanetDescription : MonoBehaviour
{
    [Header("Text object")]
    public TMP_Text descriptionText;

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

    private void Start()
    {
        lastAge = AgeManager.Instance.CurrentAge;
        UpdateDescription();
    }

    private void Update()
    {
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
    }
}