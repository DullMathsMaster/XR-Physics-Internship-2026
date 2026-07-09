using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHilightManager : MonoBehaviour
{
    private UIHilight[] components;
    public static bool hilight;
    
    void Start()
    {
        components = GetComponentsInChildren<UIHilight>(true);
        Switch(hilight);
    }
    
    public void Switch(bool value)
    {
        hilight = value;
        foreach (UIHilight component in components)
        {
            component.Switch(value);
        }
    }
}
