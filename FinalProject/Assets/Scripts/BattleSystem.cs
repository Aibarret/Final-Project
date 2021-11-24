using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST}

public class BattleSystem : MonoBehaviour
{
    public string debug;

    public BattleState state;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public GameObject playerField;
    PFieldManager pField;

    public GameObject enemyField;
    PFieldManager eField;

    public Text dialogue;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    private int targetPosn;
    public Text targetCharText;


    void Start()
    {
        state = BattleState.START;

        GameObject units = GameObject.Find("Units");
        AvailibleUnits unitList = units.GetComponent<AvailibleUnits>();

        //TODO: Add in method call (or script?) that creates the team
        GameObject[] testPlayerTeam = new GameObject[3] { unitList.Andy, unitList.Aurelia, unitList.Denda };
        GameObject[] testEnemyTeam = new GameObject[3] { unitList.Spike, unitList.Zantz, unitList.Rione };

        StartCoroutine(setUpBattle(testPlayerTeam, testEnemyTeam));
    }

    IEnumerator setUpBattle(GameObject[] playerTeam, GameObject[] enemyTeam)
    {
        dialogue.text = "Battle Start!";

        pField = playerField.GetComponent<PFieldManager>();
        pField.initializeField(playerTeam, false);

        eField = enemyField.GetComponent<PFieldManager>();
        eField.initializeField(enemyTeam, true);

        playerHUD.setHUD(pField);
        enemyHUD.setHUD(eField);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        playerTurn();
    }

    void playerTurn()
    {
        targetPosn = 0;
        setTarget(targetPosn);

        dialogue.text = "Player Turn!";
    }

    public void setTarget(int posn)
    {
        targetCharText.text = eField.getUnits()[posn].unitName;
    }

    IEnumerator playerAttack()
    {
        //bool isDead = enemyUnit.takeDamage(playerUnit.damage);

        //enemyHUD.setHP(enemyUnit.currentHP);
        //dialogue.text = "The attack is successful!";

        yield return new WaitForSeconds(2f);

        //if (isDead)
        //{
        //    state = BattleState.WON;
        //    EndBattle();
        //}
        //else
        //{
        //    state = BattleState.ENEMYTURN;
        //    StartCoroutine(EnemyTurn());
        //}

    }

    //IEnumerator EnemyTurn()
    //{
    //    dialogue.text = enemyUnit.unitName + " attacks!";

    //    yield return new WaitForSeconds(1f);

    //    bool isDead = playerUnit.takeDamage(enemyUnit.damage);

    //    playerHUD.setHP(playerUnit.currentHP);

    //    yield return new WaitForSeconds(1f);

    //    if (isDead)
    //    {
    //        state = BattleState.LOST;
    //        EndBattle();
    //    }
    //    else
    //    {
    //        state = BattleState.PLAYERTURN;
    //        playerTurn();
    //    }
    //}

    void EndBattle()
    {
        if(state == BattleState.WON)
        {
            dialogue.text = "You won the battle!";
        } else if (state == BattleState.LOST)
        {
            dialogue.text = "You were defeated.";
        }
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        StartCoroutine(playerAttack());
    }

    public void OnRight()
    {
        if (state == BattleState.PLAYERTURN)
        {
            pField.rotate(true);
            playerHUD.setHUD(pField);
        }
    }

    public void OnLeft()
    {
        if (state == BattleState.PLAYERTURN)
        {
            pField.rotate(false);
            playerHUD.setHUD(pField);
        }
    }

    public void OnTargetLeft()
    {
        if (state == BattleState.PLAYERTURN)
        {
            targetPosn -= 1;
            if (targetPosn < 0)
            {
                targetPosn = 2;
            }
            setTarget(targetPosn);
        }
    }

    public void OnTargetRight()
    {
        if (state == BattleState.PLAYERTURN)
        {
            targetPosn += 1;
            if (targetPosn > 2)
            {
                targetPosn = 0;
            }
            setTarget(targetPosn);
        }
    }
}
