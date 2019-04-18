using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : BaseAttack
{
    public Punch()
    {
        name = "Punch";
        description = "A normal punch";
        attackType = Type.Physical;
        physicalDamage = 2;
        magicalDamage = 0;
        cost = 0;
        animState = 2;
    }
}
