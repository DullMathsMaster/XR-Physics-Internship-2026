using UnityEngine;

public class Teleport : MonoBehaviour
{
    [Header("Teleport Targets")]
    public GameObject MercurySphere;
    public GameObject VenusSphere;
    public GameObject EarthSphere;
    public GameObject MarsSphere;
    public GameObject JupiterSphere;
    public GameObject SaturnSphere;
    public GameObject UranusSphere;
    public GameObject NeptuneSphere;
    public GameObject PlutoSphere;
    public GameObject CeresSphere;

    [Header("Current Target Information")]
    public Vector3 PlanetLocation;
    public float PlanetScale;

    private bool isTracking = false;
    private int currentPlanetIndex = -1;

    private float zoomMultiplier = 1.0f;
    private float heightOffset = 0f;

    public void StartTracking()
    {
        if (currentPlanetIndex != -1)
        {
            isTracking = true;
        }
    }

    public void StopTracking()
    {
        isTracking = false;

        transform.SetParent(null);

        transform.rotation = Quaternion.Euler(
            0f,
            transform.eulerAngles.y,
            0f
        );
    }

    private void LateUpdate()
    {
        if (isTracking && currentPlanetIndex != -1)
        {
            HandleInputData(currentPlanetIndex);
        }
    }

    public void ZoomIn()
    {
        if (currentPlanetIndex != -1)
        {
            zoomMultiplier = 0.4f;
            heightOffset = -1.0f;

            HandleInputData(currentPlanetIndex);
        }
    }

    public void ResetZoom()
    {
        zoomMultiplier = 1.0f;
        heightOffset = 0f;

        if (currentPlanetIndex != -1)
        {
            HandleInputData(currentPlanetIndex);
        }
    }

    public void HandleInputData(int val)
    {
        currentPlanetIndex = val;

        GameObject selectedPlanet = null;

        switch (val)
        {
            case 0:
                selectedPlanet = MercurySphere;
                break;

            case 1:
                selectedPlanet = VenusSphere;
                break;

            case 2:
                selectedPlanet = EarthSphere;
                break;

            case 3:
                selectedPlanet = MarsSphere;
                break;

            case 4:
                selectedPlanet = JupiterSphere;
                break;

            case 5:
                selectedPlanet = SaturnSphere;
                break;

            case 6:
                selectedPlanet = UranusSphere;
                break;

            case 7:
                selectedPlanet = NeptuneSphere;
                break;

            case 8:
                selectedPlanet = PlutoSphere;
                break;

            case 9:
                selectedPlanet = CeresSphere;
                break;

            default:
                Debug.LogWarning(
                    "Teleport received an invalid planet index: " + val
                );
                return;
        }

        if (selectedPlanet == null)
        {
            Debug.LogWarning(
                "Teleport target has not been assigned for index: " + val
            );
            return;
        }

        PlanetLocation = selectedPlanet.transform.position;
        PlanetScale = selectedPlanet.transform.lossyScale.x;

        float distanceFromSun = Vector3.Magnitude(PlanetLocation);

        if (distanceFromSun <= 0.001f)
        {
            Debug.LogWarning(
                "Teleport target is too close to the world origin."
            );
            return;
        }

        float angleFromPlanet =
            ((400f / distanceFromSun) + (PlanetScale * 0.6f))
            * zoomMultiplier;

        float theta =
            Mathf.Atan2(PlanetLocation.z, PlanetLocation.x)
            - (angleFromPlanet * Mathf.Deg2Rad);

        float radius = Mathf.Sqrt(
            (PlanetLocation.x * PlanetLocation.x)
            + (PlanetLocation.z * PlanetLocation.z)
        );

        float targetY = PlanetLocation.y + heightOffset;

        transform.position = new Vector3(
            radius * Mathf.Cos(theta),
            targetY,
            radius * Mathf.Sin(theta)
        );

        Vector3 lookDirection =
            PlanetLocation - transform.position;

        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(
                lookDirection.normalized,
                Vector3.up
            );
        }
    }
}