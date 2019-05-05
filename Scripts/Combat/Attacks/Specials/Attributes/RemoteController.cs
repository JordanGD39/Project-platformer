using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteController : BaseAttack
{
    public RemoteController()
    {
        name = "Remote controller";
        description = "Throw a remote controller towards the enemy";
        attackType = Type.Physical;
        physicalDamage = 3;
        magicalDamage = 0;
        cost = 7;
        animState = 3;
    }
}
