using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameM : MonoBehaviour
{
    public int previousScene = 0;

    public Vector3 startPos;

    public bool firstTurn = false;
    public int enemyNumber = 0;
    public int partyNumber = 0;
    public int playerLevel = 1;
    public int enemyLevel = 1;

    public int mikuMaxHP = 0;
    public int mikuHP = 0;
    public int mikuMaxSP = 0;
    public int mikuSP = 0;
    public float mikuSPD = 0;
    public int mikuATK = 0;
    public int mikuDEF = 0;
    public int mikuRES = 0;
    public int mikuLUK = 0;
    public int mikuEXP = 0;
    public int mikuEXPtoLevel = 0;

    public int rinMaxHP = 0;
    public int rinHP = 0;
    public int rinMaxSP = 0;
    public int rinSP = 0;
    public float rinSPD = 0;
    public int rinATK = 0;
    public int rinDEF = 0;
    public int rinRES = 0;
    public int rinLUK = 0;
    public int rinEXP = 0;
    public int rinEXPtoLevel = 0;

    public int lenMaxHP = 0;
    public int lenHP = 0;
    public int lenMaxSP = 0;
    public int lenSP = 0;
    public float lenSPD = 0;
    public int lenATK = 0;
    public int lenDEF = 0;
    public int lenRES = 0;
    public int lenLUK = 0;
    public int lenEXP = 0;
    public int lenEXPtoLevel = 0;

    public bool statueLearned = false;
    public bool heliLearned = false;
    public bool shieldLearned = false;
    public bool sleepLearned = false;

    public int hpPotions = 0;
    public int spPotions = 0;

    public static GameM instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
