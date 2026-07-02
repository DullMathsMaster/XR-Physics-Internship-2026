using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderLabel : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The text shown will be formatted using this string.  {0} is replaced with the actual value")]
    private string formatText = "{0:F2}";

    private TextMeshProUGUI tmproText;

    private void Start()
    {
        tmproText = GetComponent<TextMeshProUGUI>();
        if (tmproText != null)
        {
            UnityEngine.Debug.Log("ComponentFound");
        }

        GetComponentInParent<Slider>().onValueChanged.AddListener(HandleValueChanged);
    }

    private void HandleValueChanged(float value)
    {
        tmproText.text = value.ToString("#.00");
    }
}
