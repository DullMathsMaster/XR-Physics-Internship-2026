using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls switching on the battery
/// </summary>
public class PowerSwitch : MonoBehaviour
{
    //public GameObject hint;

    // time out to ensure the battery switch does not alternate states while being pressed
    private float cooldownTime = 3f;
    private float pressTime;

    private bool ButtonPressed;

    public GameObject powerlight;

    // Start is called before the first frame update
    private void Start()
    {
        ButtonPressed = false;
    }

    // Update is called once per frame
    private void Update()
    {
        // if the button is pressed and the cooldown time has passed
        if (ButtonPressed && Time.time > pressTime + cooldownTime)
        {
            // switch the light to green
            powerlight.SetActive(!powerlight.activeSelf);
            // play animation and audio
            gameObject.GetComponent<Animator>().Play("switch");
            gameObject.GetComponent<AudioSource>().Play();
            pressTime = Time.time;
            Debug.Log("ACTIVATED");
        }    
        
    }

    // Recognise the presence of the user's hand on the power switch
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hand")
        {
            ButtonPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Hand")
        {
            ButtonPressed = false;
        }
    }
}
