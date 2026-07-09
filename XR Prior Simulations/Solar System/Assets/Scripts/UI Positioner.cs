using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPositioner : MonoBehaviour
{
    [Tooltip("Drag the 'PlanetSphere' child object here")]
    public GameObject PlanetSphere;
    
    [Tooltip("Extra padding above the planet surface")]
    public float padding = 0.5f; 

    private RectTransform rectTransform;
    private MeshRenderer sphereRenderer;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (PlanetSphere != null)
        {
            sphereRenderer = PlanetSphere.GetComponent<MeshRenderer>();
        }
    }

    void LateUpdate()
    {
        if (PlanetSphere == null || rectTransform == null || sphereRenderer == null) return;

        float worldRadius = sphereRenderer.bounds.extents.y;

        Vector3 worldTargetPos = PlanetSphere.transform.position + new Vector3(0f, worldRadius + padding, 0f);
        Vector3 localTargetPos = transform.parent.InverseTransformPoint(worldTargetPos);

        Vector3 newPos = rectTransform.anchoredPosition3D;
        newPos.y = localTargetPos.y;
        rectTransform.anchoredPosition3D = newPos;
    }
}