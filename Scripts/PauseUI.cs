using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public static bool Paused = false;

    public GameObject PauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (Paused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }

    void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Paused = true;
    }

    public void Retry()
    {
        Application.LoadLevel(Application.loadedLevel);
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
