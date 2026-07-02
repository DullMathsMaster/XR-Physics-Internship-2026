using UnityEngine;

public class MoonController : MonoBehaviour
{

    public float moonInclination;
    public float moonAxisTilt;
    public float moonMass;
    public float moonOrbitRadius = 1e8f;
    public float moonRadius = 1.0f;
    public GameObject moonObject;
    //public GameObject orbitPathObject;
    public GameObject moonOrbitingObject;
    public GameObject moonOrbitingSphere;
    public GameObject Earth;
    public GameObject Label;
    public bool clockwiseOrbit;
    public bool clockwiseRotation;
    public float earthHoursPerRotation;


    //public static float orbitScaleFactor = 1e-7f;
    public static float planetScaleFactor = 1e-4f;
    //public static float AngularVelocityScaleFactor = 5e2f;
    private float earthAngularVelocity;


    private float G = 6.6743e-11f;

    private float scaledOrbitRadius;
    private float scaledMoonDiameter;
    public float scaledOrbitAngularVelocity;
    private float unscaledOrbitAngularVelocity;
    private float scaledRotationAngularVelocity;



    // Start is called before the first frame update
    void Start()
    {
        unscaledOrbitAngularVelocity = -Mathf.Sqrt(G * moonOrbitingObject.GetComponent<PlanetController>().mass / Mathf.Pow(moonOrbitRadius, 3));
        if (clockwiseOrbit)
        {
            unscaledOrbitAngularVelocity = -unscaledOrbitAngularVelocity;
        }
        
        scaledOrbitRadius = moonOrbitRadius * PlanetController.orbitScaleFactor;
        Debug.Log("Moon Orbit Radius " + scaledOrbitRadius.ToString());
        transform.position = Vector3.zero;
        moonObject.transform.position = new Vector3(1f, 0, 0);
        //Debug.Log(gameObject.name + "    Orbit Radius " + scaledOrbitRadius.ToString());
        
        
        //moonObject.transform.position = new Vector3(scaledOrbitRadius, 0, 0);

        moonObject.transform.Rotate(0, 0, moonAxisTilt);
        transform.Rotate(0, 0, moonInclination);
        scaledMoonDiameter = moonRadius * PlanetController.planetScaleFactor * 2;
        Label.transform.position = moonObject.transform.position + new Vector3(0f, (scaledMoonDiameter / 2f) + 0.2f, 0f);
    }

    // Update is called once per frame
    void Update()
    {

        scaledMoonDiameter = moonRadius * PlanetController.planetScaleFactor * 2;
        moonObject.transform.localScale = new Vector3(scaledMoonDiameter, scaledMoonDiameter, scaledMoonDiameter);

        transform.position = moonOrbitingSphere.transform.position;
        scaledOrbitAngularVelocity = unscaledOrbitAngularVelocity * PlanetController.AngularVelocityScaleFactor;
        earthAngularVelocity = Earth.GetComponent<PlanetController>().scaledOrbitAngularVelocity;
        scaledRotationAngularVelocity = earthAngularVelocity * 8760f / earthHoursPerRotation;
        transform.Rotate(Vector3.up, scaledOrbitAngularVelocity * Time.deltaTime);
        //moonObject.transform.Rotate(Vector3.up, scaledOrbitAngularVelocity * Time.deltaTime);
    }

}