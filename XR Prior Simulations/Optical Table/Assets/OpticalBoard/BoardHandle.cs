using UnityEngine;

public class BoardHandle : MonoBehaviour
{
    public Board board;
    public HandleType handleType;

    public enum HandleType
    {
        Left,
        Right,
        Top,
        Bottom
    }

    private void Awake()
    {
        if (board == null)
        {
            board = GetComponentInParent<Board>();
        }
    }

    public void GrabBegin()
    {
        SetGrabState(true);
    }

    public void GrabEnd()
    {
        SetGrabState(false);
    }

    private void OnDisable()
    {
        SetGrabState(false);
    }

    private void SetGrabState(bool grabbed)
    {
        if (board == null) return;

        switch (handleType)
        {
            case HandleType.Left:
                board.leftGrabbed = grabbed;
                break;

            case HandleType.Right:
                board.rightGrabbed = grabbed;
                break;

            case HandleType.Top:
                board.topGrabbed = grabbed;
                break;

            case HandleType.Bottom:
                board.bottomGrabbed = grabbed;
                break;
        }
    }
}