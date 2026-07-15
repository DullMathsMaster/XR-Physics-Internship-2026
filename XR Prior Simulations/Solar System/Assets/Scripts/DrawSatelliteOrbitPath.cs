using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawSatelliteOrbitPath : MonoBehaviour
{
    [Header("Orbit Setup")]
    public float radius = 3.0f; // Must match the satellite's position offset!
    [Range(10, 100)] public int segments = 50; // How smooth the circle is

    private LineRenderer line;

    void Awake()
    {
        // Get the LineRenderer component on this object
        line = GetComponent<LineRenderer>();

        // Set the number of points (extra +1 to close the loop)
        line.positionCount = segments + 1;
        
        // Loop the line so the end connects to the start
        line.useWorldSpace = false; // Ensures the circle stays relative to Earth
        line.loop = true;

        line.startWidth = 0.005f;
        line.endWidth = 0.005f;

        CreatePoints();
    }

    void OnValidate()
    {
        // This allows you to see changes to the radius/segments
        // immediately in the Unity Editor without playing.
        if (Application.isPlaying || !line) return;
        CreatePoints();
    }

    // This generates the maths for the perfect circle
    public void CreatePoints()
    {
        if (line == null) line = GetComponent<LineRenderer>();
        line.positionCount = segments + 1;

        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            // Simple trigonometry to calculate points around the circle
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x, 0, z));
            angle += (360f / segments);
        }
    }
}