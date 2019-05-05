using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectroBall : BaseAttack
{
    public ElectroBall()
    {
        name = "Electro ball";
        description = "Shoots a electric ball";
        attackType = Type.Electric;
        physicalDamage = 0;
        magicalDamage = 6;
        cost = 0;
        animState = 5;
    }
}
