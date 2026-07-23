using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

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
    [Tooltip("Assign the VR 'A' Button Input Action here")]
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
    [SerializeField] private float maxGraphHeight = 2.0f;
    [Range(0.01f, 1f)][SerializeField] private float graphSmoothing = 0.2f;

    [Header("Visualizers & String")]
    [SerializeField] private LineRenderer stringLineRenderer;
    [SerializeField] private LineRenderer planeVisualizer;
    [SerializeField] private LineRenderer axisVisualizer;
    [SerializeField] private int planeSegments = 64;
    [SerializeField] private float axisLength = 2.5f;

    private ArrowVisual gravityArrow;
    private ArrowVisual tensionArrow;
    private ArrowVisual netForceArrow;
    private Rigidbody rb;

    private float targetSpeed = 0f;
    
    // Rotation & Orbit Center parameters (Supports arbitrary tilt/angle)
    private Vector3 circleCenter;
    private Vector3 horizontalRadiusVec;
    private Vector3 rotationAxis = Vector3.up;
    private float degreesPerSecond = 0f;
    private float currentAngle = 0f;
    private float currentRadius = 1.0f;
    private float cruiseMaxScale = 1.0f;

    // Smoothed force values for readout and graphs
    private float smoothedTension = 0f;
    private float smoothedCentripetal = 0f;
    private float smoothedNetForce = 0f;

    // Displacement tracking to preserve velocity during VR grabs
    private Vector3 lastFramePos;
    private Vector3 calculatedVelocity;

    // Graph history
    private List<float> tensionHistory = new List<float>();
    private List<float> centripetalHistory = new List<float>();

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

    private void ForceReleaseVRHand()
    {
        var interactable = GetComponent("XRGrabInteractable");
        if (interactable != null)
        {
            var managerProp = interactable.GetType().GetProperty("interactionManager");
            var manager = managerProp?.GetValue(interactable);
            if (manager != null)
            {
                var method = manager.GetType().GetMethod("SelectExit");
                var interactorsProp = interactable.GetType().GetProperty("interactorsSelecting");
                var interactors = interactorsProp?.GetValue(interactable) as System.Collections.IEnumerable;

                if (interactors != null)
                {
                    foreach (var interactor in interactors)
                    {
                        method?.Invoke(manager, new object[] { interactor, interactable });
                    }
                }
            }
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
            col.enabled = true;
        }
    }

    private void OnCruiseControlToggled(bool state)
    {
        if (pivot == null || rb == null) return;

        if (state)
        {
            ForceReleaseVRHand();

            SetupContinuousTrajectory();

            rb.isKinematic = true;

            float fc = (rb.mass * targetSpeed * targetSpeed) / Mathf.Max(0.0001f, currentRadius);
            float fg = Physics.gravity.magnitude * rb.mass;
            cruiseMaxScale = Mathf.Max(fc + fg, 1.0f);
            
            RebuildStaticVisualizers();
        }
        else
        {
            rb.isKinematic = false;
            RebuildStaticVisualizers();
        }
    }

    private void SetupContinuousTrajectory()
    {
        Vector3 ballPos = transform.position;
        Vector3 stringVec = pivot.position - ballPos;
        float dist = stringVec.magnitude;
        if (dist <= 0.0001f) return;

        Vector3 stringDir = stringVec / dist;

        // 1. Recover active velocity (handling VR hand drop)
        Vector3 activeVel = rb.linearVelocity.sqrMagnitude > 0.01f ? rb.linearVelocity : calculatedVelocity;
        Vector3 tangentVel = activeVel - Vector3.Project(activeVel, stringDir);

        if (tangentVel.magnitude < 0.1f)
        {
            tangentVel = Vector3.Cross(stringDir, Vector3.up).normalized * 2.0f;
            if (tangentVel.sqrMagnitude < 0.01f)
                tangentVel = Vector3.Cross(stringDir, Vector3.right).normalized * 2.0f;
        }

        targetSpeed = Mathf.Max(tangentVel.magnitude, 1.0f);

        // 2. Calculate true physics Net Force (Tension + Gravity)
        Vector3 gravityForce = Physics.gravity * rb.mass;
        float centripetalMag = (rb.mass * targetSpeed * targetSpeed) / Mathf.Max(0.0001f, dist);
        float gravRadialComp = -Vector3.Dot(gravityForce, stringDir);
        float tensionMag = Mathf.Max(0f, gravRadialComp + centripetalMag);
        
        Vector3 tensionForce = stringDir * tensionMag;
        Vector3 netForce = tensionForce + gravityForce; // Pure centripetal force vector

        // 3. If net force is zero (e.g. weightless/freefall), fall back to perpendicular velocity plane
        if (netForce.sqrMagnitude < 0.001f)
        {
            rotationAxis = Vector3.Cross(stringVec, tangentVel).normalized;
            circleCenter = pivot.position;
        }
        else
        {
            // Net force vector points DIRECTLY toward the true circle center
            Vector3 centripetalDir = netForce.normalized;

            // Rotation axis is perpendicular to velocity and net centripetal force
            rotationAxis = Vector3.Cross(tangentVel.normalized, centripetalDir).normalized;

            // Radius derived from speed and centripetal acceleration magnitude
            currentRadius = (rb.mass * targetSpeed * targetSpeed) / netForce.magnitude;

            // True center of motion in 3D space
            circleCenter = ballPos + (centripetalDir * currentRadius);
        }

        // 4. Radius vector pointing from center to ball
        horizontalRadiusVec = ballPos - circleCenter;

        // 5. Angular speed (deg/sec)
        float radiansPerSecond = targetSpeed / Mathf.Max(0.001f, currentRadius);
        degreesPerSecond = radiansPerSecond * Mathf.Rad2Deg;

        currentAngle = 0f;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastFramePos = transform.position;

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
            Debug.LogError("[ForceVisualiser] Assign Arrow Prefab in Inspector!", this);
        }
    }

    private void FixedUpdate()
    {
        if (pivot == null || rb == null) return;

        if (!isCruiseControl)
        {
            ApplyStandardPendulumConstraints();
        }
    }

    private void ApplyStandardPendulumConstraints()
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

    private void Update()
    {
        if (Time.deltaTime > 0f)
        {
            calculatedVelocity = (transform.position - lastFramePos) / Time.deltaTime;
            lastFramePos = transform.position;
        }

        if (Input.GetKeyDown(toggleCruiseControlKey))
        {
            IsCruiseControl = !IsCruiseControl;
        }
    }

    private void LateUpdate()
    {
        if (pivot == null || rb == null || arrowPrefab == null) return;

        if (isCruiseControl)
        {
            currentAngle += degreesPerSecond * Time.deltaTime;

            Vector3 rotatedRadiusVec = Quaternion.AngleAxis(currentAngle, rotationAxis) * horizontalRadiusVec;
            Vector3 posOnCircle = circleCenter + rotatedRadiusVec;

            transform.position = posOnCircle;
            rb.position = posOnCircle;
        }

        if (stringLineRenderer != null)
        {
            stringLineRenderer.enabled = true;
            stringLineRenderer.positionCount = 2;
            stringLineRenderer.SetPosition(0, pivot.position);
            stringLineRenderer.SetPosition(1, transform.position);
        }

        Vector3 ballPos = transform.position;
        Vector3 stringVec = pivot.position - ballPos;
        float dist = stringVec.magnitude;
        if (dist <= 0.0001f) return;

        Vector3 stringDir = stringVec / dist;
        Vector3 gravityForce = Physics.gravity * rb.mass;

        float speed = isCruiseControl ? targetSpeed : Vector3.ProjectOnPlane(rb.linearVelocity, stringDir).magnitude;

        float centripetalMag = (rb.mass * speed * speed) / Mathf.Max(0.0001f, currentRadius);
        float gravRadialComp = -Vector3.Dot(gravityForce, stringDir);

        float tensionMag = Mathf.Max(0f, gravRadialComp + centripetalMag);
        Vector3 tensionForce = stringDir * tensionMag;

        // FIXED: Point centripetal force arrow toward orbit center during Cruise Control
        Vector3 centerDir = (circleCenter - ballPos).normalized;
        Vector3 netForce = isCruiseControl ? (centerDir * centripetalMag) : (tensionForce + gravityForce);

        smoothedTension = Mathf.Lerp(smoothedTension, tensionMag, graphSmoothing);
        smoothedCentripetal = Mathf.Lerp(smoothedCentripetal, centripetalMag, graphSmoothing);
        smoothedNetForce = Mathf.Lerp(smoothedNetForce, netForce.magnitude, graphSmoothing);

        Vector3 displayTensionForce = stringDir * smoothedTension;
        Vector3 displayNetForce = isCruiseControl 
            ? centerDir * smoothedCentripetal 
            : (netForce.sqrMagnitude > 0.0001f ? netForce.normalized * smoothedNetForce : Vector3.zero);

        RenderArrow(gravityArrow, ballPos, gravityForce, gravityColor);
        RenderArrow(tensionArrow, ballPos, displayTensionForce, tensionColor);
        RenderArrow(netForceArrow, ballPos, displayNetForce, netForceColor);

        if (readoutText != null)
        {
            readoutText.text = 
                $"<color=#0000FF>Tension:</color> F_t = {smoothedTension:F1} N\n" +
                $"<color=#FF0000>Gravity:</color> F_g = {gravityForce.magnitude:F1} N\n" +
                $"<color=#FFFF00>Centripetal:</color> F_c = {smoothedCentripetal:F1} N\n" +
                $"<color=#FFFFFF>Cruise Control:</color> {(isCruiseControl ? $"ON ({targetSpeed:F1} m/s)" : "OFF")}";
        }

        float globalMax = isCruiseControl ? cruiseMaxScale : CalculateGlobalMax(smoothedTension, smoothedCentripetal);
        UpdateGraph(tensionGraph, tensionHistory, smoothedTension, globalMax);
        UpdateGraph(centripetalGraph, centripetalHistory, smoothedCentripetal, globalMax);
    }

    private void RebuildStaticVisualizers()
    {
        if (!isCruiseControl || pivot == null)
        {
            if (planeVisualizer != null) planeVisualizer.enabled = false;
            if (axisVisualizer != null) axisVisualizer.enabled = false;
            return;
        }

        if (axisVisualizer != null)
        {
            axisVisualizer.enabled = true;
            axisVisualizer.positionCount = 2;
            axisVisualizer.SetPosition(0, circleCenter - rotationAxis * axisLength);
            axisVisualizer.SetPosition(1, circleCenter + rotationAxis * axisLength);
        }

        if (planeVisualizer != null)
        {
            planeVisualizer.enabled = true;
            planeVisualizer.positionCount = planeSegments + 1;

            Vector3[] points = new Vector3[planeSegments + 1];
            for (int i = 0; i <= planeSegments; i++)
            {
                float angle = (i / (float)planeSegments) * 360f;
                Vector3 ringPoint = circleCenter + (Quaternion.AngleAxis(angle, rotationAxis) * horizontalRadiusVec);
                points[i] = ringPoint;
            }
            planeVisualizer.SetPositions(points);
        }
    }

    private void RenderArrow(ArrowVisual arrow, Vector3 origin, Vector3 force, Color color)
    {
        if (arrow == null) return;
        if (force.sqrMagnitude < 0.0001f)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        arrow.gameObject.SetActive(true);
        Vector3 spawnPoint = origin + (force.normalized * ballRadius);
        arrow.UpdateArrow(spawnPoint, force, forceScale, color);
    }

    private float CalculateGlobalMax(float currentTension, float currentCentripetal)
    {
        float maxVal = Mathf.Max(currentTension, currentCentripetal, 1.0f);
        foreach (float v in tensionHistory) if (v > maxVal) maxVal = v;
        foreach (float v in centripetalHistory) if (v > maxVal) maxVal = v;
        return maxVal;
    }

    private void UpdateGraph(LineRenderer line, List<float> history, float newValue, float currentMax)
    {
        if (line == null) return;

        if (history.Count >= maxGraphPoints)
        {
            history.RemoveAt(0);
        }
        history.Add(newValue);

        line.positionCount = history.Count;
        Vector3[] positions = new Vector3[history.Count];
        float zSpacing = 2.0f / Mathf.Max(1, maxGraphPoints - 1);

        float scaleMultiplier = currentMax > 0f ? (maxGraphHeight / currentMax) : 0f;

        for (int i = 0; i < history.Count; i++)
        {
            float scaledY = history[i] * scaleMultiplier;
            positions[i] = new Vector3(0f, scaledY, i * zSpacing);
        }

        line.SetPositions(positions);
    }
}