using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls how the large red and black banana plugs enter the battery and the effect this has on voltage and polarity
/// </summary>
public class BatteryConnect : MonoBehaviour
{
    // initialise the battery's red and black connection points
    public GameObject redSock;
    public GameObject blackSock;
    // initialise the source of voltage values
    public GameObject voltProvider;

    public GameObject grabHolder;
    public float voltage;

    // position of sockets
    public Vector3 redPos;
    public Vector3 blackPos;
    // offset value to be used if trying to make the plugs automatically slot into place
    public Vector3 positionOffset;

    private Vector3 ownPos;
    private bool entered;
    private VoltSelect getVoltage;

    private string ownTag;
    public string polarity;

    // Start is called before the first frame update
    void Start()
    {        
        redPos = redSock.transform.position;
        blackPos = blackSock.transform.position;
        ownPos = transform.position;
        entered = false;
        // get voltage values
        getVoltage = voltProvider.GetComponent<VoltSelect>();

        ownTag = gameObject.tag;
        polarity = "none";
    }

    // Update is called once per frame
    void Update()
    {
        redPos = redSock.transform.position;
        blackPos = blackSock.transform.position;
        if (entered)
        {
            // change voltage value
            voltage = getVoltage.voltage;
            // code below can be used if trying to make the plugs automatically slot into place

            //grabHolder.transform.position = ownPos;
            //grabHolder.GetComponent<Grabbable>().enabled = false;
            //grabHolder.GetComponent<HandGrabInteractable>().enabled = false;
        }
        if (!entered)
        {
            voltage = 0f;
        //    grabHolder.GetComponent<Grabbable>().enabled = true;
        //    grabHolder.GetComponent<HandGrabInteractable>().enabled = true;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(ownTag);
        
        // if the plug enters the red socket
        if (other.gameObject.tag == "redSocket")
        {
            entered = true;
            // give the position the plug could switch to 
            ownPos = redPos + positionOffset;
            // play connection audio
            gameObject.GetComponent<AudioSource>().Play();
            
            // change Physics properties of the wire
            foreach (GameObject obj in objectsWithTag)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null && rb.mass < 7f)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
            }
            // entering the red socket makes polarity positive
            polarity = "positive";
        }
        // similarly for the black socket
        if (other.gameObject.tag == "blackSocket")
        {
            entered = true;
            ownPos = blackPos + positionOffset;
            gameObject.GetComponent<AudioSource>().Play();

            foreach (GameObject obj in objectsWithTag)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null && rb.mass < 7f)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
            }
            // entering the black socket makes polarity negative
            polarity = "negative";
        }

    }

    private void OnTriggerExit(Collider other)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(ownTag);

        if (other.gameObject.tag == "redSocket")
        {
            entered = false;
            gameObject.GetComponent<AudioSource>().Play();

            foreach (GameObject obj in objectsWithTag)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null && rb.mass < 7f)
                {
                    //rb.isKinematic = false;
                    //rb.useGravity = true;
                }
            }

            polarity = "none";
        }
        if (other.gameObject.tag == "blackSocket")
        {
            entered = false;
            gameObject.GetComponent<AudioSource>().Play();

            foreach (GameObject obj in objectsWithTag)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null && rb.mass < 7f)
                {
                    //rb.isKinematic = false;
                    //rb.useGravity = true;
                }
            }

            polarity = "none";
        }
    }
}
