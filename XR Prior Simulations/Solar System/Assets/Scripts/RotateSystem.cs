using UnityEngine;

public class RotateSystem : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50, 0);

    void Update()
    {
        float sharedScale = PlanetController.AngularVelocityScaleFactor / 500f;

        transform.Rotate(rotationSpeed * sharedScale * Time.deltaTime, Space.World);
    }
}