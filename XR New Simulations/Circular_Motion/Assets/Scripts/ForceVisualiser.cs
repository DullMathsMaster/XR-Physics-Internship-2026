using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class ForceVisualiser : MonoBehaviour
{
    [Header("Pendulum Setup")]
    [SerializeField] private Transform pivot;
    [SerializeField] private float stringLength = 1.0f;
    [SerializeField] private float ballRadius = 0.5f;

    [Header("Control Settings")]
    [SerializeField] private bool isCruiseControl = false;
    [SerializeField] private KeyCode toggleCruiseControlKey = KeyCode.C;

    public bool IsCruiseControl
    {
        get => isCruiseControl;
        set => isCruiseControl = value;
    }

    [Header("Arrow Setup")]
    [SerializeField] private ArrowVisual arrowPrefab;
    [SerializeField] private float forceScale = 0.05f;

    [Header("Colors")]
    [SerializeField] private Color gravityColor = Color.red;
    [SerializeField] private Color tensionColor = Color.blue;
    [SerializeField] private Color netForceColor = Color.yellow;

    [Header("UI / Text Display")]
    [SerializeField] private TextMeshPro readoutText;

    [Header("Graph Setup")]
    [SerializeField] private LineRenderer tensionGraph;
    [SerializeField] private LineRenderer centripetalGraph;
    [SerializeField] private int maxGraphPoints = 100;
    [SerializeField] private float graphScaleY = 0.05f;
    [Range(0.01f, 1f)][SerializeField] private float graphSmoothing = 0.15f; // Graph smoothing factor

    private ArrowVisual gravityArrow;
    private ArrowVisual tensionArrow;
    private ArrowVisual netForceArrow;
    private Rigidbody rb;

    private float smoothedTension = 0f;
    private float smoothedCentripetal = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (pivot != null && stringLength <= 0f)
        {
            stringLength = Vector3.Distance(transform.position, pivot.position);
        }

        if (arrowPrefab != null)
        {
            gravityArrow = Instantiate(arrowPrefab);
            tensionArrow = Instantiate(arrowPrefab);
            netForceArrow = Instantiate(arrowPrefab);
        }
        else
        {
            Debug.LogError("[ForceVisualiser] Please assign an Arrow Prefab in the Inspector!", this);
        }
    }

    private void FixedUpdate()
    {
        if (pivot == null || rb == null) return;

        // Hard-clamp ball distance so the string remains taut
        Vector3 stringVec = transform.position - pivot.position;
        float currentDist = stringVec.magnitude;

        if (currentDist > stringLength)
        {
            Vector3 outboundDir = stringVec / currentDist;
            transform.position = pivot.position + (outboundDir * stringLength);

            // Strip outbound velocity
            float outboundSpeed = Vector3.Dot(rb.linearVelocity, outboundDir);
            if (outboundSpeed > 0f)
            {
                rb.linearVelocity -= outboundDir * outboundSpeed;
            }
        }
    }

    private void Update()
    {
        // Toggle Cruise Control input shortcut
        if (Input.GetKeyDown(toggleCruiseControlKey))
        {
            isCruiseControl = !isCruiseControl;
        }

        if (pivot == null || rb == null || arrowPrefab == null) return;

        Vector3 ballPos = transform.position;

        // --- Physics Force Calculations ---
        Vector3 stringVec = pivot.position - ballPos;
        float dist = stringVec.magnitude;
        if (dist <= 0.0001f) return;

        Vector3 stringDir = stringVec / dist;

        Vector3 gravityForce = Physics.gravity * rb.mass;
        Vector3 tensionForce = Vector3.zero;

        float tensionMag = 0f;
        float centripetalMag = 0f;

        // Expanded buffer (0.08f) guarantees tension remains active when taut
        if (dist >= stringLength - 0.08f)
        {
            Vector3 velocity = rb.linearVelocity;
            Vector3 radialVel = Vector3.Project(velocity, stringDir);
            Vector3 tangentialVel = velocity - radialVel;

            centripetalMag = (rb.mass * tangentialVel.sqrMagnitude) / stringLength;
            float gravComp = -Vector3.Dot(gravityForce, stringDir);
            tensionMag = Mathf.Max(0f, gravComp + centripetalMag);
            tensionForce = stringDir * tensionMag;
        }

        Vector3 netForce = tensionForce + gravityForce;

        // Smooth graph values to eliminate sharp visual jitter
        smoothedTension = Mathf.Lerp(smoothedTension, tensionMag, graphSmoothing);
        smoothedCentripetal = Mathf.Lerp(smoothedCentripetal, centripetalMag, graphSmoothing);

        // --- Render Arrows ---
        RenderArrow(gravityArrow, ballPos, gravityForce, gravityColor);
        RenderArrow(tensionArrow, ballPos, tensionForce, tensionColor);
        RenderArrow(netForceArrow, ballPos, netForce, netForceColor);

        // --- UI Text Readout ---
        if (readoutText != null)
        {
            readoutText.text = 
                $"<color=#0000FF>Tension:</color> F_t = {tensionMag:F1} N\n" +
                $"<color=#FF0000>Gravity (Const):</color> F_g = {gravityForce.magnitude:F1} N\n" +
                $"<color=#FFFF00>Centripetal:</color> F_c = {centripetalMag:F1} N\n" +
                $"<color=#FFFFFF>Cruise Control:</color> {(isCruiseControl ? "ON" : "OFF")}";
        }

        // --- Update Graphs (Y-Z Plane, Constrained to 2x2 Box) ---
        UpdateGraph(tensionGraph, smoothedTension);
        UpdateGraph(centripetalGraph, smoothedCentripetal);
    }

    private void RenderArrow(ArrowVisual arrow, Vector3 origin, Vector3 force, Color color)
    {
        if (arrow == null) return;
        Vector3 spawnPoint = origin + (force.normalized * ballRadius);
        arrow.UpdateArrow(spawnPoint, force, forceScale, color);
    }

    private void UpdateGraph(LineRenderer line, float newValue)
    {
        if (line == null) return;

        int count = line.positionCount;
        if (count < maxGraphPoints)
        {
            line.positionCount = count + 1;
            count++;
        }

        Vector3[] positions = new Vector3[count];
        line.GetPositions(positions);

        float zSpacing = 2.0f / Mathf.Max(1, maxGraphPoints - 1);

        for (int i = 0; i < count - 1; i++)
        {
            positions[i] = new Vector3(0f, positions[i + 1].y, i * zSpacing);
        }

        float scaledY = Mathf.Min(newValue * graphScaleY, 2.0f);
        positions[count - 1] = new Vector3(0f, scaledY, (count - 1) * zSpacing);

        line.SetPositions(positions);
    }

    private void OnDestroy()
    {
        if (gravityArrow != null) Destroy(gravityArrow.gameObject);
        if (tensionArrow != null) Destroy(tensionArrow.gameObject);
        if (netForceArrow != null) Destroy(netForceArrow.gameObject);
    }
}