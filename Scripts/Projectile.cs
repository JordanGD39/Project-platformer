using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject player;

    public float travelSpeed = 6f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        GetComponent<Rigidbody2D>().velocity = transform.right * travelSpeed;
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") && !collision.CompareTag("Enemy") && !collision.CompareTag("SleepBox") && !collision.CompareTag("GroundCheck"))
        {
            Debug.Log(collision);
            Destroy(gameObject);
        }

        if (collision.CompareTag("Player"))
        {
            if (player.GetComponent<Player>() != null)
            {
                if (!player.GetComponent<Player>().oof)
                {
                    player.GetComponent<Player>().healthScript.health -= 1;
                    player.GetComponent<Rigidbody2D>().gravityScale = 0;
                    player.GetComponent<Player>().Oof();
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameM>().mikuHP -= 10;
                }
            }
        }
    }
}
