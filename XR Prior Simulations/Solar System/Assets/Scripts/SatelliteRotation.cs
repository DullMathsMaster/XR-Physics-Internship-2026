using UnityEngine;

public class SatelliteRotation : MonoBehaviour
{
    // Speed of rotation in degrees per second
    public float orbitalSpeed = 30.0f;

    void Update()
    {
        transform.Rotate(Vector3.up, orbitalSpeed * Time.deltaTime);
    }
}