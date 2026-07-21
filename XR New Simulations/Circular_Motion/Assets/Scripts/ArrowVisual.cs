using UnityEngine;

public class ArrowVisual : MonoBehaviour
{
    [Header("Child Object References")]
    [SerializeField] private GameObject shaft;
    [SerializeField] private GameObject head;

    [Header("Arrow Settings")]
    [Tooltip("Maximum allowed total length for the arrow")]
    [SerializeField] private float maxLength = 5.0f;

    [Tooltip("Fixed height offset for the head cone along the shaft")]
    [SerializeField] private float headHeight = 0.4f;
    [SerializeField] private float shaftThickness = 0.1f;

    [Header("Head Size Adjustment")]
    [Tooltip("Overall scale multiplier for the arrowhead mesh")]
    [SerializeField] private Vector3 headScale = new Vector3(0.3f, 0.3f, 0.3f);

    private Renderer[] cachedRenderers;
    private MaterialPropertyBlock propertyBlock;
   
    // Shader property IDs for performance optimization
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private void Awake()
    {
        cachedRenderers = GetComponentsInChildren<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    public void UpdateArrow(Vector3 originPoint, Vector3 forceVector, float scaleFactor, Color color)
    {
        float magnitude = forceVector.magnitude;

        // Hide arrow if force is zero or negligible
        if (magnitude < 0.001f)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        Vector3 direction = forceVector.normalized;

        // Clamp total arrow length so it doesn't exceed maxLength
        float calculatedLength = magnitude * scaleFactor;
        float totalLength = Mathf.Min(calculatedLength, maxLength);

        // 1. Position and Align Arrow Parent directly to origin point
        transform.SetPositionAndRotation(originPoint, Quaternion.FromToRotation(Vector3.up, direction));

        // 2. Calculate Shaft Length (keep head height fixed)
        float shaftLength = Mathf.Max(0.01f, totalLength - headHeight);

        // 3. Update Shaft Scale and Offset
        if (shaft != null)
        {
            // Standard Unity cylinders are 2 units tall natively along Y
            shaft.transform.localScale = new Vector3(shaftThickness, shaftLength * 0.5f, shaftThickness);
            shaft.transform.localPosition = new Vector3(0f, shaftLength * 0.5f, 0f);
        }

        // 4. Update Head (Move to top of shaft & apply head scale)
        if (head != null)
        {
            head.transform.localPosition = new Vector3(0f, shaftLength, 0f);
            head.transform.localScale = headScale;
        }

        // 5. Apply Color efficiently using MaterialPropertyBlock
        ApplyColor(color);
    }

    private void ApplyColor(Color color)
    {
        if (cachedRenderers == null) return;

        foreach (Renderer r in cachedRenderers)
        {
            r.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColorID, color);
            propertyBlock.SetColor(ColorID, color);
            r.SetPropertyBlock(propertyBlock);
        }
    }
}