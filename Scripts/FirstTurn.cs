using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTurn : MonoBehaviour
{
    public GameObject FirstTurnUI;
    public GameObject player;
    public BoxCollider2D playerHitBox;

    public bool go = false;

    public float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerHitBox = player.GetComponent<BoxCollider2D>();
        timer = 0;
        playerHitBox.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(go)
        {
            Show();
        }
    }

    public void Show()
    {
        playerHitBox.enabled = false;
        FirstTurnUI.SetActive(true);
    }
}
