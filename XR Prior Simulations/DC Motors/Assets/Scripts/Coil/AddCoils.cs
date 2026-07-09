using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Allows the addition of wires to the coil and records the number of wires already added
/// </summary>
public class AddCoils : MonoBehaviour
{
    public GameObject coil1;
    public GameObject coil2;
    public GameObject coil3;
    public GameObject leftWire;
    public GameObject rightWire;
    private bool oneActive;
    private bool twoActive;
    private bool threeActive;

    public int activeCoils;

    // Start is called before the first frame update
    void Start()
    {   
        activeCoils = 0;
        oneActive = false;
        twoActive = false;
        threeActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        // record the number of wires in the coil
        if (oneActive)
        {
            activeCoils = 1;
            if (twoActive)
            {
                activeCoils = 2;
                if (threeActive)
                {
                    activeCoils = 3;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if a wire is in range of the coil
        if (other.gameObject.tag == "bodyFreeWire")
        {
            // if no wire is in the coil
            if (!oneActive)
            {
                // make one visible
                coil1.SetActive(true);
                // play connection audio
                gameObject.GetComponent<AudioSource>().Play();
                // display the coil wires capable of connecting to the split-ring commutator
                leftWire.SetActive(true);
                rightWire.SetActive(true);
                // hide the original free wire
                other.gameObject.SetActive(false);
                oneActive = true;
            }
            // if one wire is in the coil
            else if (!twoActive)
            {
                // make the second visible
                coil2.SetActive(true);
                gameObject.GetComponent<AudioSource>().Play();
                other.gameObject.SetActive(false);
                twoActive = true;
            }
            // if two wires are in the coil
            else if (!threeActive)
            {
                // make the third visible
                coil3.SetActive(true);
                gameObject.GetComponent<AudioSource>().Play();
                other.gameObject.SetActive(false);
                threeActive = true;
            }
        }
    }
}
