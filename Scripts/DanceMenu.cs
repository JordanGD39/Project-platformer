using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DanceMenu : MonoBehaviour
{
    public GameObject player;
    public GameObject dancePanel;
    public bool pressed = false;

    public float timer = 0;

    public Player playerScript;
    public GameM GM;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameM>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
        dancePanel = GameObject.FindGameObjectWithTag("DancePanel");
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (pressed)
        {
            if (timer > 0.2f)
            {
                dancePanel.SetActive(false);
                playerScript.dance = false;
                pressed = false;
            }
        }

        if (GM.heliLearned)
        {
            transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }

        if (GM.statueLearned)
        {
            transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }

        if (GM.shieldLearned)
        {
            transform.GetChild(2).transform.GetChild(1).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }

        if (GM.sleepLearned)
        {
            transform.GetChild(3).transform.GetChild(1).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
    }

    // Update is called once per frame
    public void Up()
    {
        if (GM.heliLearned)
        {
            dancePanel.SetActive(false);
            playerScript.dance = false;
            playerScript.Helicopter();
        }
    }
    public void Down()
    {
        if (GM.statueLearned)
        {
            timer = 0;
            pressed = true;
            playerScript.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            playerScript.Statue();
        }
    }
    public void Left()
    {
        if (GM.shieldLearned)
        {
            timer = 0;
            pressed = true;
            playerScript.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            playerScript.Shield();
        }
    }
    public void Right()
    {
        if (GM.sleepLearned)
        {
            timer = 0;
            pressed = true;
            playerScript.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            playerScript.Sleep();
        }
    }
}
