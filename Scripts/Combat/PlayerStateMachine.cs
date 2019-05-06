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
    public GameObject popupDamage;

    public Vector3 startPos;

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
    public float attackDistance = 1f;
    public float timer = 0f;

    public bool actionStarted = false;
    public bool dead = false;
    public bool oof = false;

    public int animState = 0;
    public int defending = 0;

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
        if (!BSM.lost)
        {
            hpText.text = "HP: " + player.currHP + "/" + player.maxHP;
            spText.text = "SP: " + player.currSP + "/" + player.maxSP;
        }
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
                if (defending >= 1)
                {
                    player.currDEF = player.maxDEF;
                    player.currRES = player.maxRES;
                    defending = 0;
                }
                break;
            case TurnState.ACTION:
                StartCoroutine(TimeForAction());
                break;
            case TurnState.DEAD:
                if (dead)
                {
                    if (player.currHP > 0)
                    {
                        currentState = TurnState.PROCESSING;
                        BSM.CharsInBattle.Add(gameObject);
                        anim.SetBool("Dead", false);
                        anim.SetInteger("State", 0);
                        dead = false;
                    }
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

        if (oof && dead)
        {
            anim.SetBool("Oof", false);
            anim.SetBool("Dead", true);
            oof = false;
        }

        if (oof)
        {
            if (timer >= 1f)
            {
                anim.SetBool("Oof", false);
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
        if (!BSM.attributeTime)
        {
            Vector3 enemyPos = new Vector3(EnemyToAttack.transform.position.x - attackDistance, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z);
            transform.localScale = new Vector3(0.4f, 0.4f, 1);
            anim.SetInteger("State", 1);
            while (MoveTowardsEnemy(enemyPos))
            {
                yield return null;
            }

            DoDamage();


            anim.SetInteger("State", animState);//animation state
            if (player.name == "Len")
            {
                Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>() as Rigidbody2D;

                rb.AddForce(Vector2.up * 200);

                yield return new WaitForSeconds(0.8f);

                Destroy(rb);
            }
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
        }

        //remove attack
        BSM.PerformList.RemoveAt(0);
        //reset BSM
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
        //end coroutine
        actionStarted = false;
        //reset player state
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

    void DoDamage()
    {
        int calcDamageMagical = 0;
        int calcDamagePhysical = 0;
        if (BSM.PerformList[0].chosenAttack.physicalDamage > 0)
        {
            calcDamagePhysical = player.currATK + BSM.PerformList[0].chosenAttack.physicalDamage;
        }
        if (BSM.PerformList[0].chosenAttack.magicalDamage > 0)
        {
            calcDamageMagical = player.currATK + BSM.PerformList[0].chosenAttack.magicalDamage;
        }
        EnemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calcDamagePhysical, calcDamageMagical);
    }

    public void TakeDamage(int damage, int magicalDamage)
    {

        float chance = Random.Range(0, 101);

        if (player.currLUK < chance)
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

            if ((calcDamage + calcMdamage) > 0)
            {
                anim.SetTrigger("Oof");
                anim.SetInteger("State", -1);
            }
            timer = 0f;
            oof = true;
            player.currHP -= calcDamage + calcMdamage;

            int damageFinal = calcDamage + calcMdamage;

            GameObject clone = Instantiate(popupDamage, new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z), transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
            clone.transform.GetChild(0).gameObject.GetComponent<Text>().text = "" + damageFinal;

            Destroy(clone, 1f);
        }
        else if (player.currLUK > chance)
        {
            anim.SetInteger("State", -3);
            timer = 0f;
            oof = true;

            GameObject clone = Instantiate(popupDamage, new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z), transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
            clone.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Miss";

            Destroy(clone, 1f);
        }
        if (player.currHP <= 0)
        {
            player.currHP = 0;
            currentState = TurnState.DEAD;
        }
        Debug.Log(player.currHP);
    }
}
