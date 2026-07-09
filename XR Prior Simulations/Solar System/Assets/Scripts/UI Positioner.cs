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
    
    private Coroutine smoothMoveCoroutine;

    void Start()
    {
        // Having other code in here was causing it to crash (interfering with the startup?)
        rectTransform = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (PlanetSphere == null || rectTransform == null) return;

        if (sphereRenderer == null)
        {
            sphereRenderer = PlanetSphere.GetComponent<MeshRenderer>();
            if (sphereRenderer == null) return; // Wait until it's ready
        }

        if (!hasSetupScale)
        {
            lastScale = PlanetSphere.transform.localScale;
            hasSetupScale = true;
            UpdateLabelPositionInstant();
            return;
        }

        // Only update when the scale changes (slider movement)
        if (PlanetSphere.transform.localScale != lastScale)
        {
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

    // Isolated routine that smooths the slide only while the slider is moving
    IEnumerator SmoothlyMoveLabel()
    {
        while (sphereRenderer != null && rectTransform != null && transform.parent != null)
        {
            float worldRadius = sphereRenderer.bounds.extents.y;
            Vector3 worldTargetPos = PlanetSphere.transform.position + new Vector3(0f, worldRadius + padding, 0f);
            Vector3 localTargetPos = transform.parent.InverseTransformPoint(worldTargetPos);

            Vector3 currentPos = rectTransform.anchoredPosition3D;
            
            currentPos.y = Mathf.Lerp(currentPos.y, localTargetPos.y, Time.deltaTime * smoothSpeed);
            rectTransform.anchoredPosition3D = currentPos;

            if (Mathf.Abs(currentPos.y - localTargetPos.y) < 0.01f)
            {
                currentPos.y = localTargetPos.y;
                rectTransform.anchoredPosition3D = currentPos;
                break;
            }

            yield return null;
        }
    }
}