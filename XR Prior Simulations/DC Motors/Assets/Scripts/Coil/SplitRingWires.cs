using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls how the split-ring commutator's conducting wires receive voltage and polarity
/// </summary>
public class SplitRingWires : MonoBehaviour
{
    public string polarity;
    public float voltage;

    // assign the split-ring's conductors
    public GameObject conductor;

    private PolarityHandler getPolarity;

    void Start()
    {
        polarity = "none";
        voltage = 0f;
    }

    void Update()
    {
        // get polarity and voltage values from the conductors (which will be connected to free wires)
        getPolarity = conductor.GetComponent<PolarityHandler>();
        voltage = getPolarity.voltage;
        polarity = getPolarity.polarity;
    }
}
