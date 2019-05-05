using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private bool attacking = true;

    public Player player;
    public Collider2D col;

    private float timer = 0;

    // Start is called before the first frame update
    void Awake()
    {
        col.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.oof)
        {
            if (player.r_atk)
            {
                if (attacking)
                {
                    timer = 0;
                    attacking = false;
                }
                timer += Time.deltaTime;
                if (timer >= 0.55f)
                {   
                    col.enabled = true;
                }
                if (timer >= 1)
                {
                    col.enabled = false;
                    attacking = true;
                }
            }

            if (player.n_atk)
            {
                if (attacking)
                {
                    timer = 0;
                    attacking = false;
                }

                timer += Time.deltaTime;
                if (timer >= 0.18f)
                {
                    col.enabled = true;
                }
                if (timer >= 0.2f)
                {
                    col.enabled = false;
                    attacking = true;
                }
            }
        }
    }
}
