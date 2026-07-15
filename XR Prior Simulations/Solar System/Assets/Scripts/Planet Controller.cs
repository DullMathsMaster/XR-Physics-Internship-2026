using UnityEngine;

public class PlanetController : MonoBehaviour
{
    public float rotationTilt;
    public float orbitalInclination;
    public float mass;
    public float orbitRadius = 1e8f;
    public float planetRadius = 1.0f;

    public GameObject planetObject;
    public GameObject orbitPathObject;
    public GameObject orbitingObject;
    public GameObject Earth;
    public GameObject UI;
    public GameObject UI2;
    public GameObject Label;

    public bool clockwiseOrbit;
    public bool clockwiseRotation;
    public float earthHoursPerDay;

    public static float orbitScaleFactor = 1e-7f;
    public static float planetScaleFactor = 1e-4f;
    public static float AngularVelocityScaleFactor = 5e2f;

    private float earthAngularVelocity;
    private float G = 6.6743e-11f;

    private float scaledOrbitRadius;
    public float scaledPlanetDiameter;
    public float scaledOrbitAngularVelocity;

    private float unscaledOrbitAngularVelocity;
    private float scaledRotationAngularVelocity;

    void Start()
    {
        // Start all planets in enlarged model scale.
        planetScaleFactor = 1e-4f;

        unscaledOrbitAngularVelocity =
            -Mathf.Sqrt(
                G *
                orbitingObject.GetComponent<SunController2>().sunMass /
                Mathf.Pow(orbitRadius, 3f)
            );

        if (clockwiseOrbit)
        {
            unscaledOrbitAngularVelocity =
                -unscaledOrbitAngularVelocity;
        }

        scaledOrbitRadius =
            orbitRadius * orbitScaleFactor;

        orbitPathObject.transform.localScale =
            new Vector3(
                scaledOrbitRadius * 10f,
                scaledOrbitRadius * 10f,
                scaledOrbitRadius * 10f
            );

        planetObject.transform.position =
            new Vector3(
                scaledOrbitRadius,
                0f,
                0f
            );

        planetObject.transform.Rotate(
            0f,
            0f,
            rotationTilt
        );

        scaledPlanetDiameter =
            planetRadius * planetScaleFactor * 2f;

        planetObject.transform.localScale =
            new Vector3(
                scaledPlanetDiameter,
                scaledPlanetDiameter,
                scaledPlanetDiameter
            );

        if (UI != null)
        {
            UI.transform.position =
                planetObject.transform.position +
                new Vector3(
                    (scaledPlanetDiameter / 1.5f) + 2f,
                    0f,
                    0f
                );
        }

        if (UI2 != null)
        {
            UI2.transform.position =
                planetObject.transform.position +
                new Vector3(
                    (scaledPlanetDiameter / 1.5f) + 2f,
                    0f,
                    0f
                );
        }

        if (Label != null)
        {
            Label.transform.position =
                planetObject.transform.position +
                new Vector3(
                    0f,
                    (scaledPlanetDiameter / 2f) + 1f,
                    0f
                );
        }

        transform.Rotate(
            Vector3.up,
            Random.Range(0f, 360f)
        );

        transform.Rotate(
            Vector3.forward,
            orbitalInclination
        );
    }

    void Update()
    {
        scaledOrbitAngularVelocity =
            unscaledOrbitAngularVelocity *
            AngularVelocityScaleFactor;

        earthAngularVelocity =
            Earth.GetComponent<PlanetController>()
                .scaledOrbitAngularVelocity;

        scaledRotationAngularVelocity =
            earthAngularVelocity *
            8760f /
            earthHoursPerDay;

        transform.Rotate(
            Vector3.up,
            scaledOrbitAngularVelocity * Time.deltaTime
        );

        float rotationDirection =
            clockwiseRotation ? -1f : 1f;

        planetObject.transform.Rotate(
            Vector3.up,
            rotationDirection *
            scaledRotationAngularVelocity *
            Time.deltaTime
        );

        scaledPlanetDiameter =
            planetRadius * planetScaleFactor * 2f;

        planetObject.transform.localScale =
            new Vector3(
                scaledPlanetDiameter,
                scaledPlanetDiameter,
                scaledPlanetDiameter
            );
    }

    public void SetSpeed(float value)
    {
        AngularVelocityScaleFactor = value;
    }

    public void SetScaleFactor(float sliderValue)
    {
        planetScaleFactor = sliderValue;

        Debug.Log(
            "Planet slider = " +
            sliderValue +
            ", planet scale = " +
            planetScaleFactor
        );
    }
}