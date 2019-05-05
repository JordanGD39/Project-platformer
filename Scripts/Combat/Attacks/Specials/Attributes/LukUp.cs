using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LukUp : BaseAttributeUp
{
    public LukUp()
    {
        name = "Luck up!";
        description = "Gives 3 luck for 3 turns to all party members.";
        defUp = 0;
        lukUp = 3;
        spdUp = 0;
        cost = 5;
        animState = 3;
    }
}
