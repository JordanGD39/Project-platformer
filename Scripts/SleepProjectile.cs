using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepProjectile : MonoBehaviour
{

    public float travelSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Rigidbody2D>().velocity = new Vector2(1 * travelSpeed, 1f);
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") && !collision.CompareTag("Enemy") && !collision.CompareTag("GroundCheck"))
        {
            Debug.Log(collision);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().currentState = Enemy.movementState.SLEEP;
            collision.GetComponent<Enemy>().sleeping = true;
            BoxCollider2D box = collision.GetComponent<BoxCollider2D>();

            Destroy(box);

            collision.transform.GetChild(3).gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
            collision.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        }
    }
}
