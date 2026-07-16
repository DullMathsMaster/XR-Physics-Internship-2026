using UnityEngine;

public class LensSlider : MonoBehaviour
{
    [HideInInspector] public Transform slideBar;
    [HideInInspector] public bool snappable = false;

    [HideInInspector] public CellIdentity cellA;
    [HideInInspector] public CellIdentity cellB;

    [Header("Quest interaction bridge")]
    public Transform followTarget;
    [HideInInspector] public bool isGrabbed;

    private Transform A;
    private Transform B;

    private Renderer[] renderers;
    private Cell cell;

    private float slideAmount = 0f;
    private bool active;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        cell = GetComponentInChildren<Cell>();

        if (slideBar != null)
        {
            slideBar.parent = null;
        }
    }

    private void Update()
    {
        if (cellA != null && A == null) A = cellA.transform;
        if (cellB != null && B == null) B = cellB.transform;

        if (A != null && B != null)
        {
            Vector3 ab = B.position - A.position;
            float distance = ab.magnitude;

            if (slideBar != null)
            {
                slideBar.localScale = new Vector3(
                    slideBar.localScale.x,
                    slideBar.localScale.y,
                    1f + distance / 0.03f
                );

                slideBar.position = Vector3.Lerp(A.position, B.position, 0.5f);
                slideBar.LookAt(B.position, Vector3.up);
            }

            if (isGrabbed && followTarget != null)
            {
                transform.position = followTarget.position;
                transform.rotation = followTarget.rotation;

                if (distance > Mathf.Epsilon)
                {
                    slideAmount = Mathf.Clamp(
                        Vector3.Project(transform.position - A.position, ab).magnitude / distance,
                        0f,
                        1f
                    );

                    if (snappable)
                    {
                        float snapStep = 1f / (distance / 0.06f);
                        slideAmount = Mathf.Round(slideAmount / snapStep) * snapStep;
                    }

                    if (Vector3.Dot(transform.position - A.position, ab) < 0f)
                    {
                        slideAmount = 0f;
                    }
                }
            }

            transform.position = Vector3.Lerp(A.position, B.position, slideAmount);

            if (distance > Mathf.Epsilon)
            {
                transform.rotation = Quaternion.LookRotation(ab, Vector3.up);
            }
        }

        active = Board.instance != null &&
                 cellA != null &&
                 cellB != null &&
                 Board.instance.reverseGrid.ContainsKey(cellA) &&
                 Board.instance.reverseGrid.ContainsKey(cellB);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = active;
        }

        if (cell != null)
        {
            cell.enabled = active;
        }
    }

    private void OnDestroy()
    {
        if (slideBar != null)
        {
            Destroy(slideBar.gameObject);
        }
    }
}