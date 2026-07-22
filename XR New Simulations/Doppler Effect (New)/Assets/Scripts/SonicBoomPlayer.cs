using UnityEngine;

public class SonicBoomPlayer : MonoBehaviour
{
    [Header("Boom Audio")]
    [SerializeField] private AudioSource boomAudioSource;

    [Header("Settings")]
    [SerializeField] private float minimumTimeBetweenBooms = 0.5f;

    private bool armed;
    private float lastBoomTime = -100f;

    public void SetArmed(bool isArmed)
    {
        armed = isArmed;
    }

    public void TryPlayBoom()
    {
        if (!armed)
        {
            return;
        }

        if (boomAudioSource == null)
        {
            Debug.LogWarning(
                "SonicBoomPlayer: No boom AudioSource assigned.",
                this
            );

            return;
        }

        if (Time.time - lastBoomTime < minimumTimeBetweenBooms)
        {
            return;
        }

        boomAudioSource.Play();
        lastBoomTime = Time.time;
    }
}