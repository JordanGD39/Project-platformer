using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sing : BaseAttributeUp
{
    public Sing()
    {
        name = "Sing";
        description = "The character sings for all party members with 10 HP";
        hpUp = 10;
        defUp = 0;
        spdUp = 0;
        lukUp = 0;
        cost = 10;
        animState = 4;
    }
}
