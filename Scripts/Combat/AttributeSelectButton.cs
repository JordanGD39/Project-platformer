using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeSelectButton : MonoBehaviour
{
    public BaseAttributeUp specialToPerform;

    public void PerformSpecialAttack()
    {
        GameObject.FindGameObjectWithTag("BattleManager").GetComponent<BattleStateMachine>().InputAttribute(specialToPerform);
    }
}
