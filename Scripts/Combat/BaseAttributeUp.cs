using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttributeUp : MonoBehaviour
{
    public string name;

    public string description;

    public int hpUp;
    public int defUp;
    public int spdUp;
    public int lukUp;
    public int cost;// currSP - cost
    public int animState;
}