using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawSatelliteOrbitPath : MonoBehaviour
{
    public enum OrbitType { Equatorial, Polar, Inclined, Retrograde, Stationary }

    [Header("Select Orbit Type")]
    public OrbitType selectedOrbit;

    [Header("Global Radius (Same for all)")]
    public float globalRadius = 4.0f;

    [Header("References")]
    public Transform satellitePivot;
    public Transform satelliteModel;
    
    [Tooltip("Drag your Earth model here so we can match its rotation speed")]
    public Transform earthModel; 

    [Header("Materials (Colour Coding)")]
    public Material equatorialMaterial;
    public Material polarMaterial;
    public Material inclinedMaterial;
    public Material retrogradeMaterial;
    public Material stationaryMaterial;

    [Header("Line Settings")]
    [Range(0.005f, 0.1f)] public float lineWidth = 0.015f;
    [Range(10, 100)] public int segments = 60;

    private LineRenderer line;
    private Vector3 currentTilt;

    //  Temporary variables for orbit cycling demonstration
    private float orbitCycleTimer = 0f;
    private const float cycleDuration = 4f;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        UpdateOrbitSettings();
    }

    void OnValidate()
    {
        if (!line) line = GetComponent<LineRenderer>();
        UpdateOrbitSettings();
    }

    void UpdateOrbitSettings()
    {
        Vector3 tilt = Vector3.zero;
        Material chosenMaterial = equatorialMaterial;
        
        float activeRadius = globalRadius;

        switch (selectedOrbit)
        {
            case OrbitType.Equatorial:
                tilt = Vector3.zero;
                activeRadius = globalRadius * 0.85f; // Closer to Earth
                chosenMaterial = equatorialMaterial;
                break;

            case OrbitType.Polar:
                tilt = new Vector3(90f, 0f, 0f); // Perfect North-South
                activeRadius = globalRadius * 1.0f; // Medium distance
                chosenMaterial = polarMaterial;
                break;

            case OrbitType.Inclined:
                tilt = new Vector3(45f, 0f, 0f); // Tilted 45 degrees
                activeRadius = globalRadius * 1.05f; // Slightly further out
                chosenMaterial = inclinedMaterial;
                break;

            case OrbitType.Retrograde:
                tilt = new Vector3(140f, 30f, 0f); 
                activeRadius = globalRadius * 0.9f; 
                chosenMaterial = retrogradeMaterial;
                break;

            case OrbitType.Stationary:
                tilt = Vector3.zero; // Flat on the equator
                activeRadius = globalRadius * 1.5f; // Real geostationary orbits are VERY far out!
                chosenMaterial = stationaryMaterial;
                break;
        }

        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        line.loop = true;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        
        if (chosenMaterial != null) line.sharedMaterial = chosenMaterial;

        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * activeRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * activeRadius;
            line.SetPosition(i, new Vector3(x, 0, z));
            angle += (360f / segments);
        }

        currentTilt = tilt;

        if (satellitePivot != null && satelliteModel != null)
        {
            satellitePivot.localEulerAngles = Vector3.zero;
            satelliteModel.localPosition = new Vector3(0f, 0f, activeRadius); 
        }
    }
    void Update()
    {
        if (earthModel != null)
        {
            // Follow the Earth's position perfectly
            transform.position = earthModel.position;

            // Extract Earth's axial tilt without inheriting its daily spinning speed
            Quaternion earthAxialTilt = Quaternion.FromToRotation(Vector3.up, earthModel.up);

            // Combine Earth's axial tilt with our specific orbital path tilt
            transform.rotation = earthAxialTilt * Quaternion.Euler(currentTilt);
        }

        // --- TEMPORARY AUTOMATIC ORBIT CYCLER ---
        orbitCycleTimer += Time.deltaTime;
        if (orbitCycleTimer >= cycleDuration)
        {
            orbitCycleTimer = 0f;

            int totalOrbitsCount = System.Enum.GetValues(typeof(OrbitType)).Length;
            int nextOrbitIndex = ((int)selectedOrbit + 1) % totalOrbitsCount;
            
            selectedOrbit = (OrbitType)nextOrbitIndex;

            UpdateOrbitSettings();
            Debug.Log($"Orbit automatically cycled to: {selectedOrbit}");
        }
    }
}