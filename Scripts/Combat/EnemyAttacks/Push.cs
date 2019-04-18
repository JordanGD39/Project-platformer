using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : BaseAttack
{
    // Start is called before the first frame update
    public Push()
    {
        name = "Push";
        description = "A normal push";
        attackType = Type.Physical;
        physicalDamage = 2;
        magicalDamage = 0;
        cost = 0;
        animState = 4;
    }
}
