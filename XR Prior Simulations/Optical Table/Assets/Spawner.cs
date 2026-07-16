using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using LightTK;

[RequireComponent(typeof(XRSimpleInteractable))]
public class Spawner : MonoBehaviour
{
    [Header("Spawn settings")]
    public GameObject prefabToSpawn;
    public Transform spawnPoint;

    private XRSimpleInteractable simpleInteractable;

    protected virtual void Awake()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
    }

    protected virtual void OnEnable()
    {
        simpleInteractable.selectEntered.AddListener(OnSelectEntered);
    }

    protected virtual void OnDisable()
    {
        if (simpleInteractable == null) return;
        simpleInteractable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (prefabToSpawn == null)
            return;

        StartCoroutine(SpawnAndTransferToHand(args));
    }

    private IEnumerator SpawnAndTransferToHand(SelectEnterEventArgs args)
    {
        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        GameObject newObject = Instantiate(prefabToSpawn, pos, rot);

        XRGrabInteractable spawnedGrab = newObject.GetComponent<XRGrabInteractable>();
        if (spawnedGrab == null)
        {
            Debug.LogError("Spawned prefab is missing XRGrabInteractable.");
            yield break;
        }

        LightRayEmitter.colliders = FindObjectsByType<LTKCollider>(FindObjectsSortMode.None);

        Spawned(newObject);

        yield return null;

        var interactor = args.interactorObject;
        var interactionManager = simpleInteractable.interactionManager;

        if (interactor != null && interactionManager != null)
        {
            interactionManager.SelectEnter(interactor, spawnedGrab);
        }
    }

    protected virtual void Spawned(GameObject spawnedObject) { }
}