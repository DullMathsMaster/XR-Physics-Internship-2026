using UnityEngine;

public class DisableGravity : MonoBehaviour
{
    void Awake()
    {
        Physics.gravity = Vector3.zero;
    }
}