using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Teleport : MonoBehaviour
{
    public GameObject MercurySphere;
    public GameObject VenusSphere;
    public GameObject EarthSphere;
    public GameObject MarsSphere;
    public GameObject JupiterSphere;
    public GameObject SaturnSphere;
    public GameObject UranusSphere;
    public GameObject NeptuneSphere;
    public GameObject CeresSphere;

    public Vector3 PlanetLocation;
    public float PlanetScale;

    public void HandleInputData(int val)
    {
        if (val == 0) //Mercury
        {
            PlanetLocation = MercurySphere.transform.position;
            PlanetScale = MercurySphere.transform.localScale.x;
        }

        if (val == 1) //Venus
        {
            PlanetLocation = VenusSphere.transform.position;
            PlanetScale = VenusSphere.transform.localScale.x;
        }

        if (val == 2) //Earth
        {
            PlanetLocation = EarthSphere.transform.position;
            PlanetScale = EarthSphere.transform.localScale.x;
        }

        if (val == 3) //Mars
        {
            PlanetLocation = MarsSphere.transform.position;
            PlanetScale = MarsSphere.transform.localScale.x;
        }

        if (val == 4) //Jupiter
        {
            PlanetLocation = JupiterSphere.transform.position;
            PlanetScale = JupiterSphere.transform.localScale.x;
        }

        if (val == 5) //Saturn
        {
            PlanetLocation = SaturnSphere.transform.position;
            PlanetScale = SaturnSphere.transform.localScale.x;
        }

        if (val == 6) //Uranus
        {
            PlanetLocation = UranusSphere.transform.position;
            PlanetScale = UranusSphere.transform.localScale.x;
        }

        if (val == 7) //Neptune
        {
            PlanetLocation = NeptuneSphere.transform.position;
            PlanetScale = NeptuneSphere.transform.localScale.x;
        }

        if (val == 8) //Neptune
        {
            PlanetLocation = CeresSphere.transform.position;
            PlanetScale = CeresSphere.transform.localScale.x;
        }

        //transform.position = PlanetLocation + new Vector3(PlanetScale * 4, 0, 0);
        float anglefromplanet = (400f / Vector3.Magnitude(PlanetLocation)) + (PlanetScale * 0.6f);
        float theta = Mathf.Atan2(PlanetLocation.z, PlanetLocation.x) - (anglefromplanet * Mathf.PI / 180f);
        float r = Mathf.Sqrt(Mathf.Pow(PlanetLocation.x, 2) + Mathf.Pow(PlanetLocation.z, 2));
        transform.position = new Vector3( r*Mathf.Cos(theta), 0, r * Mathf.Sin(theta));
        

        Quaternion rotation = Quaternion.LookRotation(PlanetLocation - transform.position);
        transform.rotation = rotation;
    }
}
