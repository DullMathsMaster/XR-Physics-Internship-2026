using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class VerletRopeVisualizer : MonoBehaviour
{
    public Transform ballTransform;
    public int segmentCount = 15;
    public float ropeLength = 1.0f;
    public int constraintIterations = 5;
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    [Range(0f, 1f)] public float drag = 0.98f;

    private LineRenderer lineRenderer;
    private RopeNode[] nodes;
    private float maxSegmentLength;

    [Header("Collision Settings")]
    public bool enableFloorCollision = true;
    public float floorY = 0f;    
    public float nodeRadius = 0.05f; 

    private class RopeNode
    {
        public Vector3 currentPosition;
        public Vector3 previousPosition;
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segmentCount;
        maxSegmentLength = ropeLength / (segmentCount - 1);

        nodes = new RopeNode[segmentCount];
        Vector3 startPos = transform.position;
        Vector3 endPos = ballTransform != null ? ballTransform.position : startPos + Vector3.down * ropeLength;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1);
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            nodes[i] = new RopeNode { currentPosition = pos, previousPosition = pos };
        }
    }

    void Update()
    {
        if (ballTransform == null) return;

        Simulate();
        ApplyConstraints();
        DrawRope();
    }

    void Simulate()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            // Calculate velocity with a dampening drag factor
            Vector3 velocity = (nodes[i].currentPosition - nodes[i].previousPosition) * drag;
            nodes[i].previousPosition = nodes[i].currentPosition;
            
            nodes[i].currentPosition += velocity + gravity * Time.deltaTime * Time.deltaTime;
        }
    }

    void HandleFloorCollision()
    {
        if (!enableFloorCollision) return;

        float minHeight = floorY + nodeRadius;

        for (int i = 0; i < segmentCount; i++)
        {
            if (nodes[i].currentPosition.y < minHeight)
            {
                nodes[i].currentPosition.y = minHeight;
                
                // Frictional effect 
                nodes[i].previousPosition.x = Mathf.Lerp(nodes[i].previousPosition.x, nodes[i].currentPosition.x, 0.2f);
                nodes[i].previousPosition.z = Mathf.Lerp(nodes[i].previousPosition.z, nodes[i].currentPosition.z, 0.2f);
            }
        }
    }

    void ApplyConstraints()
    {
        nodes[0].currentPosition = transform.position;
        nodes[segmentCount - 1].currentPosition = ballTransform.position;

        Vector3 totalDelta = ballTransform.position - transform.position;
        float totalDistance = totalDelta.magnitude;

        if (totalDistance >= ropeLength - 0.02f)
        {
            Vector3 direction = totalDelta.normalized;
            for (int i = 1; i < segmentCount - 1; i++)
            {
                float t = (float)i / (segmentCount - 1);
                nodes[i].currentPosition = transform.position + (direction * (totalDistance * t));
                nodes[i].previousPosition = nodes[i].currentPosition;
            }
            return;
        }

        for (int iteration = 0; iteration < constraintIterations; iteration++)
        {
            for (int i = 0; i < segmentCount - 1; i++)
            {
                RopeNode nodeA = nodes[i];
                RopeNode nodeB = nodes[i + 1];

                Vector3 delta = nodeB.currentPosition - nodeA.currentPosition;
                float currentDist = delta.magnitude;
                
                if (currentDist <= 0.001f) continue;

                float error = currentDist - maxSegmentLength; 
                Vector3 changeDir = delta / currentDist;
                Vector3 changeAmount = changeDir * error;

                if (i == 0)
                {
                    nodeB.currentPosition -= changeAmount; 
                }
                else if (i == segmentCount - 2)
                {
                    nodeA.currentPosition += changeAmount;
                }
                else
                {
                    nodeA.currentPosition += changeAmount * 0.5f;
                    nodeB.currentPosition -= changeAmount * 0.5f;
                }
            }
            HandleFloorCollision();
        }
    }

    void DrawRope()
    {
        Vector3[] positions = new Vector3[segmentCount];
        for (int i = 0; i < segmentCount; i++)
        {
            positions[i] = nodes[i].currentPosition;
        }
        lineRenderer.SetPositions(positions);
    }
}