using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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

    public GameM GM;

    private HandleTurn PlayerChoice;

    public PerformAction battleStates;

    public List<HandleTurn> PerformList = new List<HandleTurn>();
    public List<GameObject> CharsInBattle = new List<GameObject>();
    public List<GameObject> AllCharsInBattle = new List<GameObject>();
    public List<GameObject> DeadEnemiesInBattle = new List<GameObject>();
    public List<GameObject> EnemiesInBattle = new List<GameObject>();
    public List<GameObject> spButtons = new List<GameObject>();
    public List<GameObject> enemyButtons = new List<GameObject>();

    public GameObject enemyButton;
    public GameObject specialButton;
    public GameObject attButton;
    public GameObject newButton;
    public GameObject newSPButton;
    public GameObject newAttButton;
    public GameObject atkButton;
    public GameObject AttackPanel;
    public GameObject EnemySelectPanel;
    public GameObject SpecialPanel;
    public GameObject ItemPanel;
    public GameObject WinPanel;
    public GameObject LosePanel;
    public GameObject LevelUpPanel;
    public GameObject DescriptionPanel;
    public GameObject PlayerPanel;
    public GameObject GameManager;
    public GameObject popUp;

    public Transform spacer;
    public Transform specialSpacer;
    public AudioManager aManager;
    public EventSystem eventSystem;

    public bool PlayerUpdateProgressBar = true;
    public bool EnemyUpdateProgressBar = true;
    public bool attributeTime = false;
    public bool stopLeveling = false;
    public bool levelUp = false;
    public bool notThisCharacterLevelUp = false;
    public bool stopEarningEXP = false;
    public bool returning = false;
    public bool lost = false;
    public bool stopLosing = false;
    public bool stopShowing = false;
    private bool stopPlayingMusic = false;

    public int hpTurn = 9;
    public int defTurn = 9;
    public int spdTurn = 9;
    public int lukTurn = 9;
    public int levelCharacter = 0;

    public int hpPotions = 0;
    public int spPotions = 0;

    private int spPup = 0;

    public float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
        GM = GameManager.GetComponent<GameM>();
        GM.previousScene = 1;
        battleStates = PerformAction.WAIT;
        aManager.StopPlaying("LevelMusic");
        aManager.Play("BattleMusic");

        EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        CharsInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        AllCharsInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        Time.timeScale = 1f;
        playerInput = PlayerUI.ACTIVATE;

        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);
        SpecialPanel.SetActive(false);
        ItemPanel.SetActive(false);

        SetupBattle();
        SetStats();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        ItemPanel.transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Text>().text = "x" + hpPotions;
        ItemPanel.transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<Text>().text = "x" + spPotions;
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
                break;
            case PerformAction.PERFORM:

                break;
        }

        switch (playerInput)
        {
            case PlayerUI.ACTIVATE:
                if (CharsToManage.Count > 0)
                {
                    PlayerStateMachine PSM = CharsToManage[0].GetComponent<PlayerStateMachine>();

                    CharsToManage[0].transform.Find("pointer").gameObject.SetActive(true);
                    PlayerChoice = new HandleTurn();
                    attributeTime = false;
                    hpTurn += 1;
                    defTurn += 1;
                    spdTurn += 1;
                    lukTurn += 1;
                    PSM.defending += 1;
                    AttackPanel.SetActive(true);
                    playerInput = PlayerUI.WAITING;
                    eventSystem.SetSelectedGameObject(atkButton);

                    if (spdTurn >= CharsInBattle.Count * CharsInBattle.Count)
                    {
                        if (PSM.player.currSPD > PSM.player.maxSPD)
                        {
                            PSM.player.currSPD = PSM.player.maxSPD;
                        }
                    }

                    if (defTurn >= CharsInBattle.Count * CharsInBattle.Count)
                    {
                        if (PSM.player.currDEF > PSM.player.maxDEF)
                        {
                            PSM.player.currDEF = PSM.player.maxDEF;
                        }
                    }

                    if (lukTurn >= CharsInBattle.Count * CharsInBattle.Count)
                    {
                        if (PSM.player.currLUK > PSM.player.maxLUK)
                        {
                            PSM.player.currLUK = PSM.player.maxLUK;
                        }
                    }
                }
                if (!stopLeveling)
                {
                    if (EnemiesInBattle.Count <= 0)
                    {
                        stopLeveling = false;
                        PlayerUpdateProgressBar = false;
                        WinPanel.SetActive(true);
                        levelCharacter = 0;
                        ExpEarn();
                        stopLeveling = true;
                    }
                }

                if (CharsInBattle.Count <= 0)
                {
                    stopLeveling = false;
                    LosePanel.SetActive(true);
                    PlayerUpdateProgressBar = false;
                    Lost();
                    stopLeveling = true;
                }

                break;
            case PlayerUI.WAITING:
                if (EnemiesInBattle.Count <= 0)
                {
                    playerInput = PlayerUI.DONE;
                }
                break;
            case PlayerUI.DONE:
                PlayerInputDone();
                break;
            default:
                break;
        }

        if (EnemySelectPanel.activeSelf)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                EnemySelectPanel.SetActive(false);
                AttackPanel.SetActive(true);
                foreach (GameObject enemyButton in enemyButtons)
                {
                    Destroy(enemyButton);
                }
                foreach (GameObject specialButton in spButtons)
                {
                    Destroy(specialButton);
                }
                foreach (GameObject attButton in spButtons)
                {
                    Destroy(attButton);
                }

                switch (EnemiesInBattle.Count)
                {
                    case 1:
                        EnemiesInBattle[0].transform.Find("pointer").gameObject.SetActive(false);
                        eventSystem.SetSelectedGameObject(atkButton);
                        break;
                    case 2:
                        EnemiesInBattle[0].transform.Find("pointer").gameObject.SetActive(false);
                        EnemiesInBattle[1].transform.Find("pointer").gameObject.SetActive(false);
                        eventSystem.SetSelectedGameObject(atkButton);
                        break;
                    case 3:
                        EnemiesInBattle[0].transform.Find("pointer").gameObject.SetActive(false);
                        EnemiesInBattle[1].transform.Find("pointer").gameObject.SetActive(false);
                        EnemiesInBattle[2].transform.Find("pointer").gameObject.SetActive(false);
                        eventSystem.SetSelectedGameObject(atkButton);
                        break;
                }
            }
        }

        if (ItemPanel.activeSelf)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                ItemPanel.SetActive(false);
                AttackPanel.SetActive(true);
                eventSystem.SetSelectedGameObject(atkButton);
            }
        }

        if (levelUp)
        {
            if (timer >= 3)
            {
                PlayWinMusic();
                ExpEarn();
                LevelUp();
            }
        }
        else
        {
            if (returning)
            {
                if (timer >= 1)
                {
                    if (!stopShowing)
                    {
                        GameObject clone2 = Instantiate(popUp, popUp.transform.position, popUp.transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
                        clone2.GetComponent<Text>().text = "+" + spPup + " SP Potions";

                        Destroy(clone2, 1f);

                        stopShowing = true;
                    }
                }
                if (timer >= 3)
                {
                    SceneManager.LoadScene("Test");
                }
            }
        }

        if (lost)
        {
            if (timer >= 8)
            {
                SceneManager.LoadScene("GameOver");
            }
        }

        if (SpecialPanel.activeSelf)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                SpecialPanel.SetActive(false);
                DescriptionPanel.SetActive(false);
                AttackPanel.SetActive(true);

                foreach (GameObject specialButton in spButtons)
                {
                    Destroy(specialButton);
                }

                foreach (GameObject attButton in spButtons)
                {
                    Destroy(attButton);
                }

                eventSystem.SetSelectedGameObject(atkButton);
            }
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

            enemyButtons.Add(newButton);
            newButton.transform.SetParent(spacer, false);
        }
    }

    public void InputAttack()//attack button
    {
        PlayerChoice.attacker = CharsToManage[0].name;
        PlayerChoice.type = "Player";
        PlayerChoice.AttakerGO = CharsToManage[0];
        PlayerChoice.chosenAttack = CharsToManage[0].GetComponent<PlayerStateMachine>().player.attacks[0];
        CharsToManage[0].GetComponent<PlayerStateMachine>().animState = PlayerChoice.chosenAttack.animState;

        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
        EnemyButtons();
        eventSystem.SetSelectedGameObject(EnemySelectPanel.transform.GetChild(0).transform.GetChild(0).gameObject);
    }

    public void InputSpecial()//special button
    {
        if (CharsToManage[0].GetComponent<PlayerStateMachine>().player.specials.Count > 0 || CharsToManage[0].GetComponent<PlayerStateMachine>().player.attUp.Count > 0)
        {
            AttackPanel.SetActive(false);
            SpecialPanel.SetActive(true);

            foreach (BaseAttack specialAtk in CharsToManage[0].GetComponent<PlayerStateMachine>().player.specials)
            {
                newSPButton = Instantiate(specialButton) as GameObject;

                Text buttonText = newSPButton.GetComponentInChildren<Text>();

                buttonText.text = specialAtk.name;

                SpecialAttackButton SAB = newSPButton.GetComponent<SpecialAttackButton>();
                SAB.specialToPerform = specialAtk;

                spButtons.Add(newSPButton);
                newSPButton.transform.SetParent(specialSpacer, false);
            }
            foreach (BaseAttributeUp attUp in CharsToManage[0].GetComponent<PlayerStateMachine>().player.attUp)
            {
                newAttButton = Instantiate(attButton) as GameObject;

                Text buttonText = newAttButton.GetComponentInChildren<Text>();

                buttonText.text = attUp.name;

                newAttButton.transform.SetParent(specialSpacer, false);

                AttributeSelectButton ASB = newAttButton.GetComponent<AttributeSelectButton>();
                ASB.specialToPerform = attUp;

                spButtons.Add(newAttButton);
                eventSystem.SetSelectedGameObject(SpecialPanel.transform.GetChild(0).transform.GetChild(0).gameObject);
            }
        }
    }

    public void InputSpecialAttack(BaseAttack chosenSpecial)//Special attack button
    {
        if (CharsToManage[0].GetComponent<PlayerStateMachine>().player.currSP >= chosenSpecial.cost)
        {
            PlayerChoice.attacker = CharsToManage[0].name;
            PlayerChoice.type = "Player";
            PlayerChoice.AttakerGO = CharsToManage[0];
            PlayerChoice.chosenAttack = chosenSpecial;
            CharsToManage[0].GetComponent<PlayerStateMachine>().animState = PlayerChoice.chosenAttack.animState;

            SpecialPanel.SetActive(false);
            DescriptionPanel.SetActive(false);
            EnemySelectPanel.SetActive(true);
            EnemyButtons();
            eventSystem.SetSelectedGameObject(EnemySelectPanel.transform.GetChild(0).transform.GetChild(0).gameObject);
        }
    }

    public void InputAttribute(BaseAttributeUp chosenAttribute)//Atributte button
    {
        if (CharsToManage[0].GetComponent<PlayerStateMachine>().player.currSP >= chosenAttribute.cost)
        {
            PlayerChoice.attacker = CharsToManage[0].name;
            PlayerChoice.type = "Player";
            PlayerChoice.AttakerGO = CharsToManage[0];
            PlayerChoice.chosenAttribute = chosenAttribute;

            for (int i = 0; i < CharsInBattle.Count; i++)
            {
                PlayerStateMachine playerStat = CharsInBattle[i].GetComponent<PlayerStateMachine>();

                if (chosenAttribute.hpUp >= 1)
                {
                    if (hpTurn >= CharsInBattle.Count * CharsInBattle.Count)
                    {
                        playerStat.player.currHP += chosenAttribute.hpUp;

                        if (playerStat.player.currHP > playerStat.player.maxHP)
                        {
                            playerStat.player.currHP = playerStat.player.maxHP;
                        }

                        if (i == CharsInBattle.Count - 1)
                        {
                            hpTurn = 0;
                        }
                    }
                }
                if (chosenAttribute.spdUp >= 1)
                {
                    if (spdTurn >= CharsInBattle.Count * CharsInBattle.Count)
                    {
                        playerStat.player.currSPD += chosenAttribute.spdUp;
                        if (i == CharsInBattle.Count - 1)
                        {
                            spdTurn = 0;
                        }
                    }
                }
                if (chosenAttribute.defUp >= 1)
                {
                    if (defTurn >= CharsInBattle.Count * CharsInBattle.Count)
                    {
                        playerStat.player.currDEF += chosenAttribute.defUp;
                        if (i == CharsInBattle.Count - 1)
                        {
                            defTurn = 0;
                        }
                    }
                }
                if (chosenAttribute.lukUp >= 1)
                {
                    if (lukTurn >= CharsInBattle.Count * CharsInBattle.Count)
                    {
                        playerStat.player.currLUK += chosenAttribute.lukUp;
                        if (i == CharsInBattle.Count - 1)
                        {
                            lukTurn = 0;
                        }
                    }
                }
            }

            attributeTime = true;
            playerInput = PlayerUI.DONE;
        }
    }

    public void InputItem()
    {
        AttackPanel.SetActive(false);
        ItemPanel.SetActive(true);

        eventSystem.SetSelectedGameObject(ItemPanel.transform.GetChild(0).gameObject);
    }

    public void HpPotion()
    {
        if (hpPotions > 0)
        {
            PlayerChoice.attacker = CharsToManage[0].name;
            PlayerChoice.type = "Player";
            PlayerChoice.AttakerGO = CharsToManage[0];
            hpPotions -= 1;

            for (int i = 0; i < AllCharsInBattle.Count; i++)
            {
                PlayerStateMachine playerStat = CharsInBattle[i].GetComponent<PlayerStateMachine>();

                playerStat.player.currHP += 20;

                if (playerStat.player.currHP > playerStat.player.maxHP)
                {
                    playerStat.player.currHP = playerStat.player.maxHP;
                }
            }

            attributeTime = true;
            playerInput = PlayerUI.DONE;
        }
    }

    public void SpPotion()
    {
        if (spPotions > 0)
        {
            PlayerChoice.attacker = CharsToManage[0].name;
            PlayerChoice.type = "Player";
            PlayerChoice.AttakerGO = CharsToManage[0];
            spPotions -= 1;

            for (int i = 0; i < AllCharsInBattle.Count; i++)
            {
                PlayerStateMachine playerStat = CharsInBattle[i].GetComponent<PlayerStateMachine>();

                playerStat.player.currSP += 10;

                if (playerStat.player.currSP > playerStat.player.maxSP)
                {
                    playerStat.player.currSP = playerStat.player.maxSP;
                }
            }

            attributeTime = true;
            playerInput = PlayerUI.DONE;
        }
    }

    public void InputDefend()
    {
        PlayerStateMachine playerManage = CharsToManage[0].GetComponent<PlayerStateMachine>();

        PlayerChoice.attacker = CharsToManage[0].name;
        PlayerChoice.type = "Player";
        PlayerChoice.AttakerGO = CharsToManage[0];

        playerManage.animState = -2;

        playerManage.player.currDEF += 5;
        playerManage.player.currRES += 5;
        attributeTime = true;
        playerManage.defending = 0;
        playerInput = PlayerUI.DONE;
    }

    public void InputTarget(GameObject chosenEnemy)//enemy select
    {
        PlayerChoice.TargetGO = chosenEnemy;
        playerInput = PlayerUI.DONE;
    }

    void PlayerInputDone()
    {
        if (PlayerChoice.chosenAttack == null && PlayerChoice.chosenAttribute != null)
        {
            CharsToManage[0].GetComponent<PlayerStateMachine>().player.currSP -= PlayerChoice.chosenAttribute.cost;
        }
        if (PlayerChoice.chosenAttribute == null && PlayerChoice.chosenAttack != null)
        {
            CharsToManage[0].GetComponent<PlayerStateMachine>().player.currSP -= PlayerChoice.chosenAttack.cost;
        }

        PerformList.Add(PlayerChoice);
        EnemySelectPanel.SetActive(false);
        SpecialPanel.SetActive(false);
        AttackPanel.SetActive(false);
        DescriptionPanel.SetActive(false);
        ItemPanel.SetActive(false);

        foreach (GameObject specialButton in spButtons)
        {
            Destroy(specialButton);
        }
        foreach (GameObject attButton in spButtons)
        {
            Destroy(attButton);
        }
        foreach (GameObject enemyButton in enemyButtons)
        {
            Destroy(enemyButton);
        }
        CharsToManage[0].transform.Find("pointer").gameObject.SetActive(false);
        CharsToManage.RemoveAt(0);
        playerInput = PlayerUI.ACTIVATE;
    }

    public void ExpEarn()
    {
        if (!stopEarningEXP)
        {
            if (levelCharacter < CharsInBattle.Count)
            {
                Debug.Log(levelCharacter);

                PlayerStateMachine playerStat = CharsInBattle[levelCharacter].GetComponent<PlayerStateMachine>();

                playerStat.player.exp += 100 * (DeadEnemiesInBattle.Count - 1 + GM.enemyLevel);
                Debug.Log(playerStat.player.name);

                for (int i = 0; i < CharsInBattle.Count; i++)
                {
                    CharsInBattle[i].GetComponent<PlayerStateMachine>().anim.SetInteger("State", 4);
                }

                if (playerStat.player.exp >= playerStat.player.expToLevel)
                {
                    playerStat.player.exp = 0;
                    playerStat.player.expToLevel += 100;
                    levelUp = true;
                    notThisCharacterLevelUp = false;
                    timer = 0;
                }
                else if (playerStat.player.exp < playerStat.player.expToLevel)
                {
                    notThisCharacterLevelUp = true;
                    timer = 2;
                    levelUp = true;
                }
                stopEarningEXP = true;
            }
        }
    }

    void LevelUp()
    {
        string hp = "";
        string atk = "";
        string spd = "";
        string def = "";
        string res = "";
        string luk = "";
        string sp = "";
        
        if (!notThisCharacterLevelUp)
        {
            PlayerStateMachine playerStat = CharsInBattle[levelCharacter].GetComponent<PlayerStateMachine>();

            float chance = Random.Range(0, 101);
            Debug.Log(chance);

            if (playerStat.player.hpChance >= chance)
            {
                playerStat.player.maxHP += 1;
                hp = " + 1";
                Debug.Log(playerStat.player.name + "'s HP went up!");
            }

            chance = Random.Range(0, 101);

            if (playerStat.player.atkChance >= chance)
            {
                playerStat.player.maxATK += 1;
                atk = " + 1";
                Debug.Log(playerStat.player.name + "'s ATK went up!");
            }
            chance = Random.Range(0, 101);

            if (playerStat.player.spdChance >= chance)
            {
                playerStat.player.maxSPD += 1;
                spd = " + 1";
                Debug.Log(playerStat.player.name + "'s SPD went up!");
            }
            chance = Random.Range(0, 101);

            if (playerStat.player.defChance >= chance)
            {
                playerStat.player.maxDEF += 1;
                def = " + 1";
                Debug.Log(playerStat.player.name + "'s DEF went up!");
            }
            chance = Random.Range(0, 101);

            if (playerStat.player.resChance >= chance)
            {
                playerStat.player.maxRES += 1;
                res = " + 1";
                Debug.Log(playerStat.player.name + "'s RES went up!");
            }

            chance = Random.Range(0, 101);

            if (playerStat.player.lukChance >= chance)
            {
                playerStat.player.maxLUK += 1;
                luk = " + 1";
                Debug.Log(playerStat.player.name + "'s LUK went up!");
            }

            playerStat.player.maxSP += 2;
            sp = " + 2";

            playerStat.player.currHP = playerStat.player.maxHP;
            playerStat.player.currSP = playerStat.player.maxSP;

            Debug.Log(playerStat.player.name + "'s SP went up by 2!");
        }

            LevelUpGUI(hp, atk, spd, def, res, luk, sp);
    }

    void LevelUpGUI(string hp, string atk, string spd, string def, string res, string luk, string sp)
    {
        if (!notThisCharacterLevelUp)
        {
            WinPanel.SetActive(false);
            LevelUpPanel.SetActive(true);

            PlayerStateMachine playerStat = CharsInBattle[levelCharacter].GetComponent<PlayerStateMachine>();
            Debug.Log(levelCharacter);

            Text statsName = LevelUpPanel.transform.GetChild(0).GetComponent<Text>();
            Text statsDEF = LevelUpPanel.transform.GetChild(1).GetComponent<Text>();

            statsName.text = playerStat.player.name + "\n" + "HP: " + playerStat.player.currHP + hp + "\n" + "ATK: " + playerStat.player.currATK + atk + "\n" + "SPD: " + playerStat.player.currSPD + spd;
            statsDEF.text = "DEF: " + playerStat.player.currDEF + def + "\n" + "RES: " + playerStat.player.currRES + res + "\n" + "LUK: " + playerStat.player.currLUK + luk + "\n" + "SP: " + playerStat.player.currSP + sp;
        }

        if (levelCharacter < CharsInBattle.Count - 1)
        {
            levelCharacter++;
            stopEarningEXP = false;
            timer = 0;
        }
        else
        { 
            levelUp = false;
            SaveVariables();
            timer = 0;
            returning = true;
        }
    }

    void SetStats()
    {
        hpPotions = GM.hpPotions;
        spPotions = GM.spPotions;

        for (int i = 0; i < CharsInBattle.Count; i++)
        {
            PlayerStateMachine playerStat = CharsInBattle[i].GetComponent<PlayerStateMachine>();

            if (playerStat.player.name == "Miku")
            {
                playerStat.player.maxATK = GM.mikuATK;
                playerStat.player.currHP = GM.mikuHP;
                playerStat.player.maxHP = GM.mikuMaxHP;
                playerStat.player.maxSP = GM.mikuMaxSP;
                playerStat.player.currSP = GM.mikuSP;
                playerStat.player.maxDEF = GM.mikuDEF;
                playerStat.player.maxRES = GM.mikuRES;
                playerStat.player.maxLUK = GM.mikuLUK;
                playerStat.player.maxSPD = GM.mikuSPD;
                playerStat.player.exp = GM.mikuEXP;
                playerStat.player.expToLevel = GM.mikuEXPtoLevel;

                playerStat.player.currATK = playerStat.player.maxATK;
                playerStat.player.currDEF = playerStat.player.maxDEF;
                playerStat.player.currRES = playerStat.player.maxRES;
                playerStat.player.currSPD = playerStat.player.maxSPD;
                playerStat.player.currLUK = playerStat.player.maxLUK;
            }

            if (playerStat.player.name == "Rin")
            {
                playerStat.player.maxATK = GM.rinATK;
                playerStat.player.currHP = GM.rinHP;
                playerStat.player.currSP = GM.rinSP;
                playerStat.player.maxHP = GM.rinMaxHP;
                playerStat.player.maxSP = GM.rinMaxSP;
                playerStat.player.maxDEF = GM.rinDEF;
                playerStat.player.maxRES = GM.rinRES;
                playerStat.player.maxLUK = GM.rinLUK;
                playerStat.player.maxSPD = GM.rinSPD;
                playerStat.player.exp = GM.rinEXP;
                playerStat.player.expToLevel = GM.rinEXPtoLevel;

                playerStat.player.currATK = playerStat.player.maxATK;
                playerStat.player.currDEF = playerStat.player.maxDEF;
                playerStat.player.currRES = playerStat.player.maxRES;
                playerStat.player.currSPD = playerStat.player.maxSPD;
                playerStat.player.currLUK = playerStat.player.maxLUK;
            }

            if (playerStat.player.name == "Len")
            {
                playerStat.player.maxATK = GM.lenATK;
                playerStat.player.currHP = GM.lenHP;
                playerStat.player.currSP = GM.lenSP;
                playerStat.player.maxHP = GM.lenMaxHP;
                playerStat.player.maxSP = GM.lenMaxSP;
                playerStat.player.maxDEF = GM.lenDEF;
                playerStat.player.maxRES = GM.lenRES;
                playerStat.player.maxLUK = GM.lenLUK;
                playerStat.player.maxSPD = GM.lenSPD;
                playerStat.player.exp = GM.lenEXP;
                playerStat.player.expToLevel = GM.lenEXPtoLevel;

                playerStat.player.currATK = playerStat.player.maxATK;
                playerStat.player.currDEF = playerStat.player.maxDEF;
                playerStat.player.currRES = playerStat.player.maxRES;
                playerStat.player.currSPD = playerStat.player.maxSPD;
                playerStat.player.currLUK = playerStat.player.maxLUK;
            }
        }
    }

    void SetStatsEnemy()
    {
        for (int i = 0; i < EnemiesInBattle.Count; i++)
        {
            EnemyStateMachine enemyStat = EnemiesInBattle[i].GetComponent<EnemyStateMachine>();

            enemyStat.enemy.maxHP = Random.Range(15, 26) + (GM.enemyLevel - 1);
            enemyStat.enemy.maxSP = Random.Range(5, 12) + (GM.enemyLevel - 1);
            enemyStat.enemy.maxSPD = Random.Range(3, 8) + (GM.enemyLevel - 1);
            enemyStat.enemy.maxATK = Random.Range(3, 8) + (GM.enemyLevel - 1);
            enemyStat.enemy.maxDEF = Random.Range(3, 6) + (GM.enemyLevel - 1);
            enemyStat.enemy.maxRES = Random.Range(2, 5) + (GM.enemyLevel - 1);
            enemyStat.enemy.maxLUK = Random.Range(1, 6) + (GM.enemyLevel - 1);

            enemyStat.enemy.currHP = enemyStat.enemy.maxHP;
            enemyStat.enemy.currSP = enemyStat.enemy.maxSP;
            enemyStat.enemy.currSPD = enemyStat.enemy.maxSPD;
            enemyStat.enemy.currATK = enemyStat.enemy.maxATK;
            enemyStat.enemy.currDEF = enemyStat.enemy.maxDEF;
            enemyStat.enemy.currRES = enemyStat.enemy.maxRES;
            enemyStat.enemy.currLUK = enemyStat.enemy.maxLUK;
        }
    }

    void SaveVariables()
    {
        int hpPup = Random.Range(0, 3);
        spPup = Random.Range(0, 3);

        hpPotions += hpPup;
        spPotions += spPup;

        GameObject clone = Instantiate(popUp, popUp.transform.position, popUp.transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
        clone.GetComponent<Text>().text = "+" + hpPup + " HP Potions";

        Destroy(clone, 1f);

        GM.hpPotions = hpPotions;
        GM.spPotions = spPotions;

        for (int i = 0; i < CharsInBattle.Count; i++)
        {
            PlayerStateMachine playerStat = CharsInBattle[i].GetComponent<PlayerStateMachine>();

            if (playerStat.player.name == "Miku")
            {
                GM.mikuATK = playerStat.player.maxATK;
                GM.mikuHP = playerStat.player.currHP;
                GM.mikuMaxHP = playerStat.player.maxHP;
                GM.mikuMaxSP = playerStat.player.maxSP;
                GM.mikuSP = playerStat.player.currSP;
                GM.mikuDEF = playerStat.player.maxDEF;
                GM.mikuRES = playerStat.player.maxRES;
                GM.mikuLUK = playerStat.player.maxLUK;
                GM.mikuSPD = playerStat.player.maxSPD;
                GM.mikuEXP = playerStat.player.exp;
                GM.mikuEXPtoLevel = playerStat.player.expToLevel;
            }

            if (playerStat.player.name == "Rin")
            {
                GM.rinATK = playerStat.player.maxATK;
                GM.rinHP = playerStat.player.currHP;
                GM.rinSP = playerStat.player.currSP;
                GM.rinMaxHP = playerStat.player.maxHP;
                GM.rinMaxSP = playerStat.player.maxSP;
                GM.rinDEF = playerStat.player.maxDEF;
                GM.rinRES = playerStat.player.maxRES;
                GM.rinLUK = playerStat.player.maxLUK;
                GM.rinSPD = playerStat.player.maxSPD;
                GM.rinEXP = playerStat.player.exp;
                GM.rinEXPtoLevel = playerStat.player.expToLevel;
            }

            if (playerStat.player.name == "Len")
            {
                GM.lenATK = playerStat.player.maxATK;
                GM.lenHP = playerStat.player.currHP;
                GM.lenSP = playerStat.player.currSP;
                GM.lenMaxHP = playerStat.player.maxHP;
                GM.lenMaxSP = playerStat.player.maxSP;
                GM.lenDEF = playerStat.player.maxDEF;
                GM.lenRES = playerStat.player.maxRES;
                GM.lenLUK = playerStat.player.maxLUK;
                GM.lenSPD = playerStat.player.maxSPD;
                GM.lenEXP = playerStat.player.exp;
                GM.lenEXPtoLevel = playerStat.player.expToLevel;
            }
        }
    }

    void SetupBattle()
    {
        GameObject robot0 = EnemiesInBattle[0];
        GameObject robot2 = EnemiesInBattle[1];
        GameObject robot1 = EnemiesInBattle[2];
        switch (GM.enemyNumber)
        {
            case 1:
                EnemiesInBattle.Remove(robot1);
                EnemiesInBattle.Remove(robot2);
                Destroy(robot1);
                Destroy(robot2);
                robot0.transform.position = new Vector2(6.83f, 1.38f);
                robot0.GetComponent<EnemyStateMachine>().startPos = new Vector2(6.83f, 1.38f);
                break;
            case 2:
                Destroy(robot1);
                EnemiesInBattle.Remove(robot1);
                robot0.transform.position = new Vector2(6.83f, 2.25f);
                robot0.GetComponent<EnemyStateMachine>().startPos = new Vector2(6.83f, 2.25f);
                robot2.transform.position = new Vector2(6.83f, 0.41f);
                robot2.GetComponent<EnemyStateMachine>().startPos = new Vector2(6.83f, 0.41f);
                break;
        }

        SetStatsEnemy();

        GameObject rin = CharsInBattle[1];
        GameObject len = CharsInBattle[0];
        GameObject miku = CharsInBattle[2];

        Transform RinUI = PlayerPanel.transform.GetChild(1);
        Transform LenUI = PlayerPanel.transform.GetChild(2);

        switch (GM.partyNumber)
        {
            case 1:
                Debug.Log("Why");
                CharsInBattle.Remove(rin);
                AllCharsInBattle.Remove(rin);
                CharsInBattle.Remove(len);
                AllCharsInBattle.Remove(len);
                Destroy(rin);
                Destroy(len);
                Destroy(RinUI.gameObject);
                Destroy(LenUI.gameObject);
                miku.transform.position = new Vector2(-7.5f, 1.13f);
                miku.GetComponent<PlayerStateMachine>().startPos = new Vector2(-7.5f, 1.13f);
                break;
            case 2:
                CharsInBattle.Remove(len);
                AllCharsInBattle.Remove(len);
                Destroy(len);
                Destroy(LenUI.gameObject);
                miku.transform.position = new Vector2(-7.5f, 2.25f);
                miku.GetComponent<PlayerStateMachine>().startPos = new Vector2(-7.5f, 2.25f);
                rin.transform.position = new Vector2(-7.5f, 0.41f);
                rin.GetComponent<PlayerStateMachine>().startPos = new Vector2(-7.5f, 0.41f);
                break;
        }
    }

    void Lost()
    {

        if (!stopLosing)
        {
            timer = 0;
            lost = true;

            for (int i = 0; i < AllCharsInBattle.Count; i++)
            {
                PlayerStateMachine playerStat = AllCharsInBattle[i].GetComponent<PlayerStateMachine>();

                playerStat.player.currHP = playerStat.player.maxHP;

                if (playerStat.player.name == "Miku")
                {
                    GM.mikuHP = playerStat.player.currHP;
                }
                if (playerStat.player.name == "Rin")
                {
                    GM.rinHP = playerStat.player.currHP;
                }
                if (playerStat.player.name == "Len")
                {
                    GM.lenHP = playerStat.player.currHP;
                }
            }
            
            stopLosing = true;
        }
    }

    void PlayWinMusic()
    {
        if (!stopPlayingMusic)
        {
            aManager.StopPlaying("BattleMusic");
            aManager.Play("BattleWon");
            stopPlayingMusic = true; 
        }
    }
}
