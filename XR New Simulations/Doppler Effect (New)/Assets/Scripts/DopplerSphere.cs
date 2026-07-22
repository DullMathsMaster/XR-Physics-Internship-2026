using UnityEngine;

[RequireComponent(typeof(SineToneGenerator))]
public class DopplerSphere : MonoBehaviour
{
    [Header("Movement")]
    public float startX = -12f;
    public float endX = 12f;
    public float movementSpeed = 5f;

    [Header("Listener")]
    public Transform listener;

    [Header("Sound")]
    public float emittedFrequency = 440f;
    public float speedOfSound = 343f;

    [Header("Reset")]
    public float restartDelay = 1f;

    private SineToneGenerator toneGenerator;
    private bool isMoving = true;
    private float restartTimer;

    private void Start()
    {
        toneGenerator = GetComponent<SineToneGenerator>();

        if (listener == null && Camera.main != null)
        {
            listener = Camera.main.transform;
        }

        ResetSphere();
    }

    private void Update()
    {
        if (listener == null)
        {
            Debug.LogWarning("No listener has been assigned.");
            return;
        }

        if (isMoving)
        {
            MoveSphere();
            UpdateDopplerFrequency();
        }
        else
        {
            WaitAndRestart();
        }
    }

    private void MoveSphere()
    {
        transform.position += Vector3.right * movementSpeed * Time.deltaTime;

        if (transform.position.x >= endX)
        {
            isMoving = false;
            restartTimer = restartDelay;
        }
    }

    private void UpdateDopplerFrequency()
    {
        Vector3 sourceVelocity = Vector3.right * movementSpeed;

        Vector3 directionFromSourceToListener =
            (listener.position - transform.position).normalized;

        float velocityTowardsListener =
            Vector3.Dot(sourceVelocity, directionFromSourceToListener);

        float denominator = speedOfSound - velocityTowardsListener;

        // Prevent division by zero or extreme invalid values.
        denominator = Mathf.Max(denominator, 1f);

        float observedFrequency =
            emittedFrequency * speedOfSound / denominator;

        toneGenerator.frequency = observedFrequency;
    }

    private void WaitAndRestart()
    {
        restartTimer -= Time.deltaTime;

        if (restartTimer <= 0f)
        {
            ResetSphere();
        }
    }

    private void ResetSphere()
    {
        transform.position = new Vector3(
            startX,
            transform.position.y,
            transform.position.z
        );

        toneGenerator.frequency = emittedFrequency;
        isMoving = true;
    }
}