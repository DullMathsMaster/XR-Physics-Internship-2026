using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Controls the requirements for the progress percentage to increase
/// </summary>
public class ProgressBar : MonoBehaviour
{
    // use a shape as a progress bar 
    public Image bar;
    public float progress;
    public TextMesh percent;

    public GameObject battery;
    private bool batteryOn;

    public GameObject coil;
    private bool oneCoil;
    private bool twoCoil;
    private bool threeCoil;

    public GameObject redPlug;
    public GameObject blackPlug;
    private bool redPlugIn;
    private bool blackPlugIn;

    private bool leftPol;
    private bool rightPol;

    private bool leftMag;
    private bool rightMag;

    private bool currentCheck;
    private bool magCheck;
    private bool rotCheck;

    private bool speedCheck;

    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        // start the progress bar at 0
        rectTransform = bar.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(0f, rectTransform.localScale.y, rectTransform.localScale.z);

        // initialise the conditions as not being met
        batteryOn = false;

        oneCoil = false;
        twoCoil = false;
        threeCoil = false;

        redPlugIn = false;
        blackPlugIn = false;

        leftPol = false;
        rightPol = false;

        leftMag = false;
        rightMag = false;

        currentCheck = false;
        magCheck = false;
        rotCheck = false;

        speedCheck = false;

        percent.text = "0";
        progress = 0;
    }
   
    void Update()
    {   
        // while the progress is not 100%
        if (rectTransform.localScale.x < 1f)
        {            
            // progress increases by 5 when the battery is switched on
            if (battery.activeSelf == true && batteryOn == false)
            {
                rectTransform.localScale += new Vector3(0.05f, 0f, 0f);
                batteryOn = true;
            }

            // for each wire added to the coil, the progress increases by 5
            if (coil.GetComponent<AddCoils>().activeCoils == 1 && oneCoil == false)
            {
                rectTransform.localScale += new Vector3(0.05f, 0f, 0f);
                oneCoil = true;
            }
            if (coil.GetComponent<AddCoils>().activeCoils == 2 && twoCoil == false)
            {
                rectTransform.localScale += new Vector3(0.05f, 0f, 0f);
                twoCoil = true;
            }
            if (coil.GetComponent<AddCoils>().activeCoils == 3 && threeCoil == false)
            {
                rectTransform.localScale += new Vector3(0.05f, 0f, 0f);
                threeCoil = true;
            }

            // progress increases by 5 each for the insertion of the red and black plugs
            if (redPlug.GetComponent<BatteryConnect>().voltage > 0 && redPlugIn == false)
            {
                rectTransform.localScale += new Vector3(0.05f, 0f, 0f);
                redPlugIn = true;
            }
            if (blackPlug.GetComponent<BatteryConnect>().voltage > 0 && blackPlugIn == false)
            {
                rectTransform.localScale += new Vector3(0.05f, 0f, 0f);
                blackPlugIn = true;
            }

            // progress increases by 5 when the coil wires are correctly connected to electricity 
            if ((coil.GetComponent<ConditionsCheck>().leftPolarity == "positive" || coil.GetComponent<ConditionsCheck>().leftPolarity == "negative") && leftPol == false)
            {
                rectTransform.localScale += new Vector3(0.05f, 0f, 0f);
                leftPol = true;
            }
            if ((coil.GetComponent<ConditionsCheck>().rightPolarity == "positive" || coil.GetComponent<ConditionsCheck>().rightPolarity == "negative") && rightPol == false)
            {
                rectTransform.localScale += new Vector3(0.05f, 0f, 0f);
                rightPol = true;
            }

            // progress increases by 5 when the coil is around magnets
            if ((coil.GetComponent<ConditionsCheck>().leftMagnet == "North" || coil.GetComponent<ConditionsCheck>().leftMagnet == "South") && leftMag == false)
            {
                rectTransform.localScale += new Vector3(0.05f, 0f, 0f);
                leftMag = true;
            }
            if ((coil.GetComponent<ConditionsCheck>().rightMagnet == "North" || coil.GetComponent<ConditionsCheck>().rightMagnet == "South") && rightMag == false)
            {
                rectTransform.localScale += new Vector3(0.05f, 0f, 0f);
                rightMag = true;
            }

            // progress increases by 10 each when the electric and magnetic polarities are respected
            if ((coil.GetComponent<ConditionsCheck>().current == "up" || coil.GetComponent<ConditionsCheck>().current == "down") && currentCheck == false)
            {
                rectTransform.localScale += new Vector3(0.1f, 0f, 0f);
                currentCheck = true;
            }
            if ((coil.GetComponent<ConditionsCheck>().magneticField == "L to R" || coil.GetComponent<ConditionsCheck>().magneticField == "R to L") && magCheck == false)
            {
                rectTransform.localScale += new Vector3(0.1f, 0f, 0f);
                magCheck = true;
            }

            // progress icreases by 10 when the coil begins to rotate
            if ((coil.GetComponent<ConditionsCheck>().rotation == "clockwise" || coil.GetComponent<ConditionsCheck>().rotation == "anticlockwise") && rotCheck == false)
            {
                rectTransform.localScale += new Vector3(0.1f, 0f, 0f);
                rotCheck = true;
            }
            
            // progress increases by 20 when maximum rotation speed is reached
            if ((coil.GetComponent<Rotation>().rotationSpeed == -144 || coil.GetComponent<Rotation>().rotationSpeed == 144) && speedCheck == false)
            {
                rectTransform.localScale += new Vector3(0.2f, 0f, 0f);
                speedCheck = true;
            }
        }

        // display the progress bar
        progress = rectTransform.localScale.x * 100;
        // display the progress percentage
        progress = Mathf.RoundToInt(progress);
        percent.text = progress.ToString();
    }
    
}
