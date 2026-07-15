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
        // Start the Sun in model scale
        sunScaleFactor = 5e-6f;

        scaledSunDiameter =
            sunRadius * sunScaleFactor * 2f;

        sunObject.transform.localScale =
            new Vector3(
                scaledSunDiameter,
                scaledSunDiameter,
                scaledSunDiameter
            );
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(Vector3.up, linearVelocity * Time.deltaTime);
        scaledSunDiameter = sunRadius * sunScaleFactor * 2;
        sunObject.transform.localScale = new Vector3(scaledSunDiameter, scaledSunDiameter, scaledSunDiameter);
    }

    //Calculate Motion Variables
    public void SetScaleFactor(float sliderValue)
    {
        const float trueScale = 1e-7f;
        const float modelPlanetScale = 1e-4f;
        const float modelSunScale = 5e-6f;

        // Convert the slider's scale range into progress from 0 to 1
        float progress = Mathf.InverseLerp(
            trueScale,
            modelPlanetScale,
            sliderValue
        );

        // At true scale: 1e-7
        // At model scale: 5e-6
        sunScaleFactor = Mathf.Lerp(
            trueScale,
            modelSunScale,
            progress
        );

        Debug.Log(
            "Sun slider value: " + sliderValue +
            " Sun scale factor: " + sunScaleFactor
        );
    }
}
