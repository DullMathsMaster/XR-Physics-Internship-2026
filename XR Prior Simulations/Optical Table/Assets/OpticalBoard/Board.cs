using System.Collections.Generic;
using UnityEngine;

// Meta Quest version of Board.cs
public class Board : MonoBehaviour
{
    [Header("Prefabs / hierarchy")]
    public GameObject cell;
    public GameObject pool;
    public GameObject container;

    [Header("Board + resize handles")]
    public Transform boardFollowTarget;
    public Transform leftHandle;
    public Transform rightHandle;
    public Transform topHandle;
    public Transform bottomHandle;

    [Header("Optional visuals")]
    public Renderer leftRenderer;
    public Renderer rightRenderer;
    public Renderer topRenderer;
    public Renderer bottomRenderer;

    [Header("Collider")]
    public BoxCollider box;

    [HideInInspector] public bool boardGrabbed;
    [HideInInspector] public bool leftGrabbed;
    [HideInInspector] public bool rightGrabbed;
    [HideInInspector] public bool topGrabbed;
    [HideInInspector] public bool bottomGrabbed;

    private Vector3 topBottomOffset;
    private Vector3 rightLeftOffset;

    private readonly List<Cell> activeCells = new List<Cell>();
    private readonly Stack<Cell> objectPool = new Stack<Cell>();

    public Dictionary<Vector2Int, CellIdentity> grid = new Dictionary<Vector2Int, CellIdentity>();
    public Dictionary<CellIdentity, Vector2Int> reverseGrid = new Dictionary<CellIdentity, Vector2Int>();
    public Dictionary<Vector2Int, Cell> cells = new Dictionary<Vector2Int, Cell>();
    public HashSet<Cell> sliderCells = new HashSet<Cell>();

    private Vector3 topLeft = new Vector3(-1, 0, 1);
    private Vector3 bottomRight = new Vector3(1, 0, -1);

    private const float halfCellSize = 0.03f; // 0.06 / 2

    public static Board instance;

    private float grabTimer = 0f;

    private void Awake()
    {
        if (!instance) instance = this;
    }

    private void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
        UpdateBoardGrab();
        UpdateResizeHandles();

        topLeft = new Vector3(leftHandle.localPosition.x / halfCellSize + 1f, 0f, topHandle.localPosition.z / halfCellSize - 1f);
        bottomRight = new Vector3(rightHandle.localPosition.x / halfCellSize - 1f, 0f, bottomHandle.localPosition.z / halfCellSize + 1f);

        GenerateGrid();
    }

    private void UpdateBoardGrab()
    {
        if (boardFollowTarget == null)
            return;

        if (boardGrabbed)
        {
            SetHandleColor(Color.Lerp(Color.blue, Color.white, grabTimer));

            if (grabTimer <= 0f)
            {
                transform.position = boardFollowTarget.position;

                Quaternion rot = transform.rotation;
                Vector3 euler = rot.eulerAngles;
                euler.y = boardFollowTarget.rotation.eulerAngles.y;
                rot.eulerAngles = euler;
                transform.rotation = rot;
            }
            else
            {
                grabTimer -= Time.deltaTime;
            }
        }
        else
        {
            grabTimer = 1f;
            SetHandleColor(Color.white);
        }
    }

    private void SetHandleColor(Color color)
    {
        if (leftRenderer != null) leftRenderer.material.color = color;
        if (rightRenderer != null) rightRenderer.material.color = color;
        if (topRenderer != null) topRenderer.material.color = color;
        if (bottomRenderer != null) bottomRenderer.material.color = color;
    }

    private void UpdateResizeHandles()
    {
        float globalOffset = 4f * halfCellSize;

        float topBound = halfCellSize * (topLeft.z + 1f);
        float bottomBound = halfCellSize * (bottomRight.z - 1f);

        if (topGrabbed)
        {
            topHandle.localPosition =
                Vector3.Project(transform.InverseTransformPoint(topHandle.position), Vector3.forward) + topBottomOffset;

            if (topHandle.localPosition.z < bottomBound + globalOffset)
            {
                topHandle.localPosition = new Vector3(0f, 0f, bottomBound + globalOffset) + topBottomOffset;
            }
        }
        else
        {
            topHandle.localPosition = new Vector3(0f, 0f, halfCellSize * Mathf.Round(topLeft.z + 0.5f)) + topBottomOffset;
        }

        if (bottomGrabbed)
        {
            bottomHandle.localPosition =
                Vector3.Project(transform.InverseTransformPoint(bottomHandle.position), Vector3.forward) + topBottomOffset;

            if (bottomHandle.localPosition.z > topBound - globalOffset)
            {
                bottomHandle.localPosition = new Vector3(0f, 0f, topBound - globalOffset) + topBottomOffset;
            }
        }
        else
        {
            bottomHandle.localPosition = new Vector3(0f, 0f, halfCellSize * Mathf.Round(bottomRight.z - 0.5f)) + topBottomOffset;
        }

        float leftBound = halfCellSize * (topLeft.x - 1f);
        float rightBound = halfCellSize * (bottomRight.x + 1f);

        if (leftGrabbed)
        {
            leftHandle.localPosition =
                Vector3.Project(transform.InverseTransformPoint(leftHandle.position), Vector3.right) + rightLeftOffset;

            if (leftHandle.localPosition.x > rightBound - globalOffset)
            {
                leftHandle.localPosition = new Vector3(rightBound - globalOffset, 0f, 0f) + rightLeftOffset;
            }
        }
        else
        {
            leftHandle.localPosition = new Vector3(halfCellSize * Mathf.Round(topLeft.x - 0.5f), 0f, 0f) + rightLeftOffset;
        }

        if (rightGrabbed)
        {
            rightHandle.localPosition =
                Vector3.Project(transform.InverseTransformPoint(rightHandle.position), Vector3.right) + rightLeftOffset;

            if (rightHandle.localPosition.x < leftBound + globalOffset)
            {
                rightHandle.localPosition = new Vector3(leftBound + globalOffset, 0f, 0f) + rightLeftOffset;
            }
        }
        else
        {
            rightHandle.localPosition = new Vector3(halfCellSize * Mathf.Round(bottomRight.x + 0.5f), 0f, 0f) + rightLeftOffset;
        }
    }

    private float width => Mathf.Abs(bottomRight.x - topLeft.x);
    private float height => Mathf.Abs(topLeft.z - bottomRight.z);

    public Vector2Int PositionToGrid(Vector3 pos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(pos.x / halfCellSize / 2f),
            Mathf.RoundToInt(pos.z / halfCellSize / 2f)
        );
    }

    public void GenerateGrid()
    {
        if (box != null)
        {
            box.center = (topLeft + bottomRight) * halfCellSize / 2f;
            box.size = new Vector3((width - 1f) * halfCellSize, 0.01f, (height - 1f) * halfCellSize);
        }

        topBottomOffset = new Vector3((topLeft.x + bottomRight.x) / 2f * halfCellSize, 0f, 0f);
        rightLeftOffset = new Vector3(0f, 0f, (topLeft.z + bottomRight.z) / 2f * halfCellSize);

        if (topHandle != null) topHandle.localScale = new Vector3(0.015f, halfCellSize * (width / 2f), 0.015f);
        if (bottomHandle != null) bottomHandle.localScale = new Vector3(0.015f, halfCellSize * (width / 2f), 0.015f);
        if (leftHandle != null) leftHandle.localScale = new Vector3(0.015f, halfCellSize * (height / 2f), 0.015f);
        if (rightHandle != null) rightHandle.localScale = new Vector3(0.015f, halfCellSize * (height / 2f), 0.015f);

        int w = Mathf.Clamp(Mathf.FloorToInt(width / 2f), 1, int.MaxValue);
        int h = Mathf.Clamp(Mathf.FloorToInt(height / 2f), 1, int.MaxValue);
        int volume = w * h;

        while (activeCells.Count < volume)
        {
            if (objectPool.Count > 0)
            {
                Cell c = objectPool.Pop();
                c.gameObject.SetActive(true);
                activeCells.Add(c);
            }
            else
            {
                GameObject g = Instantiate(cell, container.transform);
                Cell c = g.GetComponent<Cell>();
                c.onBoard = true;
                activeCells.Add(c);
            }
        }

        while (activeCells.Count > volume)
        {
            int last = activeCells.Count - 1;
            activeCells[last].gameObject.SetActive(false);

            if (activeCells[last].identity != null)
            {
                activeCells[last].identity.parent = null;
                activeCells[last].identity.owner = null;
                activeCells[last].identity.destroy = true;
                activeCells[last].identity = null;
            }

            objectPool.Push(activeCells[last]);
            activeCells.RemoveAt(last);
        }

        for (int i = 0; i < activeCells.Count; i++)
        {
            if (activeCells[i].identity != null)
            {
                activeCells[i].identity.parent = null;
                activeCells[i].identity.owner = null;
                activeCells[i].identity.destroy = true;
                activeCells[i].identity = null;
            }
        }

        cells.Clear();

        for (int i = 0, y = Mathf.CeilToInt((bottomRight.z + 1f) / 2f); i < h; i++, y++)
        {
            for (int j = 0, x = Mathf.CeilToInt((topLeft.x + 1f) / 2f); j < w; j++, x++)
            {
                int idx = i * w + j;
                Vector2Int pos = new Vector2Int(x, y);

                cells[pos] = activeCells[idx];

                if (grid.ContainsKey(pos) && grid[pos] != null)
                {
                    activeCells[idx].identity = grid[pos];
                    activeCells[idx].identity.parent = activeCells[idx].transform;
                    activeCells[idx].identity.owner = activeCells[idx];
                    activeCells[idx].identity.destroy = false;
                }

                activeCells[idx].gridPos = pos;
                activeCells[idx].transform.localPosition = new Vector3(
                    pos.x * 2f * halfCellSize,
                    0f,
                    pos.y * 2f * halfCellSize
                );
            }
        }
    }
}