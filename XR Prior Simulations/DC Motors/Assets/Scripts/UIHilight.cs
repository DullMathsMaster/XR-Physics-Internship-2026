using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHilight : MonoBehaviour
{
    public void Switch(bool isOn)
    {
        var image = GetComponent<Image>();
        
        if (isOn)
        {
            image.color = Color.black;
        }
        else
        {
            image.color = Color.white;
        }    
    }
}
