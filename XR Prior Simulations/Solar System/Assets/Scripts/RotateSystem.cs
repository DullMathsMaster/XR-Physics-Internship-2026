using UnityEngine;

public class RotateSystem : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50, 0);

    void Update()
    {
        // Specifying Space.World prevents the X/Y axes from locking up at 90 degrees
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.World);
    }
}