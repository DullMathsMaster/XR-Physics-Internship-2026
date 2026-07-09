using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rollVoltDial : MonoBehaviour
{
    public GameObject Dial;
    private bool leftRoll;
    private bool rightRoll; 
    // Start is called before the first frame update
    void Start()
    {
        leftRoll = false;
        rightRoll = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (leftRoll)
        {
            Dial.transform.Rotate(0f, 0f, 10 * Time.deltaTime, Space.Self);
        }
        if (rightRoll)
        {
            Dial.transform.Rotate(0f, 0f, -10 * Time.deltaTime, Space.Self);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "leftDial")
        {
            leftRoll = true;
        }
        if (other.gameObject.tag == "rightDial")
        {
            rightRoll = true;            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "leftDial")
        {
            leftRoll = false;
        }
        if (other.gameObject.tag == "rightDial")
        {
            rightRoll = false;
        }
    }
}
