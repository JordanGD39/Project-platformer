using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundCheck : MonoBehaviour
{

    public Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D col)
    {
        enemy.grounded = true;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        enemy.grounded = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        enemy.grounded = false;
    }
}
