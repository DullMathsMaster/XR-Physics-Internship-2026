using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPositioner : MonoBehaviour
{
    [Tooltip("Drag the 'PlanetSphere' child object here")]
    public GameObject PlanetSphere;
    
    [Tooltip("Extra padding above the planet surface")]
    public float padding = 0.5f; 

    [Tooltip("How smoothly the label slides (higher = faster snapping)")]
    public float smoothSpeed = 10f;

    private RectTransform rectTransform;
    private MeshRenderer sphereRenderer;
    private Vector3 lastScale;
    private bool hasSetupScale = false;
    
    // Track the active smoothing coroutine safely
    private Coroutine smoothMoveCoroutine;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        // Do absolutely nothing else here to ensure the game engine loads safely
    }

    void LateUpdate()
    {
        if (PlanetSphere == null || rectTransform == null) return;

        // Try to grab the renderer if we don't have it yet
        if (sphereRenderer == null)
        {
            sphereRenderer = PlanetSphere.GetComponent<MeshRenderer>();
            if (sphereRenderer == null) return; // Wait until it's ready
        }

        // Initialize the scale tracker safely on the first frame it becomes available
        if (!hasSetupScale)
        {
            lastScale = PlanetSphere.transform.localScale;
            hasSetupScale = true;
            UpdateLabelPositionInstant();
            return;
        }

        // ONLY update when the scale changes (slider movement)
        if (PlanetSphere.transform.localScale != lastScale)
        {
            // Stop any current movement before starting a new one to prevent conflicts
            if (smoothMoveCoroutine != null) StopCoroutine(smoothMoveCoroutine);
            
            smoothMoveCoroutine = StartCoroutine(SmoothlyMoveLabel());
            lastScale = PlanetSphere.transform.localScale; 
        }
    }

    // Instantly places the label at startup safely
    void UpdateLabelPositionInstant()
    {
        if (sphereRenderer == null || rectTransform == null || transform.parent == null) return;

        float worldRadius = sphereRenderer.bounds.extents.y;
        Vector3 worldTargetPos = PlanetSphere.transform.position + new Vector3(0f, worldRadius + padding, 0f);
        Vector3 localTargetPos = transform.parent.InverseTransformPoint(worldTargetPos);

        Vector3 newPos = rectTransform.anchoredPosition3D;
        newPos.y = localTargetPos.y;
        rectTransform.anchoredPosition3D = newPos;
    }

    // Isolated coroutine that smooths the slide ONLY while the slider is moving
    IEnumerator SmoothlyMoveLabel()
    {
        while (sphereRenderer != null && rectTransform != null && transform.parent != null)
        {
            float worldRadius = sphereRenderer.bounds.extents.y;
            Vector3 worldTargetPos = PlanetSphere.transform.position + new Vector3(0f, worldRadius + padding, 0f);
            Vector3 localTargetPos = transform.parent.InverseTransformPoint(worldTargetPos);

            Vector3 currentPos = rectTransform.anchoredPosition3D;
            
            // Smoothly glide the Y position
            currentPos.y = Mathf.Lerp(currentPos.y, localTargetPos.y, Time.deltaTime * smoothSpeed);
            rectTransform.anchoredPosition3D = currentPos;

            // If we are incredibly close to the target, break out and put the routine to sleep
            if (Mathf.Abs(currentPos.y - localTargetPos.y) < 0.01f)
            {
                currentPos.y = localTargetPos.y;
                rectTransform.anchoredPosition3D = currentPos;
                break;
            }

            yield return null; // Wait for the next frame safely
        }
    }
}