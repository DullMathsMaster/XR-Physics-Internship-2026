using UnityEngine;

public class SatelliteRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float standardOrbitalSpeed = 30.0f;
    
    [Header("References")]
    public DrawSatelliteOrbitPath orbitCustomizer; // Updated to match the new class name
    public Transform earthModel;          

    void Update()
    {
        if (orbitCustomizer != null && orbitCustomizer.selectedOrbit == DrawSatelliteOrbitPath.OrbitType.Stationary)
        {
            if (earthModel != null)
            {
                Vector3 earthRotation = earthModel.localEulerAngles;
                transform.localEulerAngles = new Vector3(0f, earthRotation.y, 0f);
            }
        }
        else if (orbitCustomizer != null && orbitCustomizer.selectedOrbit == DrawSatelliteOrbitPath.OrbitType.Retrograde)
        {
            transform.Rotate(Vector3.up, -standardOrbitalSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(Vector3.up, standardOrbitalSpeed * Time.deltaTime);
        }
    }
}