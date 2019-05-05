using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject Door;

    void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.GetComponent<Animator>().SetBool("pressed", true);
        Door.GetComponent<Animator>().SetBool("open", true);
        Door.GetComponent<BoxCollider2D>().enabled = false;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        gameObject.GetComponent<Animator>().SetBool("pressed", true);
        Door.GetComponent<Animator>().SetBool("open", true);
        Door.GetComponent<BoxCollider2D>().enabled = false;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        gameObject.GetComponent<Animator>().SetBool("pressed", false);
        Door.GetComponent<Animator>().SetBool("open", false);
        Door.GetComponent<BoxCollider2D>().enabled = true;
    }
}
