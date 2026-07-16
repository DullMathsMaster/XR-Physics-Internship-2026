using UnityEngine;

public class CellIdentity : MonoBehaviour
{
    [HideInInspector] public Transform parent;
    [HideInInspector] public Cell owner;

    [HideInInspector] public bool destroy = false;
    [HideInInspector] public float destroyTimer = 0f;

    [Header("Quest interaction bridge")]
    public Transform grabFollowTarget;
    [HideInInspector] public bool isGrabbed;
    [HideInInspector] public bool isHovered;

    [HideInInspector] public CellIdentity linkedIdentity;

    public int gridSnapAngle = 30;
    public int sliderSnapAngle = 90;

    private bool setPos = false;
    private bool useBoard = true;

    [HideInInspector] public Vector2Int gridPos;

    private Canvas label;
    private Collider cachedCollider;
    private Quaternion rotOffset = Quaternion.identity;

    private bool isDead = false;

    private void Start()
    {
        label = GetComponentInChildren<Canvas>();
        cachedCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        UpdateLabel();

        if (isGrabbed)
        {
            HandleGrabbed();
        }
        else
        {
            HandleReleasedHoveringNearSlider();
            HandleBoardPlacementOnRelease();
        }

        if (parent == null && destroy)
        {
            if (destroyTimer <= 0f)
            {
                InvokeDestroy();
            }
            else
            {
                destroyTimer -= Time.deltaTime;
            }
        }

        if (!destroy)
        {
            destroyTimer = 2f;
        }
    }

    private void UpdateLabel()
    {
        if (label == null || Camera.main == null)
            return;

        label.enabled = isHovered;

        if (label.enabled)
        {
            label.transform.LookAt(
                label.transform.position + Camera.main.transform.rotation * Vector3.left,
                Camera.main.transform.rotation * Vector3.up
            );

            Vector3 euler = label.transform.eulerAngles;
            label.transform.eulerAngles = new Vector3(0f, euler.y, 0f);
        }
    }

    private void HandleGrabbed()
    {
        if (Board.instance != null)
        {
            bool inGrid = Board.instance.grid.ContainsKey(gridPos) && Board.instance.grid[gridPos] == this;
            bool inReverseGrid = Board.instance.reverseGrid.ContainsKey(this) && Board.instance.reverseGrid[this] == gridPos;

            if (inGrid || inReverseGrid)
            {
                Board.instance.grid.Remove(gridPos);
                Board.instance.reverseGrid.Remove(this);
            }

            if (owner != null && Board.instance.sliderCells.Contains(owner))
            {
                useBoard = true;
                owner.identity = null;
                parent = transform;
                owner = null;
            }
        }

        destroy = false;
        if (linkedIdentity != null) linkedIdentity.destroy = false;
        parent = transform;

        if (grabFollowTarget != null)
        {
            transform.position = grabFollowTarget.position;
            transform.rotation = grabFollowTarget.rotation;
        }

        if (linkedIdentity != null && Board.instance != null)
        {
            if (Board.instance.reverseGrid.ContainsKey(linkedIdentity))
            {
                transform.parent = null;
                linkedIdentity.transform.parent = null;
                transform.parent = linkedIdentity.transform;
            }
        }

        setPos = true;

        if (Board.instance != null)
        {
            Vector3 localPos = Board.instance.transform.InverseTransformPoint(transform.position);
            gridPos = Board.instance.PositionToGrid(localPos);
        }
    }

    private void HandleReleasedHoveringNearSlider()
    {
        if (Board.instance == null || owner != null || linkedIdentity != null)
            return;

        foreach (Cell c in Board.instance.sliderCells)
        {
            if (c == null || !c.enabled || c.col == null)
                continue;

            if ((c.col.bounds.center - transform.position).sqrMagnitude < 0.1f * 0.1f)
            {
                if (c.identity == null)
                {
                    useBoard = false;
                    parent = c.transform;
                    owner = c;
                    owner.identity = this;
                    destroy = false;
                }

                rotOffset = Quaternion.Inverse(transform.rotation) * c.transform.rotation;
            }
        }
    }

    private void HandleBoardPlacementOnRelease()
    {
        if (isGrabbed || !setPos || !useBoard || Board.instance == null)
            return;

        setPos = false;

        if (Board.instance.cells.ContainsKey(gridPos) && !Board.instance.grid.ContainsKey(gridPos))
        {
            Board.instance.grid.Add(gridPos, this);
            Board.instance.reverseGrid.Add(this, gridPos);

            rotOffset = Quaternion.Inverse(transform.rotation) * Board.instance.transform.rotation;
        }
        else
        {
            if (linkedIdentity != null)
            {
                bool isParent = transform.parent == null;

                if (!isParent)
                {
                    transform.parent = null;
                    linkedIdentity.transform.parent = null;
                    transform.parent = linkedIdentity.transform;

                    transform.localPosition = Vector3.up * 0.08f;
                    transform.localRotation = Quaternion.identity;
                }
            }

            if (Bin.instance != null && cachedCollider != null && Bin.instance.colliders.Contains(cachedCollider))
            {
                Bin.instance.colliders.Remove(cachedCollider);
                InvokeDestroy();
            }
        }
    }

    public void CellUpdate(Vector3 localSnapPoint, Cell c)
    {
        transform.position = localSnapPoint;

        int snapAngle = useBoard ? gridSnapAngle : sliderSnapAngle;
        Vector3 snappedEuler = rotOffset.eulerAngles / snapAngle;
        snappedEuler = new Vector3(
            Mathf.RoundToInt(snappedEuler.x),
            Mathf.RoundToInt(snappedEuler.y),
            Mathf.RoundToInt(snappedEuler.z)
        ) * snapAngle;

        rotOffset.eulerAngles = snappedEuler;

        Quaternion multiplier = useBoard ? Board.instance.transform.rotation : c.transform.rotation;
        Quaternion rot = multiplier * Quaternion.Inverse(rotOffset);

        float xRot = useBoard ? 0f : 180f;
        Vector3 euler = rot.eulerAngles;
        rot.eulerAngles = new Vector3(xRot, euler.y - xRot, 0f);

        transform.rotation = rot;
    }

    public void InvokeDestroy()
    {
        DestroyIdentity();
        if (linkedIdentity != null)
        {
            linkedIdentity.DestroyIdentity();
        }
    }

    private void DestroyIdentity()
    {
        if (isDead) return;
        isDead = true;

        if (TryGetComponent(out SliderSpawner spawner))
        {
            if (spawner.lensSlider != null)
            {
                Destroy(spawner.lensSlider.gameObject);
            }
        }

        if (Board.instance != null)
        {
            Board.instance.grid.Remove(gridPos);
            Board.instance.reverseGrid.Remove(this);
        }

        Collider ownCollider = GetComponent<Collider>();
        if (ownCollider != null) ownCollider.enabled = false;

        foreach (Transform t in transform)
        {
            if (!t.TryGetComponent(out Rigidbody rb))
            {
                rb = t.gameObject.AddComponent<Rigidbody>();
            }

            rb.AddForceAtPosition(
                new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)),
                new Vector3(0f, 20f, 0f),
                ForceMode.Impulse
            );

            rb.angularDamping = 0f;
        }

        Destroy(gameObject, 2f);
    }
}