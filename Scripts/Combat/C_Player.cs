using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Player : MonoBehaviour
{
    public AudioManager aManager;
    private Rigidbody2D rb;
    private Animator anim;

    public bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        aManager.StopPlaying("LevelMusic");
        aManager.Play("BattleMusic");
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Combat", true);
    }
}
