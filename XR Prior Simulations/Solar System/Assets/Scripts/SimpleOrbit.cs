using UnityEngine;

public class KeplerOrbitWithInclination : MonoBehaviour
{
    [Header("Orbit Settings")]
    public GameObject sunObject;
    public float semiMajorAxis = 50f;  
    public float semiMinorAxis = 40f;  
    public float orbitSpeed = 1f;      
    [Range(-90f, 90f)] 
    public float inclination = 15f;    // Tilt angle in degrees

    [Header("Planet Rotation")]
    public float rotationSpeed = 50f;  

    private float currentAngle = 0f;

    void Start()
    {
        currentAngle = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (sunObject == null) return;

        if (semiMajorAxis < semiMinorAxis) return;

        currentAngle += orbitSpeed * Time.deltaTime;

        // Calculate linear eccentricity (focus point)
        float focusOffset = Mathf.Sqrt((semiMajorAxis * semiMajorAxis) - (semiMinorAxis * semiMinorAxis));

        // Coordintes relative to the ellipse centre
        float ellipseX = Mathf.Cos(currentAngle) * semiMajorAxis;
        float ellipseZ = Mathf.Sin(currentAngle) * semiMinorAxis;

        // Shift the flat position so the focus is at (0,0,0) before tilting
        Vector3 flatPosition = new Vector3(ellipseX - focusOffset, 0f, ellipseZ);

        // Orbital inclination implementation
        Quaternion tiltRotation = Quaternion.Euler(0f, 0f, inclination);
        Vector3 tiltedPosition = tiltRotation * flatPosition;

        transform.position = sunObject.transform.position + tiltedPosition;

        // transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}