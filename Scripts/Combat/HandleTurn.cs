using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public string attacker;
    public string type;
    public GameObject AttakerGO;
    public GameObject TargetGO;

    public BaseAttack chosenAttack;
    public BaseAttributeUp chosenAttribute;
}
