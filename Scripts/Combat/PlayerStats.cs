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

    public float hpChance;
    public float atkChance;
    public float spdChance;
    public float defChance;
    public float resChance;
    public float lukChance;

    public int expToLevel;
    public int exp;

    public List<BaseAttack> attacks = new List<BaseAttack>();
    public List<BaseAttack> specials = new List<BaseAttack>();
    public List<BaseAttributeUp> attUp = new List<BaseAttributeUp>();
    public List<BaseAttributeUp> newSkills = new List<BaseAttributeUp>();
}
