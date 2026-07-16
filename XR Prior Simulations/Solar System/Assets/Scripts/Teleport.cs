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

    private bool isTracking = false;
    private int currentPlanetIndex = -1;

    private float zoomMultiplier = 1.0f;
    private float heightOffset = 0f;

    public void StartTracking()
    {
        if (currentPlanetIndex != -1)
        {
            isTracking = true;
        }
    }

    public void StopTracking()
    {
        isTracking = false;
        transform.SetParent(null);
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    }
    private void LateUpdate()
    {
        if (isTracking && currentPlanetIndex != -1)
        {
            HandleInputData(currentPlanetIndex);
        }
    }

    public void ZoomIn()
    {
        if (currentPlanetIndex != -1)
        {
            zoomMultiplier = 0.4f;
            heightOffset = -1.0f;
            HandleInputData(currentPlanetIndex);
        }
    }

    public void ResetZoom()
    {
        zoomMultiplier = 1.0f;
        heightOffset = 0f;
        if (currentPlanetIndex != -1)
        {
            HandleInputData(currentPlanetIndex);
        }
    }

    public void HandleInputData(int val)
    {
        currentPlanetIndex = val;

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

        if (val == 8) //Ceres
        {
            PlanetLocation = CeresSphere.transform.position;
            PlanetScale = CeresSphere.transform.localScale.x;
        }

        float anglefromplanet = ((400f / Vector3.Magnitude(PlanetLocation)) + (PlanetScale * 0.6f)) * zoomMultiplier;
        
        float theta = Mathf.Atan2(PlanetLocation.z, PlanetLocation.x) - (anglefromplanet * Mathf.PI / 180f);
        float r = Mathf.Sqrt(Mathf.Pow(PlanetLocation.x, 2) + Mathf.Pow(PlanetLocation.z, 2));
        
        float targetY = PlanetLocation.y + heightOffset;
        
        transform.position = new Vector3(r * Mathf.Cos(theta), targetY, r * Mathf.Sin(theta));
        Vector3 lookDirection = PlanetLocation - transform.position;
        lookDirection.y = 0; 
        
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
    }
}