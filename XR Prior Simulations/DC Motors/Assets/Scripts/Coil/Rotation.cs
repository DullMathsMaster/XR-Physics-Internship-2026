using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls the factors influencing the speed of rotation (coil wires and voltage)
/// </summary>
public class Rotation : MonoBehaviour
{
    public string rotation;
    public float voltage;
    public int coilNumber;

    public GameObject wire;
    public GameObject voltProvider;
    public GameObject improveHint;
    public GameObject success;
    public GameObject splitRingCommutator;

    public float rotationSpeed;

    private bool successSound;
    // Start is called before the first frame update
    void Start()
    {
        // access the 'clockwise', 'anticlockwise' or 'none' rotation value
        rotation = gameObject.GetComponent<ConditionsCheck>().rotation;
        voltage = 0f;
        // get the number of wires in the coil
        coilNumber = gameObject.GetComponent<AddCoils>().activeCoils;
        rotationSpeed = 0f;

        successSound = false;
        splitRingCommutator = null;
    }

    // Update is called once per frame
    void Update()
    {
        // update the rotation direction and the number of wires in the coil
        rotation = gameObject.GetComponent<ConditionsCheck>().rotation;
        coilNumber = gameObject.GetComponent<AddCoils>().activeCoils;
        
        // when the coil has begun to rotate, the voltage value will be taken straight from the battery
        if (rotationSpeed != 0f)
        {
            // get the battery's voltage
            voltage = voltProvider.GetComponent<VoltSelect>().voltage;
        }
        // before the coil starts rotating, the voltage is taken from a wire connected to it
        else
        {
            if (wire.activeSelf == true)
            {
                voltage = wire.GetComponent<CoilWires>().voltage;
            }
            
        }
        // if the rotation has a direction
        if (rotation != "none")
        {
            // formula for speed of rotation
            rotationSpeed = 4 * coilNumber * voltage;
            // alter visual rotation direction based on 'rotation' value
            if (rotation == "clockwise")
            {
                rotationSpeed = -1 * rotationSpeed;
            }
            // rotate the coil
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            // rotate the split-ring commutator
            if (splitRingCommutator != null)
            {
                splitRingCommutator.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

            }
            // if the rotation speed is maximum in either direction, the simulation is complete
            if (rotationSpeed == -144 || rotationSpeed == 144)
            {
                // display success message
                success.SetActive(true);
                // remove any hints
                improveHint.SetActive(false);
                if (!successSound)
                {
                    // play success sound once
                    success.GetComponent<AudioSource>().Play();
                    successSound = true;
                }
                
            }
            // if the maximum hasn't been reached, keep the hint visible
            else
            {
                success.SetActive(false);
                improveHint.SetActive(true);
                improveHint.GetComponent<AudioSource>().Play();
            }
        }
        
    }
}
