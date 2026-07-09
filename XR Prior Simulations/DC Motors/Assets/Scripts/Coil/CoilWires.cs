using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls how the conducting wires of the coil receive voltage and polarity values
/// </summary>
public class CoilWires : MonoBehaviour
{
    public string polarity;
    public float voltage;
    public bool connected;
    private GameObject polarityProvider;
    private SplitRingWires getPolarity;

    public GameObject incompleteWarning;
    private bool incompleteSound;
    // Start is called before the first frame update
    void Start()
    {
        voltage = 0f;
        polarity = "none";
        connected = false;
        incompleteSound = false;
    }

    // Update is called once per frame
    void Update()
    {
        // get polarity and voltage values
        if (connected && getPolarity.voltage > 0f)
        {
            polarity = getPolarity.polarity;
            voltage = getPolarity.voltage;
        }
        else
        {
            polarity = "none";
            voltage = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if in contact with the split-ring's wires
        if (other.gameObject.tag == "leftRing" || other.gameObject.tag == "rightRing")
        {
            connected = true;
            // assign the connecting wire as the source of voltage and polarity
            polarityProvider = other.gameObject;
            getPolarity = polarityProvider.GetComponent<SplitRingWires>();
            other.gameObject.GetComponent<AudioSource>().Play();
        }
        // IMPORTANT. If the free wires are directly connected to the coil
        if (other.gameObject.tag == "leftFreeWire" || other.gameObject.tag == "rightFreeWire")
        {
            // warn that full rotation is not possible (hint at use of split-ring commutator)
            incompleteWarning.SetActive(true);
            if (!incompleteSound)
            {
                // play the warning sound each time the connection is established
                incompleteWarning.GetComponent<AudioSource>().Play();
                incompleteSound = true;
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // end the connection when the split-ring commutator is moved away
        if (other.gameObject.tag == "leftRing" || other.gameObject.tag == "rightRing")
        {
            connected = false;
        }

        // stop the warning when the wire is moved away
        if (other.gameObject.tag == "leftFreeWire" || other.gameObject.tag == "rightFreeWire")
        {
            incompleteWarning.SetActive(false);
        }
    }
}
