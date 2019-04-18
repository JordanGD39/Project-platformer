using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleStateMachine : MonoBehaviour
{

    public enum PerformAction
    {
        WAIT,
        ACTION,
        PERFORM
    }

    public enum PlayerUI
    {
        ACTIVATE,
        WAITING,
        DONE
    }

    public PlayerUI playerInput;

    public List<GameObject> CharsToManage = new List<GameObject>();
    private HandleTurn PlayerChoice;

    public PerformAction battleStates;

    public List<HandleTurn> PerformList = new List<HandleTurn>();
    public List<GameObject> CharsInBattle = new List<GameObject>();
    public List<GameObject> EnemiesInBattle = new List<GameObject>();

    public GameObject enemyButton;
    public GameObject newButton;
    public GameObject atkButton;
    public GameObject AttackPanel;
    public GameObject EnemySelectPanel;

    public Transform spacer;
    public AudioManager aManager;
    public EventSystem eventSystem;

    public bool PlayerUpdateProgressBar = true;
    public bool EnemyUpdateProgressBar = true;

    // Start is called before the first frame update
    void Start()
    {
        battleStates = PerformAction.WAIT;
        aManager.StopPlaying("LevelMusic");
        aManager.Play("BattleMusic");
        EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        CharsInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        Time.timeScale = 1f;
        playerInput = PlayerUI.ACTIVATE;

        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);

        EnemyButtons();
    }

    // Update is called once per frame
    void Update()
    {
        switch (battleStates)
        {
            case PerformAction.WAIT:
                if (PerformList.Count > 0)
                {
                    battleStates = PerformAction.ACTION;
                }
                break;
            case PerformAction.ACTION:
                GameObject performer = GameObject.Find(PerformList[0].attacker);
                if (PerformList[0].type == "Enemy")
                {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    ESM.PlayerToAttack = PerformList[0].TargetGO;
                    ESM.currenState = EnemyStateMachine.TurnState.ACTION;
                }
                if (PerformList[0].type == "Player")
                {
                    PlayerStateMachine PSM = performer.GetComponent<PlayerStateMachine>();
                    PSM.EnemyToAttack = PerformList[0].TargetGO;
                    PSM.currentState = PlayerStateMachine.TurnState.ACTION;
                }
                battleStates = PerformAction.PERFORM;
                break;
            case PerformAction.PERFORM:

                break;
        }

        switch (playerInput)
        {
            case PlayerUI.ACTIVATE:
                if (CharsToManage.Count > 0)
                {
                    CharsToManage[0].transform.Find("pointer").gameObject.SetActive(true);
                    PlayerChoice = new HandleTurn();
                    bool highlight = true;
                    AttackPanel.SetActive(true);
                    playerInput = PlayerUI.WAITING;
                    eventSystem.SetSelectedGameObject(atkButton);
                }
                break;
            case PlayerUI.WAITING:
                break;
            case PlayerUI.DONE:
                PlayerInputDone();
                break;
            default:
                break;
        }
    }

    public void CollectActions(HandleTurn input)
    {
        PerformList.Add(input);
    }

    void EnemyButtons()
    {
        foreach (GameObject enemy in EnemiesInBattle)
        {
            newButton = Instantiate(enemyButton) as GameObject;
            EnemySelector button = newButton.GetComponent<EnemySelector>();

            EnemyStateMachine currEnemy = enemy.GetComponent<EnemyStateMachine>();

            Text buttonText = newButton.GetComponentInChildren<Text>();
            buttonText.text = currEnemy.enemy.name;

            button.Enemy = enemy;

            newButton.transform.SetParent(spacer, false);
        }
    }

    public void InputAttack()//attack button
    {
        PlayerChoice.attacker = CharsToManage[0].name;
        PlayerChoice.type = "Player";
        PlayerChoice.AttakerGO = CharsToManage[0];

        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(newButton);
    }

    public void InputTarget(GameObject choosenEnemy)//enemy select
    {
        PlayerChoice.TargetGO = choosenEnemy;
        playerInput = PlayerUI.DONE;
    }

    void PlayerInputDone()
    {
        PerformList.Add(PlayerChoice);
        EnemySelectPanel.SetActive(false);
        CharsToManage[0].transform.Find("pointer").gameObject.SetActive(false);
        CharsToManage.RemoveAt(0);
        playerInput = PlayerUI.ACTIVATE;
    }
}
