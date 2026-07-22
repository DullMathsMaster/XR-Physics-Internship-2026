using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ARTrackImageSpawner : MonoBehaviour
{   
    [SerializeField] private TMP_Text textBox; 
    [SerializeField] private GameObject CharizardPrefab;

    private GameObject charizard;
    private ARTrackedImageManager aRTrackedImageManager;

    private void OnEnable()
    {
        aRTrackedImageManager = gameObject.GetComponent<ARTrackedImageManager>();
        aRTrackedImageManager.trackablesChanged.AddListener(OnImageChanged);

    }

    private void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            TrySpawnCharizard(newImage);
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            TrySpawnCharizard(updatedImage);
        }
    }

    private void TrySpawnCharizard(ARTrackedImage trackedImage)
    {
        if (charizard != null)
            return;

        if (trackedImage.trackingState != TrackingState.Tracking)
            return;

        charizard = Instantiate(CharizardPrefab, trackedImage.transform);
        textBox.text = "Charizard image detected!";
    }

    public void DestroyCharizard()
    {
        if (charizard == null)
            return;

        Destroy(charizard);
        charizard = null;
        textBox.text = "Image not detected";
    }

}
