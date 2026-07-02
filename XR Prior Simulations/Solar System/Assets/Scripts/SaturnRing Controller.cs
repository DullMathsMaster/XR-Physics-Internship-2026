using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaturnRingController : MonoBehaviour
{
    public GameObject parentPlanet;
    public float outerRadius;
    private float scaledOuterDiameter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = parentPlanet.GetComponent<PlanetController>().planetObject.transform.position;
        transform.rotation = parentPlanet.GetComponent<PlanetController>().planetObject.transform.rotation;

        scaledOuterDiameter = outerRadius * PlanetController.planetScaleFactor * 2;
        transform.localScale = new Vector3(scaledOuterDiameter, scaledOuterDiameter, scaledOuterDiameter);
    }
}
