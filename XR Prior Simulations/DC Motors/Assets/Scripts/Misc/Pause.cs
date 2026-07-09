using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The Pause class controls what happens when the Pause button is clicked and the simulation is paused
/// </summary>
public class Pause : MonoBehaviour
{
    private bool paused;
    public GameObject pauseMenu;
    public GameObject cam;
    // code containing 'offset' is kept commented as the next programmer could use it to implement a pause menu that follows the user's gaze
    //public float offset;
    
    void Start()
    {
        paused = false;
        //offset = 0.8f;
    }

    // Update is called once per frame
    void Update()
    {
        if (paused)
        {
            //pauseMenu.transform.position = cam.transform.position + cam.transform.forward * offset;
            //pauseMenu.transform.rotation = new Quaternion(0.0f, cam.transform.rotation.y, 0.0f, cam.transform.rotation.w);
        }
        // If the Pause menu has been closed then the game should resume
        if (paused == true && pauseMenu.activeSelf == false)
        {            
            paused = false;
        }
        // This allows interactions to resume
        if (!paused) 
        {            
            Time.timeScale = 1f;
            
        }        

}

    private void OnTriggerEnter(Collider other)
    {
        // If the pause button is clicked, all interactions should cease and the pause menu should appear
        if (other.gameObject.tag == "Hand")
        {
            paused = true;
            pauseMenu.SetActive(true);            
            Time.timeScale = 0f;

        }
    }
}
