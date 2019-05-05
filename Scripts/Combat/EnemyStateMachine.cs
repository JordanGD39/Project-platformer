using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{

    public EnemyStats enemy;
    public BattleStateMachine BSM;
    public GameM GM;

    public Vector3 startPos;

    public GameObject PlayerToAttack;
    public GameObject popupDamage;
    public GameObject electroBall;

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
    public float timer = 0;
    public float attackDistance = 1.3f;

    public int ridingState = 4;

    public bool oof = false;
    public bool dead = false;

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

        timer += Time.deltaTime;
        
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
                if (dead)
                {
                    return;
                }
                else
                {
                    anim.SetBool("Dead", true);

                    gameObject.tag = "DeadEnemy";
                    BSM.EnemiesInBattle.Remove(gameObject);
                    BSM.DeadEnemiesInBattle.Add(gameObject);
                    for (int i = 0; i < BSM.PerformList.Count; i++)
                    {
                        if (BSM.PerformList[i].AttakerGO == gameObject)
                        {
                            BSM.PerformList.Remove(BSM.PerformList[i]);
                        }
                    }
                    dead = true;
                }
                break;
            default:
                break;
        }

        if (oof)
        {
            if (timer >= 1f)
            {
                anim.SetInteger("Riding", 0);
                oof = false;
            }
        }
    }
    void UpdateProgressBar()
    {
        maxCooldown = 7 - (enemy.currSPD / 10);
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
            attackDistance = theAttack.chosenAttack.attackDistance;
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
        Vector3 playerPos = new Vector3(PlayerToAttack.transform.position.x + attackDistance, PlayerToAttack.transform.position.y, PlayerToAttack.transform.position.z);
        anim.SetInteger("Riding", 1);
        while (MoveTowardsEnemy(playerPos))
        {
            yield return null;
        }
        if (BSM.PerformList[0].chosenAttack.magicalDamage > 0)
        {
            GameObject clone = Instantiate(electroBall, transform.position, transform.rotation);
            clone.GetComponent<Projectile>().travelSpeed = -6f;
            Destroy(clone, 4f);

            yield return new WaitForSeconds(1.4f);
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
            calcDamageMagical = enemy.currATK + BSM.PerformList[0].chosenAttack.magicalDamage;
        }
        PlayerToAttack.GetComponent<PlayerStateMachine>().TakeDamage(calcDamagePhysical, calcDamageMagical);
    }

    public void TakeDamage(int damage, int magicalDamage)
    {
        float chance = Random.Range(0, 101);

        if (enemy.currLUK < chance)
        {
            int calcDamage = damage - enemy.currDEF;
            int calcMdamage = magicalDamage - enemy.currRES;

            if (calcDamage < 0)
            {
                calcDamage = 0;
            }
            if (calcMdamage < 0)
            {
                calcMdamage = 0;
            }

            Debug.Log(calcDamage);

            anim.SetInteger("Riding", -1);
            timer = 0f;
            oof = true;
            enemy.currHP -= calcDamage + calcMdamage;

            int damageFinal = calcDamage + calcMdamage;

            GameObject clone = Instantiate(popupDamage, new Vector3(transform.position.x,transform.position.y - 0.3f, transform.position.z), transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
            clone.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "" + damageFinal;

            Destroy(clone, 1f);
        }
        else if (enemy.currLUK > chance)
        {
            anim.SetInteger("Riding", -3);
            oof = true;

            GameObject clone = Instantiate(popupDamage, new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z), transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
            clone.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Miss";

            Destroy(clone, 1f);
        }
        if (enemy.currHP <= 0)
        {
            enemy.currHP = 0;
            currenState = TurnState.DEAD;
        }
        Debug.Log(enemy.currHP);
    }
}
