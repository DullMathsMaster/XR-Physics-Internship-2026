using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Very important. Controls what happens when wires touch each other. Handles the transfer of electricity and polarity
/// </summary>
public class WireReaction : MonoBehaviour
{
    public float voltage;
    public string polarity;
    public bool connected;
    public bool connectedWire;
    public GameObject otherSide;
    public GameObject mainBody;

    private ConnectClip getVoltage;
    private WireReaction getWireVoltage;
    private WireReaction flowVoltage;
    private GameObject voltProvider;
    
    void Start()
    {
        // initialise the necessary variables
        voltage = 0f;
        polarity = "none";
        connected = false;
        connectedWire = false;
        flowVoltage = otherSide.GetComponent<WireReaction>();
    }
  
    void Update()
    {
        // get the voltage and polarity of the OTHER SIDE OF THE SAME WIRE
        flowVoltage = otherSide.GetComponent<WireReaction>();
        Rigidbody rb = mainBody.GetComponent<Rigidbody>();
        // if the wire is connected to the clip
        if (connected)
        {
            voltage = getVoltage.voltage;
            polarity = getVoltage.polarity;
            if (rb != null)
            {
                // prevent connected wires from being influenced by collisions
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }
        // if the wire is connected to another
        else if (connectedWire)
        {
            voltage = getWireVoltage.voltage;
            polarity = getWireVoltage.polarity;
            // if the wire is receiving electricity
            if (voltage > 1f)
            {
                // play connection sound
                gameObject.GetComponent<AudioSource>().Play();
            }
            if (rb != null)
            {
                // prevent connected wires from being influenced by collisions
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }
        // IF THE OTHER SIDE OF THE SAME WIRE has a higher voltage, mimic its values
        else if ((flowVoltage.connected == true || flowVoltage.connectedWire == true) && flowVoltage.voltage > voltage)
        {
            voltage = flowVoltage.voltage;
            polarity = flowVoltage.polarity;
        }
        else
        {
            voltage = 0f;
            polarity = "none";
            //if (!rb.useGravity)
            //{
            //    rb.useGravity = true;
            //    rb.isKinematic = false;
            //}
        }

        // IF THE OTHER SIDE OF THE SAME WIRE has a higher voltage, mimic its values
        if ((flowVoltage.connected == true || flowVoltage.connectedWire == true) && flowVoltage.voltage > voltage)
        {
            voltage = flowVoltage.voltage;
            polarity = flowVoltage.polarity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if it enters the clip
        if (other.gameObject.tag == "redClip")
        {
            connected = true;
            voltProvider = other.gameObject;
            // give it access to voltage and polarity
            getVoltage = voltProvider.GetComponent<ConnectClip>();
            
        }
        // if it enters the clip
        if (other.gameObject.tag == "blackClip")
        {
            connected = true;
            voltProvider = other.gameObject;
            // give it access to voltage and polarity
            getVoltage = voltProvider.GetComponent<ConnectClip>();            
        }
        // if it connects to another wire
        if (other.gameObject.tag == "leftFreeWire" || other.gameObject.tag == "rightFreeWire")
        {
            connectedWire = true;
            voltProvider = other.gameObject;
            // give it access to voltage and polarity from the other wire
            getWireVoltage = voltProvider.GetComponent<WireReaction>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // if it leaves any of the sources of electricity, it loses its voltage
        if (other.gameObject.tag == "redClip")
        {
            connected = false;
            voltage = 0f;            
        }
        if (other.gameObject.tag == "blackClip")
        {
            connected = false;
            voltage = 0f;
        }

        if (other.gameObject.tag == "leftFreeWire" || other.gameObject.tag == "rightFreeWire")
        {
            connectedWire = false;
            voltage = 0f;
        }

    }
}
