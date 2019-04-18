using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    public string name;

    public enum weakness
    {
        FIRE,
        ELECTRIC,
        WATER,
        PHYSICAL
    }

    public weakness EnemyWeakness;

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

    public List<BaseAttack> attacks = new List<BaseAttack>();
}
