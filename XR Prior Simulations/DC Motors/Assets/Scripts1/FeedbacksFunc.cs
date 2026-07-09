using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbacksFunc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("isTeacher"))
        {
            if (PlayerPrefs.GetInt("isTeacher") == 1)
            {
                transform.GetComponent<Button>().interactable = true;
            }
        }
    }
}
