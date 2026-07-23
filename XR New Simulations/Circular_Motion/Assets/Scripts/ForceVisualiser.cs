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
    
    // Immutable planar basis for Cruise Control
    private Vector3 planeNormal = Vector3.right;
    private Vector3 planeU = Vector3.forward;
    private Vector3 planeV = Vector3.up;
    private float angularVelocity = 0f;
    private float cruiseStartTime = 0f;
    private float cruiseMaxScale = 1.0f;

    // Smoothed force values
    private float smoothedTension = 0f;
    private float smoothedCentripetal = 0f;
    private float smoothedNetForce = 0f;

    // History for dynamic graph scaling
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
        // Force drop across standard VR interactables via reflection to avoid dependency issues
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

        // Toggle components as fallback
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

            Vector3 relPos = transform.position - pivot.position;
            float dist = relPos.magnitude;
            if (dist <= 0.0001f) return;

            Vector3 currentVel = rb.linearVelocity;
            
            if (currentVel.magnitude < 0.2f)
            {
                currentVel = Vector3.Cross(relPos, Vector3.up).normalized * 2.0f;
                if (currentVel.sqrMagnitude < 0.01f)
                {
                    currentVel = Vector3.Cross(relPos, Vector3.right).normalized * 2.0f;
                }
            }

            targetSpeed = Mathf.Max(currentVel.magnitude, 1.5f);
            angularVelocity = targetSpeed / stringLength;

            Vector3 rawNormal = Vector3.Cross(relPos, currentVel);
            if (rawNormal.sqrMagnitude < 0.001f)
            {
                rawNormal = Vector3.Cross(relPos, Vector3.forward);
                if (rawNormal.sqrMagnitude < 0.001f) rawNormal = Vector3.Cross(relPos, Vector3.right);
            }
            planeNormal = rawNormal.normalized;

            planeU = Vector3.ProjectOnPlane(relPos, planeNormal).normalized;
            planeV = Vector3.Cross(planeNormal, planeU).normalized;

            float dirSign = Vector3.Dot(currentVel, planeV) >= 0f ? 1f : -1f;
            angularVelocity *= dirSign;

            cruiseStartTime = Time.time;
            rb.isKinematic = true;

            float fc = (rb.mass * targetSpeed * targetSpeed) / stringLength;
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
        if (Input.GetKeyDown(toggleCruiseControlKey))
        {
            IsCruiseControl = !IsCruiseControl;
        }
    }

    private void LateUpdate()
    {
        if (pivot == null || rb == null || arrowPrefab == null) return;

        // Position lock executed in LateUpdate overrides VR hand pose updates
        if (isCruiseControl)
        {
            float elapsedTime = Time.time - cruiseStartTime;
            float angle = angularVelocity * elapsedTime;

            Vector3 posOnCircle = pivot.position + (planeU * Mathf.Cos(angle) + planeV * Mathf.Sin(angle)) * stringLength;
            transform.position = posOnCircle;
            rb.position = posOnCircle;
        }

        // Draw connecting string cleanly without 1-frame offset
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

        // Centripetal force F_c = (m * v^2) / r
        float centripetalMag = (rb.mass * speed * speed) / stringLength;
        float gravRadialComp = -Vector3.Dot(gravityForce, stringDir);

        float tensionMag = Mathf.Max(0f, gravRadialComp + centripetalMag);
        Vector3 tensionForce = stringDir * tensionMag;

        Vector3 netForce = isCruiseControl ? (stringDir * centripetalMag) : (tensionForce + gravityForce);

        smoothedTension = Mathf.Lerp(smoothedTension, tensionMag, graphSmoothing);
        smoothedCentripetal = Mathf.Lerp(smoothedCentripetal, centripetalMag, graphSmoothing);
        smoothedNetForce = Mathf.Lerp(smoothedNetForce, netForce.magnitude, graphSmoothing);

        Vector3 displayTensionForce = stringDir * smoothedTension;
        Vector3 displayNetForce = netForce.sqrMagnitude > 0.0001f ? netForce.normalized * smoothedNetForce : Vector3.zero;

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
            axisVisualizer.SetPosition(0, pivot.position - planeNormal * axisLength);
            axisVisualizer.SetPosition(1, pivot.position + planeNormal * axisLength);
        }

        if (planeVisualizer != null)
        {
            planeVisualizer.enabled = true;
            planeVisualizer.positionCount = planeSegments + 1;

            Vector3[] points = new Vector3[planeSegments + 1];
            for (int i = 0; i <= planeSegments; i++)
            {
                float angle = (i / (float)planeSegments) * Mathf.PI * 2f;
                Vector3 offset = (planeU * Mathf.Cos(angle) + planeV * Mathf.Sin(angle)) * stringLength;
                points[i] = pivot.position + offset;
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