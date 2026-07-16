using UnityEngine;
using LightTK;

public class LTKCollider : MonoBehaviour
{
    public AbstractSurface surface;
    public SurfaceSettings settings;
    public bool enableCollision = true;

    [System.NonSerialized]
    public Surface _surface;

    private void Update()
    {
        if (surface == null)
            return;

        _surface = surface;
        _surface.settings = settings;
        _surface.position = transform.rotation * surface.position + transform.position;
        _surface.rotation = transform.rotation * surface.rotation;
    }
}