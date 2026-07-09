using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;
using UnityEngine;

public class CameraHilight : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<HighlightingRenderer>().enabled = UIHilightManager.hilight;
    }
}