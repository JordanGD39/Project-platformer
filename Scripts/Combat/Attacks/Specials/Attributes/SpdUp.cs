using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpdUp : BaseAttributeUp
{
    public SpdUp()
    {
        name = "Speed up!";
        description = "Gives 3 speed for 3 turns to all party members.";
        defUp = 0;
        lukUp = 0;
        spdUp = 3;
        cost = 5;
        animState = 3;
    }
}
