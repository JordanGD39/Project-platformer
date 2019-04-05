using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private bool attacking;

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
        if(player.r_atk)
        {
            timer += Time.deltaTime;
            if (timer >= 0.4f)
            {
                col.enabled = true;
            }
            if (timer >= 1)
            {
                timer = 0;
                col.enabled = false;
            }
        }

        if (player.n_atk)
        {
            timer += Time.deltaTime;
            if (timer >= 0.1f)
            {
                col.enabled = true;
            }
            if (timer >= 0.2f)
            {
                timer = 0;
                col.enabled = false;
            }
        }
    }
}
