using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class ForceVisualiser : MonoBehaviour
{
    [Header("Pendulum Setup")]
    [SerializeField] private Transform pivot;
    [SerializeField] private float stringLength = 5.0f;
    [SerializeField] private float ballRadius = 0.5f;

    [Header("Arrow Setup")]
    [SerializeField] private ArrowVisual arrowPrefab;
    [SerializeField] private float forceScale = 0.05f;

    [Header("Colors")]
    [SerializeField] private Color gravityColor = Color.red;
    [SerializeField] private Color tensionColor = Color.blue;
    [SerializeField] private Color netForceColor = Color.yellow;

    private ArrowVisual gravityArrow;
    private ArrowVisual tensionArrow;
    private ArrowVisual netForceArrow;
    private Rigidbody rb;

    // Cached force values for rendering frame-rate independently
    private Vector3 currentGravityForce;
    private Vector3 currentTensionForce;
    private Vector3 currentNetForce;

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
        Vector3 ballPos = transform.position;

        // --- Calculate Physics Forces ---
        Vector3 stringVec = pivot.position - ballPos;
        float dist = stringVec.magnitude;
        if (dist <= 0.0001f) return;
        Vector3 stringDir = stringVec / dist;
        currentGravityForce = Physics.gravity * rb.mass;
        currentTensionForce = Vector3.zero;

        if (dist >= stringLength)
        {
            Vector3 velocity = rb.linearVelocity; // Use rb.velocity on Unity 2022 or older
            Vector3 radialVel = Vector3.Project(velocity, stringDir);
            Vector3 tangentialVel = velocity - radialVel;

            float centripetal = (rb.mass * tangentialVel.sqrMagnitude) / dist;
            float gravComp = -Vector3.Dot(currentGravityForce, stringDir);
            float tensionMag = Mathf.Max(0f, gravComp + centripetal);
            currentTensionForce = stringDir * tensionMag;
        }
        currentNetForce = currentTensionForce + currentGravityForce;
    }

    private void Update()
    {
        if (arrowPrefab == null) return;

        // --- Render Arrows Every Frame at Current Ball Position ---
        Vector3 ballPos = transform.position;

        RenderArrow(gravityArrow, ballPos, currentGravityForce, gravityColor);
        RenderArrow(tensionArrow, ballPos, currentTensionForce, tensionColor);
        RenderArrow(netForceArrow, ballPos, currentNetForce, netForceColor);
    }



    private void RenderArrow(ArrowVisual arrow, Vector3 origin, Vector3 force, Color color)
    {
        if (arrow == null) return;
        Vector3 spawnPoint = origin + (force.normalized * ballRadius);
        arrow.UpdateArrow(spawnPoint, force, forceScale, color);
    }

    private void OnDestroy()
    {
        if (gravityArrow != null) Destroy(gravityArrow.gameObject);
        if (tensionArrow != null) Destroy(tensionArrow.gameObject);
        if (netForceArrow != null) Destroy(netForceArrow.gameObject);

    }
}

