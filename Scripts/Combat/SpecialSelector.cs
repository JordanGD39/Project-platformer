using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSelector : MonoBehaviour
{
    public GameObject Special;
    private bool showSelector = false;

    public void SelectSpecial()
    {
        GameObject.FindGameObjectWithTag("BattleManager").GetComponent<BattleStateMachine>().InputTarget(Special);
    }
}
