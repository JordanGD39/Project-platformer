using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public AudioManager AM;
    public GameM GM;
    public Animator MikuAnim;
    public Animator FadeAnim;

    public GameObject GameOverText;
    public GameObject RetryButton;
    public GameObject QuitButton;

    public bool noPressy = false;

    public float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        AM = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameM>();

        AM.Play("GameOver");
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (noPressy)
        {
            if (timer >= 2.5f)
            {
                MikuAnim.SetTrigger("Stand");
            }
            if (timer >= 25)
            {
                SceneManager.LoadScene(GM.previousScene);
            }
            if (timer >= 1)
            {
                if (Input.GetButtonDown("Submit"))
                {
                    SceneManager.LoadScene(GM.previousScene);
                }
            }
        }
    }

    // Update is called once per frame
    public void Retry()
    {
        if (!noPressy)
        {
            AM.StopPlaying("GameOver");
            AM.Play("StandUp");

            timer = 0;
            noPressy = true;
            GameOverText.SetActive(false);
            RetryButton.SetActive(false);
            QuitButton.SetActive(false);
            FadeAnim.SetTrigger("Fade");
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
