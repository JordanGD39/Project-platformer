using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelector : MonoBehaviour
{
    public GameObject Enemy;
    private bool showSelector = false;

    public void SelectEnemy()
    {
        GameObject.FindGameObjectWithTag("BattleManager").GetComponent<BattleStateMachine>().InputTarget(Enemy);
    }

    public void HidePointer()
    {
         Enemy.transform.Find("pointer").gameObject.SetActive(false);
    }

    public void ShowPointer()
    {
        Enemy.transform.Find("pointer").gameObject.SetActive(true);
    }
}
