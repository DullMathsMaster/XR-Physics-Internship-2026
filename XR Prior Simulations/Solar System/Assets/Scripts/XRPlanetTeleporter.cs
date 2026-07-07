using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPlanetTeleport : MonoBehaviour
{
    public Teleport teleportScript;
    private XRRayInteractor rayInteractor;

    void Awake()
    {
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    void OnEnable()
    {
        rayInteractor.selectEntered.AddListener(OnPlanetSelected);
    }

    void OnDisable()
    {
        rayInteractor.selectEntered.RemoveListener(OnPlanetSelected);
    }

    private void OnPlanetSelected(SelectEnterEventArgs args)
    {
        // 1. Get the GameObject that was hit by the raycast click
        GameObject hitPlanet = args.interactableObject.transform.gameObject;

        // 2. VISUAL TEST: Search the planet (or its children) for a Renderer to flash green
        Renderer planetRenderer = hitPlanet.GetComponentInChildren<Renderer>();
        if (planetRenderer != null)
        {
            StartCoroutine(FlashPlanetColor(planetRenderer));
        }

        // 3. Match the planet to trigger your script's indexes
        if (hitPlanet == teleportScript.MercurySphere) teleportScript.HandleInputData(0);
        else if (hitPlanet == teleportScript.VenusSphere) teleportScript.HandleInputData(1);
        else if (hitPlanet == teleportScript.EarthSphere) teleportScript.HandleInputData(2);
        else if (hitPlanet == teleportScript.MarsSphere) teleportScript.HandleInputData(3);
        else if (hitPlanet == teleportScript.JupiterSphere) teleportScript.HandleInputData(4);
        else if (hitPlanet == teleportScript.SaturnSphere) teleportScript.HandleInputData(5);
        else if (hitPlanet == teleportScript.UranusSphere) teleportScript.HandleInputData(6);
        else if (hitPlanet == teleportScript.NeptuneSphere) teleportScript.HandleInputData(7);
    }

    // Put this helper function right below the OnPlanetSelected function
    private System.Collections.IEnumerator FlashPlanetColor(Renderer targetRenderer)
    {
        Color originalColor = targetRenderer.material.color;
        targetRenderer.material.color = Color.green; // Turn green on click
        
        yield return new WaitForSeconds(0.5f);
        
        targetRenderer.material.color = originalColor; // Change back to normal
    }
}