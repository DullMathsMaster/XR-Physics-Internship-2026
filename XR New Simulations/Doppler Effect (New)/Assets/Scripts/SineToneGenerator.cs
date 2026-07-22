using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SineToneGenerator : MonoBehaviour
{
    [Header("Tone Settings")]
    [Tooltip("The frequency currently being produced, measured in hertz.")]
    public float frequency = 440f;

    [Range(0f, 1f)]
    public float volume = 0.1f;

    private double phase;
    private int sampleRate;

    private void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.dopplerLevel = 0f;

        audioSource.Play();
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        double phaseIncrement =
            2.0 * Mathf.PI * frequency / sampleRate;

        for (int sample = 0; sample < data.Length; sample += channels)
        {
            float value = Mathf.Sin((float)phase) * volume;

            for (int channel = 0; channel < channels; channel++)
            {
                data[sample + channel] = value;
            }

            phase += phaseIncrement;

            if (phase > 2.0 * Mathf.PI)
            {
                phase -= 2.0 * Mathf.PI;
            }
        }
    }
}