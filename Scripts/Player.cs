using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    public float maxSpeed = 3;
    public float speed = 50f;
    public float jumpForce = 300f;
    private float timer;

    public bool grounded;
    public bool canDoubleJump;
    public bool canTripleJump;
    public bool dead = false;
    public bool combat = false;
    public bool attacking = false;
    public bool n_atk = false;
    public bool r_atk = false;

    private Rigidbody2D rb;
    private Animator anim;

    public GameObject HealthScript;
    public AudioManager aManager;
    private GUIScript Health;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        Health = HealthScript.GetComponent<GUIScript>();
        timer = 0.0f;

        aManager.StopPlaying("BattleMusic");
        aManager.Play("LevelMusic");
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (!dead)
        {
            if (!attacking)
            {
                if (Health.health <= 0)
                {
                    Die();
                }

                if (Health.health >= 10)
                {
                    Health.health = 10;
                }

                if (transform.position.y <= -15)
                {
                    Health.health = 0;
                    Die();
                }

                anim.SetBool("Grounded", grounded);
                anim.SetBool("Combat", false);
                anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

                if (Input.GetAxis("Horizontal") < -0.1f)
                {
                    transform.localScale = new Vector3(-0.5f, 0.5f, 1);
                }

                if (Input.GetAxis("Horizontal") > 0.1f)
                {
                    transform.localScale = new Vector3(0.5f, 0.5f, 1);
                }
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
                if (grounded)
                {
                    if (Input.GetButtonDown("Attack"))
                    {
                        timer = 0;
                        attacking = true;
                        n_atk = true;
                        anim.SetTrigger("N_Attack");
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
                if (timer >= 0.2f)
                {
                    attacking = false;
                    n_atk = false;
                }
            }
        }
    }
    void FixedUpdate()
    {
        if (!dead)
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
    }
    public void Oof()
    {
        anim.SetTrigger("Oof");
    }

    void Die()
    {
        anim.SetBool("Dead", true);
        dead = true;
    }
}
