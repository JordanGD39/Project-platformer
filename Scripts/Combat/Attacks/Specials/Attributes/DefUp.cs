using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefUp : BaseAttributeUp
{
    public DefUp()
    {
        name = "Defense up!";
        description = "Gives 3 defense for 3 turns to all party members.";
        defUp = 3;
        lukUp = 0;
        spdUp = 0;
        cost = 5;
        animState = 3;
    }
}
