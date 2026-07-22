using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachSpeedController : MonoBehaviour
{
    [Header("Doppler Sphere Components")]
    [SerializeField] private DopplerSphere dopplerSphere;
    [SerializeField] private WavefrontEmitter wavefrontEmitter;

    [Header("UI")]
    [SerializeField] private Slider machSlider;
    [SerializeField] private TMP_Text speedText;

    private const float FixedSphereSpeed = 5f;
    private const float MinimumMach = 0.1f;
    private const float MaximumMach = 3f;

    [Header("Starting Mach Number")]
    [Range(MinimumMach, MaximumMach)]
    [SerializeField] private float startingMach = 0.5f;

    private void Awake()
    {
        if (dopplerSphere == null)
        {
            dopplerSphere = GetComponent<DopplerSphere>();
        }

        if (wavefrontEmitter == null)
        {
            wavefrontEmitter = GetComponent<WavefrontEmitter>();
        }
    }

    private void Start()
    {
        if (dopplerSphere == null)
        {
            Debug.LogError(
                "MachSpeedController could not find DopplerSphere.",
                this
            );

            enabled = false;
            return;
        }

        if (wavefrontEmitter == null)
        {
            Debug.LogError(
                "MachSpeedController could not find WavefrontEmitter.",
                this
            );

            enabled = false;
            return;
        }

        if (machSlider == null)
        {
            Debug.LogError(
                "MachSpeedController: Mach Slider has not been assigned.",
                this
            );

            enabled = false;
            return;
        }

        machSlider.minValue = MinimumMach;
        machSlider.maxValue = MaximumMach;
        machSlider.wholeNumbers = false;
        machSlider.value = startingMach;

        machSlider.onValueChanged.AddListener(SetMachNumber);

        SetMachNumber(machSlider.value);
    }

    private void OnDestroy()
    {
        if (machSlider != null)
        {
            machSlider.onValueChanged.RemoveListener(SetMachNumber);
        }
    }

    public void SetMachNumber(float machNumber)
    {
        machNumber = Mathf.Clamp(
            machNumber,
            MinimumMach,
            MaximumMach
        );

        float soundAndWaveSpeed =
            FixedSphereSpeed / machNumber;

        dopplerSphere.movementSpeed =
            FixedSphereSpeed;

        dopplerSphere.speedOfSound =
            soundAndWaveSpeed;

        wavefrontEmitter.waveSpeed =
            soundAndWaveSpeed;

        if (speedText != null)
        {
            speedText.text =
                $"Mach {machNumber:F2}\n" +
                $"Sphere speed: {FixedSphereSpeed:F1} units/s\n" +
                $"Sound speed: {soundAndWaveSpeed:F2} units/s";
        }
    }
}