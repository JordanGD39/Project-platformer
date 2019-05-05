using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour
{
    public string name;
    public enum Type
    {
        Fire,
        Electric,
        Water,
        Physical
    }

    public Type attackType;

    public string description;
    public int physicalDamage;//atk - def
    public int magicalDamage;//atk - res
    public int cost;// currSP - cost
    public int animState;
    public float attackDistance;
}
