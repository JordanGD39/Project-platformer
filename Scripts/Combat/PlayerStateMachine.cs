using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateMachine : MonoBehaviour
{

    public PlayerStats player;
    public BattleStateMachine BSM;
    public GameM GM;

    public Animator anim;

    public GameObject EnemyToAttack;
    public GameObject pointer;
    public GameObject hpTextBar;
    public GameObject spTextBar;

    private Vector3 startPos;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState;

    public Image bar;
    public Text hpText;
    public Text spText;

    public float curCooldown = 0f;
    public float maxCooldown = 10f;
    public float animSpeed = 10f;
    public float attackDistance = 1.5f;
    public float timer = 0f;

    public bool actionStarted = false;
    public bool dead = false;
    public bool oof = false;

    public int animState = 0;

    // Start is called before the first frame update
    void Start()
    {
        pointer.SetActive(false);
        startPos = transform.position;
        anim.SetInteger("State", 0);//idle
        anim.SetBool("Combat", true);
        currentState = TurnState.PROCESSING;
        BSM = GameObject.FindGameObjectWithTag("BattleManager").GetComponent<BattleStateMachine>();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameM>();
        hpText = hpTextBar.GetComponent<Text>();
        spText = spTextBar.GetComponent<Text>();

        if (GM.firstTurn)
        {
            curCooldown = maxCooldown - 5 + (player.currSPD/10);
        } else
        {
            curCooldown = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        hpText.text = "HP: " + player.currHP + "/" + player.maxHP;
        spText.text = "SP: " + player.currSP + "/" + player.maxSP;
        switch (currentState)
        {
            case TurnState.PROCESSING:
                if (BSM.PlayerUpdateProgressBar)
                {
                    UpdateProgressBar();
                }
                break;
            case TurnState.ADDTOLIST:
                BSM.CharsToManage.Add(gameObject);
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
                BSM.EnemyUpdateProgressBar = false;
                BSM.PlayerUpdateProgressBar = false;
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

                    gameObject.tag = "DeadCharacter";
                    BSM.CharsInBattle.Remove(gameObject);
                    BSM.CharsToManage.Remove(gameObject);
                    pointer.SetActive(false);
                    BSM.AttackPanel.SetActive(false);
                    BSM.EnemySelectPanel.SetActive(false);
                    for (int i = 0; i < BSM.PerformList.Count; i++)
                    {
                        if (BSM.PerformList[i].AttakerGO == gameObject)
                        {
                            BSM.PerformList.Remove(BSM.PerformList[i]);
                        }
                    }
                    BSM.playerInput = BattleStateMachine.PlayerUI.ACTIVATE;
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
                anim.SetInteger("State", 0);
                oof = false;
            }
        }
    }
    void UpdateProgressBar()
    {
        maxCooldown = 7 - (player.currSPD / 10);
        curCooldown += Time.deltaTime;
        float calcCooldown = curCooldown / maxCooldown;
        bar.transform.localScale = new Vector3(Mathf.Clamp(calcCooldown, 0, 1), bar.transform.localScale.y, bar.transform.localScale.z);
        if (curCooldown >= maxCooldown)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;
        BSM.EnemyUpdateProgressBar = true;
        BSM.PlayerUpdateProgressBar = true;
        //go to hero
        Vector3 enemyPos = new Vector3(EnemyToAttack.transform.position.x - attackDistance, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z);
        transform.localScale = new Vector3(0.4f, 0.4f, 1);
        anim.SetInteger("State", 1);
        while (MoveTowardsEnemy(enemyPos))
        {
            yield return null;
        }

        anim.SetInteger("State", animState);//animation state
        yield return new WaitForSeconds(1f);
        //attack

        //go back to startpos
        Vector3 firstPos = startPos;
        transform.localScale = new Vector3(-0.4f, 0.4f, 1);
        anim.SetInteger("State", 1);//Walk
        while (MoveTowardsStart(firstPos))
        {
            yield return null;
        }

        //remove attack
        BSM.PerformList.RemoveAt(0);
        //reset BSM
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
        //end coroutine
        actionStarted = false;
        //reset enemy state
        anim.SetInteger("State", 0);
        transform.localScale = new Vector3(0.4f, 0.4f, 1);
        curCooldown = 0;
        currentState = TurnState.PROCESSING;
    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public void TakeDamage(int damage, int magicalDamage)
    {
        int calcDamage = damage - player.currDEF;
        int calcMdamage = magicalDamage - player.currRES;

        if (calcDamage < 0)
        {
            calcDamage = 0;
        }
        if (calcMdamage < 0)
        {
            calcMdamage = 0;
        }

        Debug.Log(calcDamage);

        anim.SetTrigger("Oof");
        anim.SetInteger("State", -1);
        timer = 0f;
        oof = true;
        player.currHP -= calcDamage + calcMdamage;
        if (player.currHP <= 0)
        {
            player.currHP = 0;
            currentState = TurnState.DEAD;
        }
        Debug.Log(player.currHP);
    }
}
