using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls what happens when the magnets get close to either side of the coil of wires
/// </summary>
public class MagnetismHandler : MonoBehaviour
{
    public string magnet;
    public bool inField;
    // Start is called before the first frame update
    void Start()
    {
        inField = false;
        magnet = "none";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // if this side of the coil is in range of a N or S magnet, change the 'magnet' value accordingly
        if (other.gameObject.tag == "North")
        {
            inField = true;
            magnet = "North";
        }
        if (other.gameObject.tag == "South")
        {
            inField = true;
            magnet = "South";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // if the magnet is moved away, remove the 'magnet' value
        if (other.gameObject.tag == "North" || other.gameObject.tag == "South")
        {
            inField = false;
            magnet = "none";
        }
    }
}
