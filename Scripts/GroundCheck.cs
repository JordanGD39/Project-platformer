using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject.GetComponentInParent<Player>();
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.isTrigger)
        {
            player.grounded = true;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (!col.isTrigger)
        {
            player.grounded = true;
        }
    }
    
    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.isTrigger)
        {
            player.grounded = false;
        }
    }
}
