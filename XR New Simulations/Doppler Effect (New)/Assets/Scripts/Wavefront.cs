using UnityEngine;

public class Wavefront : MonoBehaviour
{
    [Header("Expansion")]
    public float expansionSpeed = 5f;
    public float maximumRadius = 20f;

    [Header("Fading")]
    [Range(0f, 1f)]
    public float startingAlpha = 0.25f;

    [Range(0f, 1f)]
    public float endingAlpha = 0f;

    private static readonly int ShellAlphaID =
        Shader.PropertyToID("_ShellAlpha");

    private Renderer waveRenderer;
    private Material waveMaterial;
    private float currentRadius;

    private void Awake()
    {
        waveRenderer = GetComponent<Renderer>();

        if (waveRenderer == null)
        {
            Debug.LogError(
                "Wavefront requires a Renderer component.",
                this
            );

            enabled = false;
            return;
        }

        waveMaterial = waveRenderer.material;

        currentRadius = transform.localScale.x * 0.5f;

        SetAlpha(startingAlpha);
    }

    private void Update()
    {
        currentRadius += expansionSpeed * Time.deltaTime;

        float diameter = currentRadius * 2f;

        transform.localScale =
            Vector3.one * diameter;

        float fadeProgress = Mathf.InverseLerp(
            0f,
            maximumRadius,
            currentRadius
        );

        float currentAlpha = Mathf.Lerp(
            startingAlpha,
            endingAlpha,
            fadeProgress
        );

        SetAlpha(currentAlpha);

        if (currentRadius >= maximumRadius)
        {
            Destroy(gameObject);
        }
    }

    private void SetAlpha(float alpha)
    {
        if (waveMaterial == null)
        {
            return;
        }

        waveMaterial.SetFloat(
            ShellAlphaID,
            alpha
        );
    }

    private void OnDestroy()
    {
        if (waveMaterial != null)
        {
            Destroy(waveMaterial);
        }
    }
}