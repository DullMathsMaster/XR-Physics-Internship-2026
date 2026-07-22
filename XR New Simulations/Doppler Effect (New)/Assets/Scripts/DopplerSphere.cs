using UnityEngine;

[RequireComponent(typeof(SineToneGenerator))]
public class DopplerSphere : MonoBehaviour
{
    [Header("Movement")]
    public float startX = -20f;
    public float endX = 20f;
    public float movementSpeed = 0f;

    [Header("Listener")]
    public Transform listener;

    [Header("Sound")]
    public float emittedFrequency = 440f;
    public float speedOfSound = 5f;

    [Header("Reset")]
    public float restartDelay = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource toneAudioSource;

    private SineToneGenerator toneGenerator;

    private bool isMoving = true;
    private bool sonicMode;
    private float restartTimer;

    private void Start()
    {
        toneGenerator = GetComponent<SineToneGenerator>();

        if (toneAudioSource == null)
        {
            toneAudioSource = GetComponent<AudioSource>();
        }

        if (listener == null && Camera.main != null)
        {
            listener = Camera.main.transform;
        }

        ResetSphere();
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveSphere();

            if (listener != null && !sonicMode)
            {
                UpdateDopplerFrequency();
            }
        }
        else
        {
            WaitAndRestart();
        }
    }

    private void MoveSphere()
    {
        transform.position +=
            Vector3.right * movementSpeed * Time.deltaTime;

        if (transform.position.x >= endX)
        {
            isMoving = false;
            restartTimer = restartDelay;
        }
    }

    private void UpdateDopplerFrequency()
    {
        Vector3 sourceVelocity =
            Vector3.right * movementSpeed;

        Vector3 directionFromSourceToListener =
            (listener.position - transform.position).normalized;

        float velocityTowardsListener =
            Vector3.Dot(
                sourceVelocity,
                directionFromSourceToListener
            );

        float denominator =
            speedOfSound - velocityTowardsListener;

        denominator = Mathf.Max(denominator, 0.01f);

        float observedFrequency =
            emittedFrequency * speedOfSound / denominator;

        toneGenerator.frequency = observedFrequency;
    }

    public void SetSonicMode(bool enabled)
    {
        sonicMode = enabled;

        if (toneAudioSource != null)
        {
            toneAudioSource.mute = enabled;
        }

        if (!enabled && toneGenerator != null)
        {
            toneGenerator.frequency = emittedFrequency;
        }
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

        if (toneGenerator != null)
        {
            toneGenerator.frequency = emittedFrequency;
        }

        isMoving = true;
    }
}