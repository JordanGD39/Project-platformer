using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_GameM : MonoBehaviour
{
    public GameM GM;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("GameManagerP").GetComponent<GameM>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
