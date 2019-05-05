using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialAttackButton : MonoBehaviour
{
    public BaseAttack specialSelected;
    public BaseAttack specialToPerform;

    public GameObject DescriptionPanel;

    void Start()
    {
        DescriptionPanel = GameObject.FindGameObjectWithTag("DescriptionPanel");
    }

    public void PerformSpecialAttack()
    {
        GameObject.FindGameObjectWithTag("BattleManager").GetComponent<BattleStateMachine>().InputSpecialAttack(specialToPerform);
    }

    public void ShowText()
    {
        //Text desText = DescriptionPanel.transform.GetChild(0).GetComponent<Text>();

        //desText.text = specialSelected.description;
    }
}
