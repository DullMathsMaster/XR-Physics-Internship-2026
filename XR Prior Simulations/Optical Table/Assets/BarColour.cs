using UnityEngine;

public class BarColour : MonoBehaviour
{
    [HideInInspector] public bool isHovered;
    public MeshRenderer meshRenderer;

    private void Start()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    private void Update()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = isHovered ? Color.red : Color.white;
        }
    }
}