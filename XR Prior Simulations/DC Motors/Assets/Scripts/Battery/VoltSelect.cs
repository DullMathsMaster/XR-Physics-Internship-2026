using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Changes the battery's voltage based on the rotation of the dial
/// </summary>
public class VoltSelect : MonoBehaviour
{
    public float voltage;
    
    void Start()
    {
        voltage = 0.0f;
    }

    void Update()
    {
        
    }
    // VX corresponds to the dial facing number X on the battery
    // voltage values are altered to match the value the dial is pointing at
    // 0V is given 0.05 volts as a way of confirming that the battery has been switched on
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trying");
        if (other.gameObject.tag == "V0")
        {
            Debug.Log("Registered Collision");
            voltage = 0.05f;
        }
        if (other.gameObject.tag == "V2")
        {
            Debug.Log("Registered Collision");
            voltage = 2.0f;
        }
        if (other.gameObject.tag == "V4")
        {   
            Debug.Log("Registered Collision");
            voltage = 4.0f;
        }
        if (other.gameObject.tag == "V6")
        {
            Debug.Log("Registered Collision");
            voltage = 6.0f;
        }
        if (other.gameObject.tag == "V8")
        {
            Debug.Log("Registered Collision");
            voltage = 8.0f;
        }
        if (other.gameObject.tag == "V10")
        {
            Debug.Log("Registered Collision");
            voltage = 10.0f;
        }
        if (other.gameObject.tag == "V12")
        {
            Debug.Log("Registered Collision");
            voltage = 12.0f;
        }
    }

}
