using UnityEngine;

public class ForceVisualizer : MonoBehaviour
{
    public Transform pivot; // string's anchor point here
    public float forceScale = 0.1f; // Adjusts the length of the arrows

    private LineRenderer gravityLine;
    private LineRenderer tensionLine;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        gravityLine = CreateLine(Color.red, "GravityLine");
        tensionLine = CreateLine(Color.blue, "TensionLine");
    }

    void Update()
    {
        if (pivot == null || rb == null) return;

        Vector3 ballPosition = transform.position;

        Vector3 gravityForce = Physics.gravity * rb.mass;
        DrawArrow(gravityLine, ballPosition, gravityForce * forceScale);

        // Calculate Tension Force Vector (NOT PHYSICALLY ACCURATE YET)
        Vector3 stringDirection = (pivot.position - ballPosition).normalized;
        float distance = Vector3.Distance(pivot.position, ballPosition);
        
        // Approximating tension for visualization (NOT PHYSICALLY ACCURATE YET)
        float gravityAlongString = Vector3.Dot(gravityForce, stringDirection);
        float centripetalForce = (rb.mass * Mathf.Pow(rb.linearVelocity.magnitude, 2)) / distance;
        float totalTensionMagnitude = -gravityAlongString + centripetalForce;
        
        Vector3 tensionForce = stringDirection * totalTensionMagnitude;
        DrawArrow(tensionLine, ballPosition, tensionForce * forceScale);
    }

    LineRenderer CreateLine(Color color, string name)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(transform);
        
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.positionCount = 2;
        return lr;
    }

    void DrawArrow(LineRenderer lr, Vector3 start, Vector3 direction)
    {
        lr.SetPosition(0, start);
        lr.SetPosition(1, start + direction);
    }
}