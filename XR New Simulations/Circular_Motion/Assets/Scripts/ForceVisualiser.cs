using UnityEngine;
using UnityEngine.InputSystem;
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

    [Tooltip("Assign the VR 'A' Button Input Action here (e.g., RightHand/PrimaryButton)")]
    [SerializeField] private InputActionProperty toggleCruiseAction;

    public bool IsCruiseControl
    {
        get => isCruiseControl;
        set
        {
            if (isCruiseControl != value)
            {
                isCruiseControl = value;
                OnCruiseControlToggled(isCruiseControl);
            }
        }
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
    [Range(0.01f, 1f)][SerializeField] private float graphSmoothing = 0.15f;

    private ArrowVisual gravityArrow;
    private ArrowVisual tensionArrow;
    private ArrowVisual netForceArrow;
    private Rigidbody rb;

    private float targetSpeed = 0f;
    private Vector3 rotationAxis = Vector3.up;
    private Vector3 orbitCenter;
    private float orbitRadius;
    private float orbitDirectionSign = 1f;

    private float smoothedTension = 0f;
    private float smoothedCentripetal = 0f;

    private void OnEnable()
    {
        toggleCruiseAction.action?.Enable();
        if (toggleCruiseAction.action != null)
        {
            toggleCruiseAction.action.performed += OnToggleCruiseControlPerformed;
        }
    }

    private void OnDisable()
    {
        if (toggleCruiseAction.action != null)
        {
            toggleCruiseAction.action.performed -= OnToggleCruiseControlPerformed;
        }
        toggleCruiseAction.action?.Disable();
    }

    private void OnToggleCruiseControlPerformed(InputAction.CallbackContext context)
    {
        IsCruiseControl = !IsCruiseControl;
    }

    private void OnCruiseControlToggled(bool state)
    {
        if (state && rb != null && pivot != null)
        {
            Vector3 stringVec = transform.position - pivot.position;
            float currentDist = stringVec.magnitude;
            if (currentDist <= 0.0001f) return;

            Vector3 currentVel = rb.linearVelocity;
            targetSpeed = currentVel.magnitude;

            if (targetSpeed < 0.5f) targetSpeed = 2.0f;

            Vector3 calculatedAxis = Vector3.Cross(stringVec, currentVel).normalized;

            if (calculatedAxis.sqrMagnitude < 0.01f)
            {
                calculatedAxis = Vector3.up;
            }

            rotationAxis = calculatedAxis;

            // Snap position precisely to full string distance
            transform.position = pivot.position + (stringVec.normalized * stringLength);

            // Calculate exact fixed orbit plane parameters
            orbitCenter = pivot.position + Vector3.Project(transform.position - pivot.position, rotationAxis);
            orbitRadius = Vector3.Distance(transform.position, orbitCenter);

            // Determine rotational handedness (+1 or -1) relative to rotationAxis
            Vector3 radiusVec = (transform.position - orbitCenter).normalized;
            Vector3 expectedTangent = Vector3.Cross(rotationAxis, radiusVec).normalized;
            orbitDirectionSign = Vector3.Dot(currentVel, expectedTangent) >= 0f ? 1f : -1f;
        }
    }

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

        if (isCruiseControl)
        {
            MaintainPerfectCircularMotion();
        }
        else
        {
            Vector3 stringVec = transform.position - pivot.position;
            float currentDist = stringVec.magnitude;

            if (currentDist > stringLength)
            {
                Vector3 outboundDir = stringVec / currentDist;
                transform.position = pivot.position + (outboundDir * stringLength);

                float outboundSpeed = Vector3.Dot(rb.linearVelocity, outboundDir);
                if (outboundSpeed > 0f)
                {
                    rb.linearVelocity -= outboundDir * outboundSpeed;
                }
            }
        }
    }

    private void MaintainPerfectCircularMotion()
    {
        if (orbitRadius <= 0.0001f) return;

        Vector3 currentOffset = transform.position - orbitCenter;
        Vector3 radiusVector = Vector3.ProjectOnPlane(currentOffset, rotationAxis).normalized * orbitRadius;
        transform.position = orbitCenter + radiusVector;
        Vector3 tangent = Vector3.Cross(rotationAxis, radiusVector).normalized * orbitDirectionSign;

        rb.linearVelocity = tangent * targetSpeed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleCruiseControlKey))
        {
            IsCruiseControl = !IsCruiseControl;
        }

        if (pivot == null || rb == null || arrowPrefab == null) return;

        Vector3 ballPos = transform.position;
        Vector3 stringVec = pivot.position - ballPos;
        float dist = stringVec.magnitude;
        if (dist <= 0.0001f) return;

        Vector3 stringDir = stringVec / dist;
        Vector3 gravityForce = Physics.gravity * rb.mass;

        float centripetalMag;

        if (isCruiseControl && orbitRadius > 0.0001f)
        {
            centripetalMag = (rb.mass * targetSpeed * targetSpeed) / orbitRadius;
        }
        else
        {
            Vector3 velocity = rb.linearVelocity;
            Vector3 radialVel = Vector3.Project(velocity, stringDir);
            Vector3 tangentialVel = velocity - radialVel;
            centripetalMag = (rb.mass * tangentialVel.sqrMagnitude) / stringLength;
        }

        float gravComp = -Vector3.Dot(gravityForce, stringDir);
        float tensionMag = Mathf.Max(0f, gravComp + centripetalMag);
        Vector3 tensionForce = stringDir * tensionMag;
        Vector3 netForce = tensionForce + gravityForce;

        smoothedTension = Mathf.Lerp(smoothedTension, tensionMag, graphSmoothing);
        smoothedCentripetal = Mathf.Lerp(smoothedCentripetal, centripetalMag, graphSmoothing);

        RenderArrow(gravityArrow, ballPos, gravityForce, gravityColor);
        RenderArrow(tensionArrow, ballPos, tensionForce, tensionColor);
        RenderArrow(netForceArrow, ballPos, netForce, netForceColor);

        if (readoutText != null)
        {
            readoutText.text = 
                $"<color=#0000FF>Tension:</color> F_t = {tensionMag:F1} N\n" +
                $"<color=#FF0000>Gravity (Const):</color> F_g = {gravityForce.magnitude:F1} N\n" +
                $"<color=#FFFF00>Centripetal:</color> F_c = {centripetalMag:F1} N\n" +
                $"<color=#FFFFFF>Cruise Control:</color> {(isCruiseControl ? $"ON ({targetSpeed:F1} m/s)" : "OFF")}";
        }

        UpdateGraph(tensionGraph, smoothedTension);
        UpdateGraph(centripetalGraph, smoothedCentripetal);
    }

    private void RenderArrow(ArrowVisual arrow, Vector3 origin, Vector3 force, Color color)
    {
        if (arrow == null) return;
        if (force.sqrMagnitude < 0.0001f)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

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
}