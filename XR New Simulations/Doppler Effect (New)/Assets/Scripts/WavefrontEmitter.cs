using UnityEngine;

public class WavefrontEmitter : MonoBehaviour
{
    [Header("Wavefront")]
    public GameObject wavefrontPrefab;

    [Header("Emission")]
    public float emissionInterval = 0.4f;
    public bool emitImmediately = true;

    [Header("Wave settings")]
    public float waveSpeed = 5f;
    public float maximumRadius = 20f;

    [Header("Spawn position")]
    public Vector3 localSpawnOffset;

    private float timer;

    private void Start()
    {
        timer = emitImmediately ? emissionInterval : 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        while (timer >= emissionInterval)
        {
            EmitWavefront();
            timer -= emissionInterval;
        }
    }

    private void EmitWavefront()
    {
        if (wavefrontPrefab == null)
        {
            Debug.LogError(
                "WavefrontEmitter: No wavefront prefab assigned.",
                this
            );

            return;
        }

        Vector3 spawnPosition =
            transform.TransformPoint(localSpawnOffset);

        GameObject newWavefront = Instantiate(
            wavefrontPrefab,
            spawnPosition,
            Quaternion.identity
        );

        newWavefront.transform.SetParent(null);

        Wavefront wavefront =
            newWavefront.GetComponent<Wavefront>();

        if (wavefront == null)
        {
            Debug.LogError(
                "The Wavefront Prefab does not contain a Wavefront component.",
                newWavefront
            );

            Destroy(newWavefront);
            return;
        }

        wavefront.expansionSpeed = waveSpeed;
        wavefront.maximumRadius = maximumRadius;
    }
}