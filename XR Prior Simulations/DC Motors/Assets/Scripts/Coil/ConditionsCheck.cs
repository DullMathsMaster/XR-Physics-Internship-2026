using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Very Important. Checks whether the learning objectives are met and decides the commencement and direcction of rotation
/// </summary>
public class ConditionsCheck : MonoBehaviour
{
    public GameObject left;
    public GameObject right;
    
    public string leftPolarity;
    public string rightPolarity;
    public string current;

    public string leftMagnet;
    public string rightMagnet;
    public string magneticField;

    public GameObject polarityWarning;
    public GameObject magnetWarning;    

    public string rotation;
    // Start is called before the first frame update
    void Start()
    {
        leftPolarity = "none";
        rightPolarity = "none";
        leftMagnet = "none";
        rightMagnet = "none";

        current = "none";
        magneticField = "none";
        
        rotation = "none";
    }

    // Update is called once per frame
    void Update()
    {   // get access to the electric and magnetic polarity values of both coil wires
        leftPolarity = left.GetComponent<CoilWires>().polarity;
        rightPolarity = right.GetComponent<CoilWires>().polarity;

        leftMagnet = left.GetComponent<MagnetismHandler>().magnet;
        rightMagnet = right.GetComponent<MagnetismHandler>().magnet;

        // warn the user if both wires have positive/negative electric polarity
        if (leftPolarity == "positive" && rightPolarity == "positive")
        {
            polarityWarning.SetActive(true);
            polarityWarning.GetComponent<AudioSource>().Play();
        }
        else if (rightPolarity == "negative" && leftPolarity == "negative")
        {
            polarityWarning.SetActive(true);
            polarityWarning.GetComponent<AudioSource>().Play();
        }
        
        else if ((leftPolarity == "positive" && rightPolarity == "negative") || (leftPolarity == "negative" && rightPolarity == "positive"))
        {
            // remove the polarity warning if electric polarity is respected
            polarityWarning.SetActive(false);
            // indicate the direction of current in the coil of wires according to Fleming's LHR
            if (leftPolarity == "positive" && rightPolarity == "negative")
            {
                current = "up";
            }
            else if (leftPolarity == "negative" && rightPolarity == "positive")
            {
                current = "down";
            }
            // if no magnets have been placed beside the coil
            if (leftMagnet == "none" || rightMagnet == "none")
            {
                // advice the user to include a magnet
                magnetWarning.SetActive(true);
                // read out the warning
                magnetWarning.GetComponent<AudioSource>().Play();
            }
            // if N-N or S-S magnet poles are on either side of the coil
            else if (leftMagnet == rightMagnet)
            {
                // don't warn about including a magnet
                magnetWarning.SetActive(false);
                // but warn about observing polarity
                polarityWarning.SetActive(true);
                // read out the warning
                polarityWarning.GetComponent<AudioSource>().Play();
            }
            // if the magnets are correctly placed
            else
            {
                // remove the warnings
                magnetWarning.SetActive(false);
                polarityWarning.SetActive(false);
                // indicate the direction of of the magnetic field according to Fleming's LHR
                if (leftMagnet == "North" && rightMagnet == "South")
                {
                    magneticField = "L to R";
                }
                else if (leftMagnet == "South" && rightMagnet == "North")
                {
                    magneticField = "R to L";
                }
            }
        }
        // use Fleming's LHR to determine the direction of the coil's rotation
        if (current != "none" && magneticField != "none")
        {
            if (magneticField == "L to R")
            {
                if (current == "down")
                {
                    rotation = "clockwise";
                }
                if (current == "up")
                {
                    rotation = "anticlockwise";
                }
            }
            if (magneticField == "R to L")
            {
                if (current == "down")
                {
                    rotation = "anticlockwise";
                }
                if (current == "up")
                {
                    rotation = "clockwise";
                }
            }
        }

    }
}
