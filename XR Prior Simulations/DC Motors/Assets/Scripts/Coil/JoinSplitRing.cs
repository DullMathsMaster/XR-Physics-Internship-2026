using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Ensures that the split-ring commutator rotates with the main coil after it is connected
/// </summary>
public class JoinSplitRing : MonoBehaviour
{    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // The split-ring commutator is only recognised by the coil when in range
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "mainCoil")
        {
            other.gameObject.GetComponent<Rotation>().splitRingCommutator = gameObject;               
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "mainCoil")
        {
            other.gameObject.GetComponent<Rotation>().splitRingCommutator = null;
        }
    }
}
