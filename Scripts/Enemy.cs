using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    public GameObject block;
    public GameM GM;
    public GameObject playerHitboxATK;
    public GameObject canvas;
    public GameObject spawnPoint;
    public GameObject electroBall;
    public Collider2D playerATK;
    public FirstTurn ftUI;
    public Animator anim;

    public BoxCollider2D playerHitBox;

    private Player playerScript;

    private Rigidbody2D rb;

    public float distance;
    public float speed = 70f;
    public float maxSpeed = 4f;
    public float jumpForce = 300f;
    public float dir = 1f;
    public float timer = 0;
    public float uTimer;

    public int level = 1;

    public bool grounded;
    public bool jumpReady = true;
    public bool slow = false;
    public bool ride = false;
    public bool shoot = true;
    public bool sleeping = false;

    public int enemyNumber = 0;

    public enum movementState
    {
        LEFT,
        RIGHT,
        FOLLOW,
        SLEEP
    };

    public movementState currentState;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerScript = player.GetComponent<Player>();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameM>();
        playerHitboxATK = GameObject.FindGameObjectWithTag("PlayerHitBox");
        playerATK = playerHitboxATK.GetComponent<BoxCollider2D>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        ftUI = canvas.GetComponent<FirstTurn>();
        playerHitBox = player.GetComponent<BoxCollider2D>();
    }

    private void Search()
    {
        if(currentState == movementState.FOLLOW)
        {
            if (transform.localScale.x == 0.5f)
            {
                currentState = movementState.RIGHT;
            }
            if (transform.localScale.x == -0.5f)
            {
                currentState = movementState.LEFT;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        uTimer += Time.fixedDeltaTime;
        if (slow)
        {
            if (uTimer > 2)
            {
                slow = false;
                SceneManager.LoadScene("CombatTest");
            }
        }


        RaycastHit2D hit = Physics2D.Raycast(block.transform.position, -Vector2.up, 10);
        distance = (transform.position - player.transform.position).magnitude;

        switch (currentState)
        {
            case movementState.LEFT:
                if (!sleeping)
                {
                    if (hit.collider != null)
                    {
                        anim.SetInteger("Riding", 1);
                        ride = true;
                        dir = -1f;
                    }
                    else if (hit.collider == null)
                    {
                        anim.SetInteger("Riding", 0);
                        ride = false;
                        transform.position += Vector3.right * 10 * Time.deltaTime;
                        currentState = movementState.RIGHT;
                    }
                    transform.localScale = new Vector3(-0.5f, 0.5f, 1);
                }
                break;
            case movementState.RIGHT:
                if (!sleeping)
                {
                    if (hit.collider != null)
                    {
                        anim.SetInteger("Riding", 1);
                        ride = true;
                        dir = 1f;
                    }
                    else if (hit.collider == null)
                    {
                        anim.SetInteger("Riding", 0);
                        ride = false;
                        transform.position += -Vector3.right * 10 * Time.deltaTime;
                        currentState = movementState.LEFT;
                    }
                    transform.localScale = new Vector3(0.5f, 0.5f, 1);
                }
                break;
            case movementState.FOLLOW:
                if (!sleeping)
                {
                    if (player.transform.position.x > transform.position.x)
                    {
                        transform.localScale = new Vector3(0.5f, 0.5f, 1);
                        if (hit.collider != null)
                        {
                            anim.SetInteger("Riding", 1);
                            ride = true;
                            dir = 1f;
                        }
                        if (hit.collider == null)
                        {
                            anim.SetInteger("Riding", 0);
                            rb.velocity = new Vector2(0, rb.velocity.y);
                            ride = false;
                        }
                    }
                    if (player.transform.position.x < transform.position.x)
                    {
                        if (hit.collider != null)
                        {
                            anim.SetInteger("Riding", 1);
                            ride = true;
                            dir = -1f;
                        }
                        transform.localScale = new Vector3(-0.5f, 0.5f, 1);
                        if (hit.collider == null)
                        {
                            anim.SetInteger("Riding", 0);
                            rb.velocity = new Vector2(0, rb.velocity.y);
                            ride = false;
                        }
                    }

                    if (player.transform.position.y > transform.position.y + 1)
                    {
                        if (grounded)
                        {
                            if (jumpReady)
                            {
                                anim.SetInteger("Riding", 3);
                                anim.SetTrigger("Jumping");
                                jumpReady = false;
                                rb.AddForce(Vector2.up * jumpForce);
                            }
                        }
                    }
                    else
                    {
                        if (grounded)
                        {
                            jumpReady = true;
                        }
                    }
                    if (shoot && !sleeping)
                    {
                        if (timer >= 5)
                        {
                            GameObject projectile = Instantiate(electroBall, spawnPoint.transform.position, spawnPoint.transform.rotation);
                            if (transform.localScale.x == 0.5f)
                            {
                                projectile.GetComponent<Projectile>().travelSpeed = 6;
                            }
                            if (transform.localScale.x == -0.5f)
                            {
                                projectile.GetComponent<Projectile>().travelSpeed = -6;
                            }
                            Destroy(projectile, 5f);
                            timer = 0;
                        }
                    }
                }
                break;
            case movementState.SLEEP:
                anim.SetInteger("Riding", 9);
                break;
        }

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "Ground")
            {
                if (distance < 12)
                {
                    currentState = movementState.FOLLOW; 
                } else
                {
                    Search();
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (ride)
        {
            rb.AddForce((Vector2.right * speed) * dir);
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !sleeping)
        {
            GM.firstTurn = false;
            uTimer = 0;
            playerHitBox.enabled = false;
            playerScript.Oof();
            player.GetComponent<Rigidbody2D>().gravityScale = 0;
            Slow();
        }
    }

    public void Damage()
    {
        GM.firstTurn = true;
        uTimer = 0;
        ftUI.go = true;
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
        Slow();
    }

    public void Slow()
    {
        GM.enemyNumber = enemyNumber;
        GM.enemyLevel = level;
        Debug.Log("Slomo");
        Time.timeScale = 0.1f;
        slow = true;
    }
}
