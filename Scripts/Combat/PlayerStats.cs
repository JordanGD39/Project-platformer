using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public string name;

    public int maxHP;
    public int currHP;
    public int maxSP;
    public int currSP;
    public int maxATK;
    public int currATK;
    public float maxSPD;
    public float currSPD;
    public int maxDEF;
    public int currDEF;
    public int maxRES;
    public int currRES;
    public int maxLUK;
    public int currLUK;

    public int expToLevel;
    public int exp;

    public List<BaseAttack> attacks = new List<BaseAttack>();
}
