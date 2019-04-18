using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leak : BaseAttack
{
    public Leak()
    {
        name = "Leak";
        description = "A attack with a leak";
        attackType = Type.Physical;
        physicalDamage = 1;
        magicalDamage = 0;
        cost = 0;
        animState = 2;
    }
}
