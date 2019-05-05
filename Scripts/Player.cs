using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    public enum danceStonePower { NOTHING, HELI, STATUE, SHIELD, SLEEP};

    public danceStonePower currDancePower;

    public enum partyMember { RIN, LEN };

    public partyMember currPartyMember;

    public float maxSpeed = 3;
    public float speed = 50f;
    public float jumpForce = 300f;
    public float timer;
    public float statueTimer;
    public float distance;
    public float helipower = 11;

    public bool grounded;
    public bool canDoubleJump;
    public bool canTripleJump;
    public bool dead = false;
    public bool combat = false;
    public bool attacking = false;
    public bool n_atk = false;
    public bool r_atk = false;
    public bool oof = false;
    public bool statuePlaced = false;
    public bool heli = false;
    public bool shield = false;
    public bool sleep = false;
    public bool dance = false;
    public bool inDanceStone = false;
    public bool nearPartyMember = false;
    public bool danceStoneAnim = false;
    public bool finish = false;
    public bool stopFinish = false;

    private Rigidbody2D rb;
    private Animator anim;

    public EventSystem eventSystem;

    public GameObject health;
    public GameObject statuePrefab;
    public GameObject statueGameObject;
    public GameObject danceButtons;
    public GameObject dancePanel;
    public GameObject popup;
    public GameObject zPopup;
    public GameObject shieldPrefab;
    public GameObject spawnPoint;
    public GameObject musicNote;

    public AudioManager aManager;
    public GUIScript healthScript;
    public GameM GM;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        healthScript = health.GetComponent<GUIScript>();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameM>();
        timer = 0.0f;
        dance = false;
        dancePanel.SetActive(false);
        currDancePower = danceStonePower.NOTHING;

        transform.position = GM.startPos;

        DestroyNearestEnemy();

        aManager.StopPlaying("BattleMusic");
        aManager.Play("LevelMusic");
        GM.previousScene = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        statueTimer += Time.deltaTime;

        anim.SetBool("Dance", dance);

        if (!dance && !finish)
        {
            if (!dead && !oof)
            {
                if (!attacking)
                {
                    if (healthScript.health <= 0)
                    {
                        Die();
                    }

                    if (healthScript.health >= 10)
                    {
                        healthScript.health = 10;
                    }

                    if (transform.position.y <= -15)
                    {
                        healthScript.health = 0;
                        Die();
                    }

                    anim.SetBool("Grounded", grounded);
                    anim.SetBool("Combat", false);
                    anim.SetBool("Heli", heli);
                    anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

                    if (Input.GetAxis("Horizontal") < -0.1f)
                    {
                        transform.localScale = new Vector3(-0.5f, 0.5f, 1);
                    }

                    if (Input.GetAxis("Horizontal") > 0.1f)
                    {
                        transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    }

                    if (shield)
                    {
                        maxSpeed = maxSpeed / 2;
                        if (Input.GetButtonDown("Cancel"))
                        {
                            maxSpeed = 3;
                            Destroy(GameObject.FindGameObjectWithTag("Shield"));
                            shield = false;
                        }
                    }

                    if (sleep && grounded && !Input.GetButton("Run"))
                    {
                        if (Input.GetButtonDown("Attack"))
                        {
                            rb.velocity = new Vector2(0, 0);
                            anim.SetTrigger("Singing");
                            GameObject clone = Instantiate(musicNote, spawnPoint.transform.position, transform.rotation);
                            Destroy(clone, 6f);
                        }

                        if (Input.GetButtonDown("Dance"))
                        {
                            sleep = false;
                        }
                    }

                    if (!inDanceStone && !shield && !nearPartyMember)
                    {
                        if (Input.GetButtonDown("Jump"))
                        {

                            if (grounded)
                            {
                                aManager.Play("Jump sfx");
                                anim.SetTrigger("jump");
                                jumpForce = 300f;
                                rb.AddForce(Vector2.up * jumpForce);
                            }
                            else
                            {
                                if (canDoubleJump)
                                {
                                    aManager.Play("Jump sfx");
                                    anim.SetTrigger("jump");
                                    canDoubleJump = false;
                                    rb.velocity = new Vector2(rb.velocity.x, 0);
                                    rb.AddForce(Vector2.up * jumpForce);
                                    canTripleJump = true;
                                }

                                else if (canTripleJump)
                                {
                                    aManager.Play("Jump sfx");
                                    anim.SetTrigger("jump");
                                    jumpForce = 250f;
                                    canTripleJump = false;
                                    rb.velocity = new Vector2(rb.velocity.x, 0);
                                    rb.AddForce(Vector2.up * jumpForce);
                                }
                            }
                        }
                    }
                }
            }

            if (statuePlaced)
            {
                if (statueTimer > 30)
                {
                    Destroy(statueGameObject);
                    statuePlaced = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                Helicopter();
            }
            if (!heli && !shield)
            {
                if (Input.GetButton("Run"))
                {
                    if (grounded)
                    {
                        maxSpeed = 5;
                        speed = 100;
                    }
                    else
                    {
                        maxSpeed = 4;
                        speed = 70;
                    }
                    if (grounded)
                    {
                        if (Input.GetButtonDown("Attack"))
                        {
                            GM.startPos = transform.position;
                            timer = 0;
                            attacking = true;
                            r_atk = true;
                            anim.SetTrigger("R_Attack");
                        }
                    }
                }
                else
                {
                    maxSpeed = 3;
                    speed = 50;
                    if (grounded && !sleep)
                    {
                        if (Input.GetButtonDown("Attack"))
                        {
                            GM.startPos = transform.position;
                            timer = 0;
                            attacking = true;
                            n_atk = true;
                            anim.SetTrigger("N_Attack");
                        }
                    }
                }

                if (oof)
                {
                    if (timer >= 1)
                    {
                        rb.gravityScale = 1;
                        anim.SetBool("Oof", false);
                        if (healthScript.health <= 0)
                        {
                            anim.SetBool("Dead", true);
                        }
                        oof = false;
                    }
                }
            }

            if (r_atk)
            {
                if (transform.localScale.x == 0.5f)
                {
                    rb.velocity = Vector3.right * 200 * Time.deltaTime;
                }
                if (transform.localScale.x == -0.5f)
                {
                    rb.velocity = -Vector3.right * 200 * Time.deltaTime;
                }
                if (timer >= 1)
                {
                    attacking = false;
                    r_atk = false;
                }
            }
            if (n_atk)
            {
                rb.velocity = Vector3.zero;
                if (timer >= 0.25f)
                {
                    attacking = false;
                    n_atk = false;
                }
            }

            if (heli)
            {
                if (timer > 1.2f)
                {
                    heli = false;
                    AfterHeli();
                }
            }

            if (inDanceStone)
            {
                zPopup.GetComponent<SpriteRenderer>().enabled = true;
                if (Input.GetButtonDown("Submit"))
                {
                    DanceMenu danceMenu = danceButtons.GetComponent<DanceMenu>();

                    popup.GetComponent<Text>().text = "Learned new dance!";

                    switch (currDancePower)
                    {
                        case danceStonePower.NOTHING:
                            break;
                        case danceStonePower.HELI:
                            if (!GM.heliLearned)
                            {
                                GM.heliLearned = true;
                                GameObject clone = Instantiate(popup, transform.position, transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
                                Destroy(clone, 1f);
                                danceButtons.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);

                                rb.velocity = new Vector2(0, 0);
                                dance = true;
                                timer = 0;
                                danceStoneAnim = true;
                            }
                            break;
                        case danceStonePower.STATUE:
                            if (!GM.statueLearned)
                            {

                                GM.statueLearned = true;
                                GameObject clone2 = Instantiate(popup, transform.position, transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
                                danceButtons.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                                Destroy(clone2, 1f);

                                rb.velocity = new Vector2(0, 0);
                                dance = true;
                                timer = 0;
                                danceStoneAnim = true;
                            }
                            break;
                        case danceStonePower.SHIELD:
                            if (!GM.shieldLearned)
                            {
                                GM.shieldLearned = true;
                                GameObject clone3 = Instantiate(popup, transform.position, transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
                                danceButtons.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                                Destroy(clone3, 1f);

                                rb.velocity = new Vector2(0, 0);
                                dance = true;
                                timer = 0;
                                danceStoneAnim = true;
                            }
                            break;
                        case danceStonePower.SLEEP:
                            if (!GM.sleepLearned)
                            {
                                GM.sleepLearned = true;
                                GameObject clone4 = Instantiate(popup, transform.position, transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
                                Destroy(clone4, 1f);
                                danceButtons.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);

                                rb.velocity = new Vector2(0, 0);
                                dance = true;
                                timer = 0;
                                danceStoneAnim = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            if (!nearPartyMember && !inDanceStone)
            {
                zPopup.GetComponent<SpriteRenderer>().enabled = false;
            }

            if (nearPartyMember)
            {
                zPopup.GetComponent<SpriteRenderer>().enabled = true;
                if (Input.GetButtonDown("Submit"))
                {
                    popup.GetComponent<Text>().text = "Got new party member!";

                    switch (currPartyMember)
                    {
                        case partyMember.RIN:
                            if (GM.partyNumber != 2 && GM.partyNumber != 3)
                            {
                                GM.partyNumber = 2;
                                GameObject clone = Instantiate(popup, transform.position, transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
                                Destroy(clone, 1f);

                                rb.velocity = new Vector2(0, 0);
                            }
                            break;
                        case partyMember.LEN:
                            if (GM.partyNumber != 3)
                            {
                                GM.partyNumber = 3;
                                GameObject clone2 = Instantiate(popup, transform.position, transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
                                Destroy(clone2, 1f);

                                rb.velocity = new Vector2(0, 0);
                            }
                            break;
                    }
                }
            }
        }


        if (!dead && grounded && !oof && !finish)
        {
            if (dancePanel.activeSelf)
            {
                if (Input.GetButtonDown("Dance"))
                {
                    dancePanel.SetActive(false);

                    //aManager.Play("LevelMusic");
                    //aManager.StopPlaying("DanceMusic");

                    dance = false;
                }
            }
            else
            {
                if (Input.GetButtonDown("Dance"))
                {
                    //aManager.StopPlaying("LevelMusic");
                    //aManager.Play("DanceMusic");

                    danceButtons.transform.position = transform.position;
                    dancePanel.SetActive(true);
                    rb.velocity = new Vector2(0, rb.velocity.y);

                    dance = true;

                    eventSystem.SetSelectedGameObject(null);
                    eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
                }
            }

            if (danceStoneAnim)
            {
                if (timer >= 1.9f)
                {
                    dance = false;
                    danceStoneAnim = false;
                }
            }
        }

        if (dead)
        {
            if (timer >= 3)
            {
                SceneManager.LoadScene("GameOver");
            }
        }

        if (dance && oof)
        {
            dance = false;
            dancePanel.SetActive(false);
        }

        if (finish && grounded)
        {
            dance = true;

            if (!stopFinish)
            {
                rb.velocity = new Vector2(0,0);
                timer = 0;
                stopFinish = true;
            }

            if (timer >= 2)
            {
                SceneManager.LoadScene("LevelCleared");
            }
        }
    }

    void FixedUpdate()
    {
        if (!dance)
        {
            if (!dead && !oof)
            {
                if (!attacking)
                {
                    Vector3 easeVelocity = rb.velocity;
                    easeVelocity.y = rb.velocity.y;
                    easeVelocity.z = 0.0f;
                    easeVelocity.x *= 0.75f;


                    float h = Input.GetAxis("Horizontal");

                    //Easing X speed

                    if (grounded)
                    {
                        rb.velocity = easeVelocity;
                        canDoubleJump = true;
                    }

                    rb.AddForce((Vector2.right * speed) * h);

                    if (rb.velocity.x > maxSpeed)
                    {
                        rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
                    }
                    if (rb.velocity.x < -maxSpeed)
                    {
                        rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
                    }
                }
            }

            if (heli)
            {
                canDoubleJump = false;
                canTripleJump = false;
                maxSpeed = maxSpeed / 2;
                rb.AddForce(Vector2.up * helipower);
            }
        }
    }

    void DestroyNearestEnemy()
    {
        GameObject closestEnemy = null;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in allEnemies)
        {
            distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance < 248)
            {
                closestEnemy = enemy;
                Destroy(closestEnemy);
            }
        }
    }

    public void Oof()
    {
        GM.startPos = transform.position;
        oof = true;
        rb.velocity = new Vector2(0, 0);
        timer = 0;
        anim.SetBool("Oof", true);
    }

    public void setPosAfterBattle()
    {
        GM.previousScene = 0;
        transform.position = GM.startPos;
    }

    void Die()
    {
        anim.SetBool("Dead", true);
        timer = 0;
        rb.velocity = new Vector2(0, 0);
        dead = true;
    }

    public void Statue()
    {
        statueTimer = 0;
        if (!statuePlaced)
        {
            Instantiate(statuePrefab, new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), transform.rotation);
            statueGameObject = GameObject.FindGameObjectWithTag("Statue");
            statuePlaced = true;
        }
        else
        {
            statueGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
        }
    }

    public void Helicopter()
    {
        if (!heli)
        {
            heli = true;
            timer = 0;
        }
    }

    public void Shield()
    {
        if (!shield)
        {
            Instantiate(shieldPrefab, transform.position, transform.rotation, gameObject.transform);
            shield = true;
        }
    }

    public void Sleep()
    {
        if (!sleep)
        {
            sleep = true;
        }
    }

    void AfterHeli()
    {
        canDoubleJump = true;
        canTripleJump = true;
        maxSpeed = 3;
    }
}
