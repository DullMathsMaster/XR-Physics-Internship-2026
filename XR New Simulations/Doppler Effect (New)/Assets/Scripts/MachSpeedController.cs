using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachSpeedController : MonoBehaviour
{
    [Header("Doppler Sphere Components")]
    [SerializeField] private DopplerSphere dopplerSphere;
    [SerializeField] private WavefrontEmitter wavefrontEmitter;
    [SerializeField] private SonicBoomPlayer sonicBoomPlayer;

    [Header("UI")]
    [SerializeField] private Slider machSlider;
    [SerializeField] private TMP_Text speedText;

    [Header("Fixed Sound Settings")]
    [SerializeField] private float fixedSoundSpeed = 5f;

    [Header("Starting Mach Number")]
    [Range(0f, 1f)]
    [SerializeField] private float startingMach = 0f;

    [Header("Sonic Boom Threshold")]
    [Range(0.9f, 1f)]
    [SerializeField] private float sonicThreshold = 0.99f;

    private const float MinimumMach = 0f;
    private const float MaximumMach = 1f;

    private void Awake()
    {
        if (dopplerSphere == null)
        {
            dopplerSphere = FindFirstObjectByType<DopplerSphere>();
        }

        if (wavefrontEmitter == null)
        {
            wavefrontEmitter = FindFirstObjectByType<WavefrontEmitter>();
        }

        if (sonicBoomPlayer == null)
        {
            sonicBoomPlayer = FindFirstObjectByType<SonicBoomPlayer>();
        }
    }

    private void Start()
    {
        if (!CheckReferences())
        {
            enabled = false;
            return;
        }

        machSlider.minValue = MinimumMach;
        machSlider.maxValue = MaximumMach;
        machSlider.wholeNumbers = false;
        machSlider.value = startingMach;

        SetMachNumber(machSlider.value);
    }

    public void SetMachNumber(float machNumber)
    {
        machNumber = Mathf.Clamp01(machNumber);

        float sphereSpeed = machNumber * fixedSoundSpeed;

        dopplerSphere.movementSpeed = sphereSpeed;

        dopplerSphere.speedOfSound = fixedSoundSpeed;
        wavefrontEmitter.waveSpeed = fixedSoundSpeed;

        bool sonicMode = machNumber >= sonicThreshold;

        dopplerSphere.SetSonicMode(sonicMode);

        if (sonicBoomPlayer != null)
        {
            sonicBoomPlayer.SetArmed(sonicMode);
        }

        UpdateText(machNumber, sphereSpeed, sonicMode);
    }

    private void UpdateText(
        float machNumber,
        float sphereSpeed,
        bool sonicMode
    )
    {
        if (speedText == null)
        {
            return;
        }

        if (sonicMode)
        {
            speedText.text =
                $"Mach {machNumber:F2}\n" +
                $"Sphere speed: {sphereSpeed:F2} units/s\n" +
                "Sonic boom mode";
        }
        else
        {
            speedText.text =
                $"Mach {machNumber:F2}\n" +
                $"Sphere speed: {sphereSpeed:F2} units/s";
        }
    }

    private bool CheckReferences()
    {
        bool referencesValid = true;

        if (dopplerSphere == null)
        {
            Debug.LogError(
                "MachSpeedController could not find DopplerSphere.",
                this
            );

            referencesValid = false;
        }

        if (wavefrontEmitter == null)
        {
            Debug.LogError(
                "MachSpeedController could not find WavefrontEmitter.",
                this
            );

            referencesValid = false;
        }

        if (machSlider == null)
        {
            Debug.LogError(
                "MachSpeedController: Mach Slider has not been assigned.",
                this
            );

            referencesValid = false;
        }

        return referencesValid;
    }
}