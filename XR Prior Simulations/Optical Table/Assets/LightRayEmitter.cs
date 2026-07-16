using UnityEngine;
using LightTK;
using static LightTK.LTK;

public class LightRayEmitter : MonoBehaviour
{
    public static LTKCollider[] colliders;

    public LightRay[] rays;
    private LineRenderer[] renderers;

    [SerializeField] public GameObject rayPrefab;

    private Spawner spawner;

    private void Start()
    {
        colliders = FindObjectsByType<LTKCollider>(FindObjectsSortMode.None);

        rays = new LightRay[10];
        renderers = new LineRenderer[rays.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            GameObject rayObject = Instantiate(rayPrefab);
            renderers[i] = rayObject.GetComponent<LineRenderer>();
        }

        if (transform.parent != null)
        {
            spawner = transform.parent.GetComponentInParent<Spawner>();
        }
    }

    private void Update()
    {
        if (spawner != null && spawner.enabled)
            return;

        if (renderers == null || rays == null)
            return;

        for (int j = 0; j < rays.Length; j++)
        {
            LightRay ray = new LightRay();
            rays[j] = ray;

            ray.position = transform.position +
                           Quaternion.AngleAxis(360f * j / rays.Length, transform.forward) *
                           Vector3.up * 0.01f;

            ray.direction = -transform.forward;

            int i = 1;
            renderers[j].positionCount = i;
            renderers[j].SetPosition(0, ray.position);

            bool hitBlock = false;

            for (int z = 0; z < 100 && SimulateRay(ray, colliders, out LightRayHit hit); z++, i++)
            {
                renderers[j].positionCount = i + 1;
                renderers[j].SetPosition(i, ray.position);

                if (hit.surface.settings.type == SurfaceSettings.SurfaceType.Block)
                {
                    hitBlock = true;
                    break;
                }
            }

            if (!hitBlock)
            {
                renderers[j].positionCount = i + 1;
                renderers[j].SetPosition(i, ray.position + ray.direction * 100f);
            }
        }
    }

    private void OnDestroy()
    {
        if (renderers == null)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                Destroy(renderers[i].gameObject);
            }
        }
    }
}