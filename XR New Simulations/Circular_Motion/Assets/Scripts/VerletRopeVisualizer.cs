using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class VerletRopeVisualizer : MonoBehaviour
{
    public Transform ballTransform;
    public int segmentCount = 15;
    public float ropeLength = 1.0f;
    public int constraintIterations = 15;
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

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segmentCount;
        maxSegmentLength = ropeLength / Mathf.Max(1, segmentCount - 1);

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

    private void FixedUpdate()
    {
        if (ballTransform == null) return;

        Simulate();
        ApplyConstraints();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void Simulate()
    {
        float subTimeStep = Time.fixedDeltaTime;
        Vector3 gravityStep = gravity * (subTimeStep * subTimeStep);

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 velocity = (nodes[i].currentPosition - nodes[i].previousPosition) * drag;
            nodes[i].previousPosition = nodes[i].currentPosition;
            nodes[i].currentPosition += velocity + gravityStep;
        }
    }

    private void HandleFloorCollision()
    {
        if (!enableFloorCollision) return;

        float minHeight = floorY + nodeRadius;

        for (int i = 0; i < segmentCount; i++)
        {
            if (nodes[i].currentPosition.y < minHeight)
            {
                nodes[i].currentPosition.y = minHeight;
                nodes[i].previousPosition.x = Mathf.Lerp(nodes[i].previousPosition.x, nodes[i].currentPosition.x, 0.2f);
                nodes[i].previousPosition.z = Mathf.Lerp(nodes[i].previousPosition.z, nodes[i].currentPosition.z, 0.2f);
            }
        }
    }

    private void ApplyConstraints()
    {
        nodes[0].currentPosition = transform.position;
        nodes[segmentCount - 1].currentPosition = ballTransform.position;

        for (int iteration = 0; iteration < constraintIterations; iteration++)
        {
            for (int i = 0; i < segmentCount - 1; i++)
            {
                RopeNode nodeA = nodes[i];
                RopeNode nodeB = nodes[i + 1];

                Vector3 delta = nodeB.currentPosition - nodeA.currentPosition;
                float currentDist = delta.magnitude;
                
                if (currentDist <= 0.0001f) continue;

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

            nodes[0].currentPosition = transform.position;
            nodes[segmentCount - 1].currentPosition = ballTransform.position;

            HandleFloorCollision();
        }
    }

    private void DrawRope()
    {
        if (lineRenderer == null || ballTransform == null || nodes == null || nodes.Length == 0) return;

        Vector3 startPos = transform.position;
        Vector3 endPos = ballTransform.position;
        Vector3 totalVec = endPos - startPos;
        float totalDist = totalVec.magnitude;

        // Check if rope is pulled taut or ball is kinematic (e.g. Cruise Control)
        Rigidbody ballRb = ballTransform.GetComponent<Rigidbody>();
        bool isKinematic = ballRb != null && ballRb.isKinematic;
        bool isTaut = totalDist >= (ropeLength - 0.03f) || isKinematic;

        Vector3[] positions = new Vector3[segmentCount];

        if (isTaut)
        {
            // Direct per-frame linear interpolation when taut guarantees zero visual jitter
            Vector3 straightDir = totalDist > 0.0001f ? (totalVec / totalDist) : Vector3.down;
            float lineSpan = Mathf.Min(totalDist, ropeLength);

            for (int i = 0; i < segmentCount; i++)
            {
                float t = (float)i / (segmentCount - 1);
                Vector3 targetPos = startPos + straightDir * (t * lineSpan);
                
                positions[i] = targetPos;

                // Sync internal physics nodes so they don't explode when returning to slack
                nodes[i].currentPosition = targetPos;
                nodes[i].previousPosition = targetPos;
            }
        }
        else
        {
            // Standard slack rope simulation display
            nodes[0].currentPosition = startPos;
            nodes[segmentCount - 1].currentPosition = endPos;

            for (int i = 0; i < segmentCount; i++)
            {
                positions[i] = nodes[i].currentPosition;
            }
        }

        lineRenderer.positionCount = segmentCount;
        lineRenderer.SetPositions(positions);
    }
}