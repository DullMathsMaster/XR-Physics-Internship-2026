using UnityEngine;

public class ContainerHandle : MonoBehaviour
{
    [Header("Quest interaction bridge")]
    public Transform followTarget;
    [HideInInspector] public bool isGrabbed;

    private Transform parentTransform;

    private void Start()
    {
        parentTransform = transform.parent;
    }

    private void Update()
    {
        if (!isGrabbed || followTarget == null || parentTransform == null)
            return;

        parentTransform.position = followTarget.position - transform.localPosition;
        parentTransform.rotation = Quaternion.Euler(0f, followTarget.rotation.eulerAngles.y, 0f);
    }
}