using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPositioner : MonoBehaviour
{
    public GameObject Planet;
    public float PlanetRadius;

    // Start is called before the first frame update
    void Start()
    {
        PlanetRadius = Planet.GetComponent<PlanetController>().scaledPlanetDiameter / 2f;
        // + new Vector3(PlanetRadius + 3f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {

        //transform.rotation = Planet.GetComponent<PlanetController>().transform.rotation;
        

    }
}
