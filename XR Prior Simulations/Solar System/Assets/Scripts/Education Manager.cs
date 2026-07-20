using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EducationFeatureManager : MonoBehaviour
{
    [Header("KS3 Features")]
    public GameObject asteroidBelt;

    [Header("KS4 Features")]
    public GameObject ceres;
    public GameObject pluto;

    [Header("KS5 Features")]
    public GameObject satellites;
    public PlanetController[] planets;

    [Header("Teleport Dropdown")]
    public TMP_Dropdown teleportDropdown;

    [Header("Menu Buttons")]
    public Button asteroidButton;
    public Button satelliteButton;

    private AgeGroup lastAge;

    private List<TMP_Dropdown.OptionData> allTeleportOptions;

    void Start()
    {
        allTeleportOptions =
            new List<TMP_Dropdown.OptionData>(
                teleportDropdown.options
            );

        lastAge =
            AgeManager.Instance.CurrentAge;

        UpdateFeatures();
    }

    void Update()
    {
        if (AgeManager.Instance.CurrentAge != lastAge)
        {
            lastAge =
                AgeManager.Instance.CurrentAge;

            UpdateFeatures();
        }
    }

    void UpdateFeatures()
    {
        AgeGroup age =
            AgeManager.Instance.CurrentAge;

        // KS3+
        bool showAsteroids =
            age >= AgeGroup.KS3;

        asteroidBelt.SetActive(
            showAsteroids
        );

        if (asteroidButton != null)
        {
            asteroidButton.interactable =
                showAsteroids;
        }

        // KS4+
        bool showCeres =
            age >= AgeGroup.KS4;

        ceres.SetActive(
            showCeres
        );

        bool showPluto =
            age >= AgeGroup.KS4;

        pluto.SetActive(
            showPluto
        );

        // KS5 only
        bool showSatellites =
            age == AgeGroup.KS5;

        satellites.SetActive(
            showSatellites
        );

        if (satelliteButton != null)
        {
            satelliteButton.interactable =
                showSatellites;
        }

        // KS5 only
        bool useInclination =
            age == AgeGroup.KS5;

        foreach (PlanetController planet in planets)
        {
            if (planet != null)
            {
                planet.SetOrbitalInclination(
                    useInclination
                );
            }
        }

        UpdateTeleportDropdown(
            showCeres,
            showPluto
        );
    }

    void UpdateTeleportDropdown(
        bool showCeres,
        bool showPluto
    )
    {
        teleportDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> availableOptions =
            new List<TMP_Dropdown.OptionData>();

        foreach (TMP_Dropdown.OptionData option in allTeleportOptions)
        {
            if (option.text == "Ceres" && !showCeres)
            {
                continue;
            }

            if (option.text == "Pluto" && !showPluto)
            {
                continue;
            }

            availableOptions.Add(option);
        }

        teleportDropdown.AddOptions(
            availableOptions
        );

        teleportDropdown.value = 0;
        teleportDropdown.RefreshShownValue();
    }
}