using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButtonUI : MonoBehaviour
{
    public void InfoButton()
    {
        GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag ("UIBox");
 
        foreach(GameObject go in gameObjectArray)
        {
            // Get the Canvas component
            Canvas canvas = go.GetComponent<Canvas>();

            // Check if the GameObject has a Canvas component
            if (canvas != null)
            {
                // Toggle the visibility of the Canvas
                canvas.enabled = !canvas.enabled;
            }
            else
            {
                Debug.LogWarning("Canvas not found on GameObject with tag 'UIBox'");
            }
        }
    }
}
