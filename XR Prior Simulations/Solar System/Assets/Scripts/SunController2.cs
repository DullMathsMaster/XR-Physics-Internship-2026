using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController2 : MonoBehaviour
{
    public float sunMass = 1.0f;
    public float sunRadius = 1.0f;
    public GameObject sunObject;

    public static float sunScaleFactor = 5e-6f;

    public float scaledSunDiameter;
    private float scaledSunRadius;
    private float sunScale;


    // Start is called before the first frame update
    void Start()
    {
        //scaledSunDiameter = sunRadius * sunScaleFactor * 2;
        //sunObject.transform.localScale = new Vector3(scaledSunDiameter, scaledSunDiameter, scaledSunDiameter);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(Vector3.up, linearVelocity * Time.deltaTime);
        scaledSunDiameter = sunRadius * sunScaleFactor * 2;
        sunObject.transform.localScale = new Vector3(scaledSunDiameter, scaledSunDiameter, scaledSunDiameter);
    }

    //Calculate Motion Variables
    private void CalculateSunMotionVariables()
    {
        //force = G*orbitingObject.GetComponent<SunController>().mass * (mass) / (orbitRadius * orbitRadius);
        //linearVelocity = Mathf.Sqrt(force * orbitRadius/mass);
        //linearVelocity = Mathf.Sqrt(G * orbitingObject.GetComponent<SunController>().mass / orbitRadius) * AngularVelocityScaleFactor;
    }

    //Calculate Scaled Variables
    private void CalculateSunScaledVariables()
    {
        //scaledOrbitRadius = orbitRadius * orbitScaleFactor;
        //scaledSunRadius = sunRadius * sunScaleFactor;
        //sunColliderRadius = sunObject.GetComponent<SphereCollider>().radius;
        //scaledSunMass = orbitingObject.GetComponent<SunController>().mass * massScaleFactor;
    }

    public void SetScaleFactor(float valueonslider)
    {
        sunScaleFactor = Mathf.Abs((5e-6f - 10e-7f)/(1e-4f - 1e-7f))*valueonslider;
    }
}
