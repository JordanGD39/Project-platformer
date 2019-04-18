using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{

    public EnemyStats enemy;
    public BattleStateMachine BSM;
    public GameM GM;

    private Vector3 startPos;

    public GameObject PlayerToAttack;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currenState;

    public float curCooldown = 0f;
    public float maxCooldown = 10f;
    public float animSpeed = 10f;

    public int ridingState = 4;

    private bool actionStarted = false;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        currenState = TurnState.PROCESSING;
        BSM = GameObject.FindGameObjectWithTag("BattleManager").GetComponent<BattleStateMachine>();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameM>();
        startPos = transform.position;

        if (!GM.firstTurn)
        {
            curCooldown = maxCooldown - 1;
        }
        else
        {
            curCooldown = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currenState)
        {
            case TurnState.PROCESSING:
                if (BSM.EnemyUpdateProgressBar)
                {
                    UpdateProgressBar();
                }
                break;
            case TurnState.CHOOSEACTION:
                ChooseAction();
                currenState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
                break;
            case TurnState.ACTION:
                StartCoroutine(TimeForAction());
                break;
            case TurnState.DEAD:
                break;
            default:
                break;
        }
    }
    void UpdateProgressBar()
    {
        maxCooldown = 8 - (enemy.currSPD / 10);
        curCooldown += Time.deltaTime;
        if (curCooldown >= maxCooldown)
        {
            currenState = TurnState.CHOOSEACTION;
        }
    }

    void ChooseAction()
    {
        if (BSM.CharsInBattle.Count > 0)
        {
            HandleTurn theAttack = new HandleTurn();
            theAttack.attacker = enemy.name;
            theAttack.type = "Enemy";
            theAttack.AttakerGO = gameObject;
            theAttack.TargetGO = BSM.CharsInBattle[Random.Range(0, BSM.CharsInBattle.Count)];
            int num = Random.Range(0, enemy.attacks.Count);
            theAttack.chosenAttack = enemy.attacks[num];
            ridingState = theAttack.chosenAttack.animState;
            Debug.Log(gameObject.name + " did " + theAttack.chosenAttack.name + " and did " + theAttack.chosenAttack.physicalDamage + " damage and magical " + theAttack.chosenAttack.magicalDamage);
            BSM.CollectActions(theAttack);
        }
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;
        BSM.PlayerUpdateProgressBar = false;
        BSM.EnemyUpdateProgressBar = false;

        //go to hero
        Vector3 playerPos = new Vector3(PlayerToAttack.transform.position.x + 1.5f, PlayerToAttack.transform.position.y, PlayerToAttack.transform.position.z);
        anim.SetInteger("Riding", 1);
        while (MoveTowardsEnemy(playerPos))
        {
            yield return null;
        }

        DoDamage();
        anim.SetInteger("Riding", ridingState);
        yield return new WaitForSeconds(1f);
        //attack

        //go back to startpos
        Vector3 firstPos = startPos;
        anim.SetInteger("Riding", 1);
        while (MoveTowardsStart(firstPos))
        {
            yield return null;
        }

        //remove attack
        BSM.PerformList.RemoveAt(0);
        //reset BSM
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
        //end coroutine
        BSM.PlayerUpdateProgressBar = true;
        BSM.EnemyUpdateProgressBar = true;
        actionStarted = false;
        //reset enemy state
        anim.SetInteger("Riding", 0);
        curCooldown = 0;
        currenState = TurnState.PROCESSING;
    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    void DoDamage()
    {
        int calcDamageMagical = 0;
        int calcDamagePhysical = 0;
        if (BSM.PerformList[0].chosenAttack.physicalDamage > 0)
        {
            calcDamagePhysical = enemy.currATK + BSM.PerformList[0].chosenAttack.physicalDamage;
        }
        if (BSM.PerformList[0].chosenAttack.magicalDamage > 0)
        {
            Debug.Log("Going in here for no reason");
            calcDamageMagical = enemy.currATK + BSM.PerformList[0].chosenAttack.magicalDamage;
        }
        PlayerToAttack.GetComponent<PlayerStateMachine>().TakeDamage(calcDamagePhysical, calcDamageMagical);
    }
}
