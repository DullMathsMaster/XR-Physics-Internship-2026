using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandling : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject magnetPrefab;
    public GameObject positivePrefab;
    public GameObject negativePrefab;

    [Header("Buttons")]
    public Button spawnMagnetButton;
    public Button spawnPositiveButton;
    public Button spawnNegativeButton;
    public Button deleteAllButton;
    public Button togglePosButton;
    public Button toggleNegButton;
    public Button toggleSimulateButton;

    [Header("Optional Labels")]
    public TMP_Text togglePosLabel;
    public TMP_Text toggleNegLabel;
    public TMP_Text toggleSimulateLabel;

    [Header("Spawn")]
    public Transform spawnPoint;

    private readonly System.Collections.Generic.List<GameObject> spawnedObjects = new();

    private bool renderPos = false;
    private bool renderNeg = false;
    private bool simulate = false;

    void Start()
    {
        if (spawnMagnetButton != null)
            spawnMagnetButton.onClick.AddListener(() => SpawnObject(magnetPrefab));

        if (spawnPositiveButton != null)
            spawnPositiveButton.onClick.AddListener(() => SpawnObject(positivePrefab));

        if (spawnNegativeButton != null)
            spawnNegativeButton.onClick.AddListener(() => SpawnObject(negativePrefab));

        if (deleteAllButton != null)
            deleteAllButton.onClick.AddListener(DeleteAll);

        if (togglePosButton != null)
            togglePosButton.onClick.AddListener(TogglePositive);

        if (toggleNegButton != null)
            toggleNegButton.onClick.AddListener(ToggleNegative);

        if (toggleSimulateButton != null)
            toggleSimulateButton.onClick.AddListener(ToggleSimulate);

        RefreshLabels();
    }

    void SpawnObject(GameObject prefab)
    {
        if (prefab == null || spawnPoint == null) return;

        GameObject obj = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        spawnedObjects.Add(obj);
    }

    void DeleteAll()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] != null)
                Destroy(spawnedObjects[i]);
        }

        spawnedObjects.Clear();
    }

    void TogglePositive()
    {
        renderPos = !renderPos;
        RefreshLabels();

        Debug.Log("Positive lines: " + renderPos);
    }

    void ToggleNegative()
    {
        renderNeg = !renderNeg;
        RefreshLabels();

        Debug.Log("Negative lines: " + renderNeg);
    }

    void ToggleSimulate()
    {
        simulate = !simulate;
        RefreshLabels();

        Debug.Log("Simulation: " + simulate);
    }

    void RefreshLabels()
    {
        if (togglePosLabel != null)
            togglePosLabel.text = renderPos ? "Disable Positive Field Lines" : "Render Positive Field Lines";

        if (toggleNegLabel != null)
            toggleNegLabel.text = renderNeg ? "Disable Negative Field Lines" : "Render Negative Field Lines";

        if (toggleSimulateLabel != null)
            toggleSimulateLabel.text = simulate ? "Disable Simulation" : "Enable Simulation";
    }
}