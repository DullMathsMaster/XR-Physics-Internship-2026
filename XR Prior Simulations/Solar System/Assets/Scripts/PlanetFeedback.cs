using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlanetFeedback : MonoBehaviour
{
    [Header("Visual Feedback Settings")]
    [Tooltip("Drag a Particle System Prefab here (e.g., a sparkle, flare, or ring burst)")]
    public GameObject clickVisualEffectPrefab;

    public void TriggerFlash(BaseInteractionEventArgs args)
    {
        // 1. Verify we have a valid interactor hitting the object
        if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            // 2. Get the exact point in 3D space where the ray hit the collider
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                SpawnEffect(hit.point);
            }
            else
            {
                // Fallback: if raycast data isn't exposed, spawn it right at the planet's position
                SpawnEffect(transform.position);
            }
        }
    }

    private void SpawnEffect(Vector3 spawnPosition)
    {
        if (clickVisualEffectPrefab != null)
        {
            // Spawn the visual effect object at the target point
            GameObject spawnedEffect = Instantiate(clickVisualEffectPrefab, spawnPosition, Quaternion.identity);
            
            // Automatically destroy the visual effect after 1 second so it doesn't clutter your scene
            Destroy(spawnedEffect, 1.0f);
        }
    }
}