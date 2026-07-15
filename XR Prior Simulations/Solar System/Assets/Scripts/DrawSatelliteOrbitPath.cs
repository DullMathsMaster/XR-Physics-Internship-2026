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

        switch (selectedOrbit)
        {
            case OrbitType.Equatorial:
                tilt = Vector3.zero;
                chosenMaterial = equatorialMaterial;
                break;
            case OrbitType.Polar:
                tilt = new Vector3(90f, 0f, 0f);
                chosenMaterial = polarMaterial;
                break;
            case OrbitType.Inclined:
                tilt = new Vector3(45f, 0f, 45f);
                chosenMaterial = inclinedMaterial;
                break;
            case OrbitType.Retrograde:
                tilt = new Vector3(180f, 15f, 0f);
                chosenMaterial = retrogradeMaterial;
                break;
            case OrbitType.Stationary:
                tilt = Vector3.zero; 
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
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * globalRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * globalRadius;
            line.SetPosition(i, new Vector3(x, 0, z));
            angle += (360f / segments);
        }

        transform.localEulerAngles = tilt;

        if (satellitePivot != null && satelliteModel != null)
        {
            satellitePivot.localEulerAngles = tilt;
            satelliteModel.localPosition = new Vector3(globalRadius, 0f, 0f); 
        }
    }

    void Update()
    {
        if (earthModel != null)
        {
            // Follow the Earth's position perfectly, keeping our offset anchor!
            transform.position = earthModel.position;
        }
    }
}