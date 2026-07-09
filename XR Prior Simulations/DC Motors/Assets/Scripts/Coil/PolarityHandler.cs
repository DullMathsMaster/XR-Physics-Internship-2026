using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Alows the split-ring commutator's conductors to receive voltage and polarity
/// </summary>
public class PolarityHandler : MonoBehaviour
{
    public string polarity;
    public float voltage;
    public bool connected;
    private GameObject polarityProvider;
    private WireReaction getPolarity;    

    void Start()
    {
        voltage = 0f;
        polarity = "none";
        connected = false; 
    }

    void Update()
    {
        // get the voltage and polarity of the connected wire
        if (connected && getPolarity.voltage > 0f)
        {
            polarity = getPolarity.polarity;
            voltage = getPolarity.voltage;
        }
        else
        {
            polarity = "none";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if any wires are connected to the conductor
        if (other.gameObject.tag == "leftFreeWire" || other.gameObject.tag == "rightFreeWire")
        {
            Debug.Log("Wire in contact with conductor " + gameObject.tag);
            connected = true;
            // assign the wire as the source of voltage and polarity values
            polarityProvider = other.gameObject;
            getPolarity = polarityProvider.GetComponent<WireReaction>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "leftFreeWire" || other.gameObject.tag == "rightFreeWire")
        {
            connected = false;            
        }
    }
}
