using UnityEngine;

public class PlanetController : MonoBehaviour
{

    public float rotationTilt;
    public float mass;
    public float orbitRadius = 1e8f;
    public float planetRadius = 1.0f;
    public GameObject planetObject;
    public GameObject orbitPathObject;
    public GameObject orbitingObject;
    public GameObject Earth;
    public GameObject UI;
    public GameObject UI2;
    public GameObject Label;
    public bool clockwiseOrbit;
    public bool clockwiseRotation;
    public float earthHoursPerDay;


    public static float orbitScaleFactor = 1e-7f;
    public static float planetScaleFactor = 1e-4f;
    public static float AngularVelocityScaleFactor = 5e2f;
    private float earthAngularVelocity;


    private float G = 6.6743e-11f;

    private float scaledOrbitRadius;
    public float scaledPlanetDiameter;
    public float scaledOrbitAngularVelocity;
    private float unscaledOrbitAngularVelocity;
    private float scaledRotationAngularVelocity;


    

    // Start is called before the first frame update
    void Start()
    {
        unscaledOrbitAngularVelocity = -Mathf.Sqrt(G * orbitingObject.GetComponent<SunController2>().sunMass / Mathf.Pow(orbitRadius, 3));       
        if (clockwiseOrbit)
        {
            unscaledOrbitAngularVelocity = -unscaledOrbitAngularVelocity;
        }
        //earthAngularVelocity = Earth.GetComponent<PlanetController>().scaledOrbitAngularVelocity;
        //scaledRotationAngularVelocity = earthAngularVelocity * 8760f/earthHoursPerDay;
        //scaledPlanetDiameter = planetRadius * planetScaleFactor * 2;
        scaledOrbitRadius = orbitRadius * orbitScaleFactor;

        //planetObject.transform.localScale = new Vector3(scaledPlanetDiameter, scaledPlanetDiameter, scaledPlanetDiameter);
        orbitPathObject.transform.localScale = new Vector3(scaledOrbitRadius * 10, scaledOrbitRadius * 10, scaledOrbitRadius * 10);
        planetObject.transform.position = new Vector3(scaledOrbitRadius, 0, 0);      
        planetObject.transform.Rotate(0, 0, rotationTilt);
        scaledPlanetDiameter = planetRadius * planetScaleFactor * 2;
        UI.transform.position = planetObject.transform.position + new Vector3((scaledPlanetDiameter / 1.5f) + 2f, 0f, 0f);
        if (UI2 != null)
        {
            UI2.transform.position = planetObject.transform.position + new Vector3((scaledPlanetDiameter / 1.5f) + 2f, 0f, 0f);
        }

        Label.transform.position = planetObject.transform.position + new Vector3(0f, (scaledPlanetDiameter / 2f) + 1f, 0f);
        transform.Rotate(Vector3.up, Random.Range(0f,360f));
        //Debug.Log(gameObject.name + "    Angular Velocity " + unscaledOrbitAngularVelocity.ToString() + "   Planet Diameter " + scaledPlanetDiameter.ToString() + "   Planet Orbit " + scaledOrbitRadius.ToString());

    }

    // Update is called once per frame
    void Update()
    {
        scaledOrbitAngularVelocity = unscaledOrbitAngularVelocity * AngularVelocityScaleFactor;
        earthAngularVelocity = Earth.GetComponent<PlanetController>().scaledOrbitAngularVelocity;
        scaledRotationAngularVelocity = earthAngularVelocity * 8760f / earthHoursPerDay; //CHANGE THIS
        transform.Rotate(Vector3.up, scaledOrbitAngularVelocity * Time.deltaTime);
        planetObject.transform.Rotate(Vector3.up, scaledRotationAngularVelocity * Time.deltaTime);

        scaledPlanetDiameter = planetRadius * planetScaleFactor * 2;
        planetObject.transform.localScale = new Vector3(scaledPlanetDiameter, scaledPlanetDiameter, scaledPlanetDiameter);      
    }

    public void SetSpeed(float value)
    {
        AngularVelocityScaleFactor = value;
    }

    public void SetScaleFactor(float value)
    {
        planetScaleFactor = value;
    }
}