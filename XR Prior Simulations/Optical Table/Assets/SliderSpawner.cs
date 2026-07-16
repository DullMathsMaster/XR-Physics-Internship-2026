using System.Linq;
using UnityEngine;

public class SliderSpawner : Spawner
{
    [HideInInspector] public LensSlider lensSlider;

    public GameObject lensCartPrefab;
    public bool snappable = false;

    protected override void Spawned(GameObject spawnedObject)
    {
        GameObject lc = Instantiate(lensCartPrefab);

        Cell c = lc.GetComponentInChildren<Cell>();
        if (c != null)
        {
            c.onBoard = false;
        }

        lensSlider = lc.GetComponent<LensSlider>();
        if (lensSlider == null)
        {
            Debug.LogError("Lens cart prefab is missing a LensSlider component.");
            return;
        }

        lensSlider.snappable = snappable;

        lensSlider.cellA = spawnedObject.GetComponent<CellIdentity>();
        lensSlider.cellB = spawnedObject
            .GetComponentsInChildren<CellIdentity>()
            .Where(x => x.transform != spawnedObject.transform)
            .FirstOrDefault();

        SliderSpawner spawnedSliderSpawner = spawnedObject.GetComponent<SliderSpawner>();
        if (spawnedSliderSpawner != null)
        {
            spawnedSliderSpawner.lensSlider = lensSlider;
        }
    }
}