using UnityEngine;

public class UIFlipLabel : MonoBehaviour
{
    public GameObject Planet;
    public float PlanetRadius;

    void Start()
    {
        if (Planet != null)
        {
            PlanetController controller = Planet.GetComponent<PlanetController>();

            if (controller != null)
            {
                PlanetRadius = controller.scaledPlanetDiameter / 2f;
            }
            else
            {
                Debug.LogError("The object assigned to Planet does not have a PlanetController component!");
            }
        }
        else
        {
            Debug.LogError("No Planet object assigned in UILabel!");
        }

        Debug.Log("UILabel running on: " + gameObject.name);

        initialLocalRotation = transform.localRotation;

        // Determine which local face of the label is the visible text face at startup.
        // We test both local forward and local back in world space and pick the one
        // whose normal points more toward the camera, then set the initial `flipped`
        // state so the visible face faces the camera.
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 toCamera = cam.transform.position - transform.position;
            Vector3 toCameraFlat = new Vector3(toCamera.x, 0f, toCamera.z);
            if (toCameraFlat.sqrMagnitude > 0.0001f)
            {
                toCameraFlat.Normalize();
                Quaternion baseRotation = transform.parent != null ? transform.parent.rotation * initialLocalRotation : initialLocalRotation;

                Vector3 forwardWorld = baseRotation * Vector3.forward;
                Vector3 backWorld = baseRotation * Vector3.back;

                Vector3 forwardFlat = new Vector3(forwardWorld.x, 0f, forwardWorld.z).normalized;
                Vector3 backFlat = new Vector3(backWorld.x, 0f, backWorld.z).normalized;

                float dotForward = Vector3.Dot(forwardFlat, toCameraFlat);
                float dotBack = Vector3.Dot(backFlat, toCameraFlat);

                // Choose the face (forward or back) that points more toward the camera.
                // The boolean needs to match the initial rotation so the label does not
                // briefly snap to the wrong side after teleporting.
                if (dotBack > dotForward)
                {
                    flipped = true;
                    transform.localRotation = initialLocalRotation * Quaternion.Euler(0f, 180f, 0f);
                }
                else
                {
                    flipped = false;
                    transform.localRotation = initialLocalRotation;
                }
            }
        }
    }

    private bool flipped = false;
    private Quaternion initialLocalRotation;
    private const float flipInThreshold = 0.25f;
    private const float flipOutThreshold = -0.25f;

    void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("No Main Camera found");
            return;
        }

        Vector3 toCamera = cam.transform.position - transform.position;
        Vector3 toCameraFlat = new Vector3(toCamera.x, 0f, toCamera.z);
        if (toCameraFlat.sqrMagnitude < 0.0001f)
        {
            return;
        }

        toCameraFlat.Normalize();

        Quaternion baseRotation = transform.parent != null ? transform.parent.rotation * initialLocalRotation : initialLocalRotation;
        Vector3 baseForwardWorld = baseRotation * Vector3.back;
        Vector3 baseForwardFlat = new Vector3(baseForwardWorld.x, 0f, baseForwardWorld.z).normalized;

        float facing = Vector3.Dot(baseForwardFlat, toCameraFlat);
        bool targetFlip = flipped;

        if (facing < flipOutThreshold)
        {
            targetFlip = true;
        }
        else if (facing > flipInThreshold)
        {
            targetFlip = false;
        }

        if (targetFlip != flipped)
        {
            flipped = targetFlip;
            float yaw = flipped ? 180f : 0f;
            transform.localRotation = initialLocalRotation * Quaternion.Euler(0f, yaw, 0f);
        }
    }
}
