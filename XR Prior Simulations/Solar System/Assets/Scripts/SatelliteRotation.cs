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

    void LateUpdate()
{
    // Instead of locking the whole rotation to zero, 
    // we only cancel out the roll/tilt from the parent Earth
    if (transform.parent != null)
    {
        Vector3 parentRot = transform.parent.eulerAngles;
        // This keeps our local Y rotation (the orbital spin) working, 
        // while counter-acting the parent Earth's rotation!
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y - parentRot.y, 0);
    }
}
    }