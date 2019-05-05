using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMember : MonoBehaviour
{
    public enum members { RIN, LEN};

    public members membersGet;

    void Update()
    {
        switch (membersGet)
        {
            case members.RIN:
                if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameM>().partyNumber == 2)
                {
                    Destroy(gameObject, 2f);
                }
                break;
            case members.LEN:
                if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameM>().partyNumber == 3)
                {
                    Destroy(gameObject, 2f);
                }
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerScript = collision.GetComponent<Player>();

            playerScript.nearPartyMember = true;

            switch (membersGet)
            {
                case members.RIN:
                    playerScript.currPartyMember = Player.partyMember.RIN;
                    break;
                case members.LEN:
                    playerScript.currPartyMember = Player.partyMember.LEN;
                    break;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerScript = collision.GetComponent<Player>();

            playerScript.nearPartyMember = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player playerScript = collision.GetComponent<Player>();

            playerScript.nearPartyMember = false;
        }
    }
}
