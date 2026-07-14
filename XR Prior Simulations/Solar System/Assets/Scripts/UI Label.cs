using UnityEngine;

public class UILabel : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null)
            return;

        // Face the camera
        transform.LookAt(Camera.main.transform);

        // Flip because UI faces backwards
        transform.Rotate(0f, 180f, 0f);
    }
}