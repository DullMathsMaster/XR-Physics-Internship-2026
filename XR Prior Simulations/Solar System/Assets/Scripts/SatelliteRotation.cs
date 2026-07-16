using UnityEngine;

public class SatelliteRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("How much faster non-stationary satellites move relative to Earth's baseline speed.")]
    public float speedMultiplier = 2.0f;
    
    [Tooltip("The standard radius where an orbit would naturally match Earth's rotation speed.")]
    public float physicalStationaryRadius = 4.0f;
    
    [Header("References")]
    public DrawSatelliteOrbitPath orbitCustomizer; 
    public Transform earthModel;          

    private float lastEarthY;

    void Start()
    {
        if (earthModel != null)
        {
            lastEarthY = earthModel.localEulerAngles.y;
        }
    }

    void Update()
    {
        if (orbitCustomizer == null || earthModel == null) return;

        // Earth's dynamic speed (degrees per second)
        float currentEarthY = earthModel.localEulerAngles.y;
        float earthSpeed = 0f;

        if (Time.deltaTime > 0f)
        {
            earthSpeed = Mathf.DeltaAngle(lastEarthY, currentEarthY) / Time.deltaTime;
        }
        lastEarthY = currentEarthY;

        float radius = orbitCustomizer.globalRadius;
        if (radius <= 0.05f) radius = 0.05f; // Protect against division by zero

        if (orbitCustomizer.selectedOrbit == DrawSatelliteOrbitPath.OrbitType.Stationary)
        {
            // Stationary orbits must perfectly match Earth's exact rotation speed
            transform.Rotate(Vector3.up, earthSpeed * Time.deltaTime);
        }
        else
        {
            // Kepler's Third Law but scaled down as too fast to see
            float keplerFactor = Mathf.Pow(physicalStationaryRadius / radius, 1.5f);
            float calculatedSpeed = 0.03f * (earthSpeed * speedMultiplier * keplerFactor);

            if (orbitCustomizer.selectedOrbit == DrawSatelliteOrbitPath.OrbitType.Retrograde)
            {
                // Orbit backwards relative to Earth's rotation direction
                transform.Rotate(Vector3.up, -calculatedSpeed * Time.deltaTime);
            }
            else
            {
                // Orbit forwards (Equatorial, Polar, Inclined)
                transform.Rotate(Vector3.up, calculatedSpeed * Time.deltaTime);
            }
        }
    }
}