using UnityEngine;
using TMPro;

public class AgeSelector : MonoBehaviour
{
    public static int CurrentAge = 0;

    public void SetAge(int value)
    {
        CurrentAge = value;

        Debug.Log("Age selected: " + value);
    }
}