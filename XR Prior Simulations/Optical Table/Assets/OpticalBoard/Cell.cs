using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2Int gridPos;
    public CellIdentity identity;
    public GameObject snapPoint;

    public bool onBoard = true;
    public Collider col;

    private void Start()
    {
        if (!onBoard)
        {
            if (transform.parent != null)
            {
                col = transform.parent.GetComponent<Collider>();
            }

            if (Board.instance != null)
            {
                Board.instance.sliderCells.Add(this);
            }
        }
    }

    private void Update()
    {
        if (identity != null && snapPoint != null)
        {
            identity.CellUpdate(snapPoint.transform.position, this);
        }
    }

    private void OnDestroy()
    {
        if (Board.instance != null)
        {
            Board.instance.sliderCells.Remove(this);
        }

        if (identity != null)
        {
            identity.InvokeDestroy();
        }
    }
}