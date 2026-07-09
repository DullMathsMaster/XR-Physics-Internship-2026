using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
/// <summary>
/// Controls what happens when the wire enters the crocodile clips
/// </summary>
public class ConnectClip : MonoBehaviour
{
    public float newYRotation;
    private float oldYRotation;
    public Vector3 currentRotation;

    public GameObject voltProvider;    
    public float voltage;
    private BatteryConnect getVoltage;
    public string polarity;
    

    // Start is called before the first frame update
    void Start()
    {
        // open up the clip
        newYRotation = 8.5f;
        oldYRotation = 10.535f;
        // get voltage values
        getVoltage = voltProvider.GetComponent<BatteryConnect>();        

    }

    // Update is called once per frame
    void Update()
    {
        voltage = getVoltage.voltage;
        polarity = getVoltage.polarity;
    }

    private void OnTriggerEnter(Collider other)
    {
        // if either side of any open wire enters the clip
        if (other.gameObject.tag == "leftFreeWire" || other.gameObject.tag == "rightFreeWire")
        {
            Debug.Log("IN");
            // play connection audio
            gameObject.GetComponent<AudioSource>().Play();
            // change rotation to open up the clip
            currentRotation = transform.localEulerAngles;
            currentRotation.y = newYRotation;
            transform.localRotation = Quaternion.Euler(currentRotation);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // if either side of any open wire leaves the clip
        if (other.gameObject.tag == "leftFreeWire" || other.gameObject.tag == "rightFreeWire")
        {
            Debug.Log("OUT");
            // play connection audio
            gameObject.GetComponent<AudioSource>().Play();
            // change rotation to close the clip
            currentRotation = transform.localEulerAngles;
            currentRotation.y = oldYRotation;
            transform.localRotation = Quaternion.Euler(currentRotation);
        }
    }
}
