using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnFunc : MonoBehaviour
{
    public void LoadScene(int s)
    {
        SceneManager.LoadScene(s);
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
