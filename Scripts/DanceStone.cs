using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceStone : MonoBehaviour
{
    public enum dances { HELI, STATUE, SHIELD, SLEEP};

    public dances dancesGet;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerScript = collision.GetComponent<Player>();

            playerScript.inDanceStone = true;

            switch (dancesGet)
            {
                case dances.HELI:
                    playerScript.currDancePower = Player.danceStonePower.HELI;
                    break;
                case dances.STATUE:
                    playerScript.currDancePower = Player.danceStonePower.STATUE;
                    break;
                case dances.SHIELD:
                    playerScript.currDancePower = Player.danceStonePower.SHIELD;
                    break;
                case dances.SLEEP:
                    playerScript.currDancePower = Player.danceStonePower.SLEEP;
                    break;
                default:
                    break;
            }
        }   
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerScript = collision.GetComponent<Player>();

            playerScript.inDanceStone = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerScript = collision.GetComponent<Player>();

            playerScript.inDanceStone = false;
        }
    }
}
