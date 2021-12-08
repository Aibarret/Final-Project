using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST}
public enum PassiveState { NONE, TURNSTART, STARTATTACK, ENDATTACK}

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

    public Button fight;
    public Button ability;
    public Button rotatePartyRight;
    public Button rotatePartyLeft;
    public Button rotateTargetRight;
    public Button rotateTargetLeft;


    void Start()
    {
        state = BattleState.START;

        GameObject units = GameObject.Find("Units");
        AvailibleUnits unitList = units.GetComponent<AvailibleUnits>();

        //TODO: Add in method call (or script?) that creates the team
        GameObject[] testPlayerTeam = new GameObject[3] { unitList.Wizzaro, unitList.Sir, unitList.deyece };
        GameObject[] testEnemyTeam = new GameObject[3] { unitList.dragomar, unitList.dragomar, unitList.dragomar };

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

        yield return new WaitForSeconds(1f);

        
        StartCoroutine(playerTurn());
    }

    IEnumerator playerTurn()
    {
        if (pField.checkLoss() || eField.checkLoss())
        {
            if (pField.checkLoss())
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.WON;
                EndBattle();
            }
        }
        else
        {
            dialogue.text = "Player Turn!";

            yield return new WaitForSeconds(1f);

            yield return pField.fieldPassive(PassiveState.TURNSTART);

            state = BattleState.PLAYERTURN;

            targetPosn = 0;
            setTarget(targetPosn);

            dialogue.text = "What will you do?";

            updateAllButtons(true);
        }
    }

    public void endTurn()
    {
        pField.resetFieldMODS();

        StartCoroutine(EnemyTurn());
    }

    public void endEnemyTurn()
    {
        eField.resetFieldMODS();


        StartCoroutine(playerTurn());
    }

    public void setTarget(int posn)
    {
        targetCharText.text = eField.getUnits()[posn].unitName;
    }

    IEnumerator playerAttack()
    {
        yield return pField.fieldPassive(PassiveState.STARTATTACK);

        Unit[] attackUnits = pField.getUnits();
        dialogue.text = attackUnits[0].unitName + " attacks!";

        yield return new WaitForSeconds(1f);

        Unit attacker = pField.getUnits()[0];
        Unit target = eField.getUnits()[targetPosn];

        int hitPercentage = attacker.accuracy - target.evasion;

        if (Random.Range(1, 101) <= hitPercentage)
        {
            dialogue.text = "Deals " + ABIcalcOffenseDefense(attacker.GetAttributes(), target.GetAttributes()) + " damage!";
            target.GetAttributes().takeDamage(ABIcalcOffenseDefense(attacker.GetAttributes(), target.GetAttributes()));
        }
        else
        {
            dialogue.text = "They missed!";
        }

        yield return new WaitForSeconds(1f);

        yield return pField.fieldPassive(PassiveState.ENDATTACK);

        endTurn();
    }

    IEnumerator EnemyTurn()
    {
        if (pField.checkLoss() || eField.checkLoss())
        {
            if (pField.checkLoss())
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.WON;
                EndBattle();
            }
        }
        else
        {
            dialogue.text = "Enemy Turn!";

            yield return new WaitForSeconds(1f);

            state = BattleState.ENEMYTURN;

            yield return eField.fieldPassive(PassiveState.TURNSTART);

            yield return new WaitForSeconds(1f);

            dialogue.text = "Choosing Actions!";

            yield return new WaitForSeconds(1f);

            yield return eField.getUnits()[0].GetAttributes().enemyAction(eField, pField);

            //yield return new WaitForSeconds(1f);
            //dialogue.text = "Enemy Passes!";

            //yield return new WaitForSeconds(1f);
        }

    }

    void EndBattle()
    {
        if(state == BattleState.WON)
        {
            dialogue.text = "You won the battle!";
        } else if (state == BattleState.LOST)
        {
            dialogue.text = "You were defeated.";
        }
        updateAllButtons(false);
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        else
        {
            updateAllButtons(false);
            StartCoroutine(playerAttack());
        }   
    }

    public void OnAbilityButton()
    {
        updateAllButtons(false);
        StartCoroutine(abilityEnumerator());
    }

    IEnumerator abilityEnumerator()
    {
        yield return StartCoroutine(pField.fieldPassive(PassiveState.STARTATTACK));
        Attributes abilityUser = pField.getUnits()[0].GetAttributes();

        abilityUser.ability();
    }

    public void OnRight()
    {
        
        if (state == BattleState.PLAYERTURN)
        {
            updatePartyRotate(false);
            pField.rotate(true);
            playerHUD.setHUD(pField);
            
        }
    }

    public void OnLeft()
    {
        if (state == BattleState.PLAYERTURN)
        {
            updatePartyRotate(false);
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

            if (eField.getUnits()[targetPosn].isKO)
            {
                OnTargetLeft();
            }
            else
            {
                setTarget(targetPosn);
            }
            
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

            if (eField.getUnits()[targetPosn].isKO)
            {
                OnTargetRight();
            }
            else
            {
                setTarget(targetPosn);
            }
        }
    }

    // Funtions for making updating UI Buttons easier
    public void updateAllButtons(bool isInteractible)
    {
        fight.interactable = isInteractible;
        ability.interactable = isInteractible;
        rotatePartyRight.interactable = isInteractible;
        rotatePartyLeft.interactable = isInteractible;
        rotateTargetRight.interactable = isInteractible;
        rotateTargetLeft.interactable = isInteractible;
    }

    public void updateFightButtons(bool isInteractible)
    {
        fight.interactable = isInteractible;
        rotateTargetRight.interactable = isInteractible;
        rotateTargetLeft.interactable = isInteractible;
    }

    public void updatePartyRotate(bool isInteractible)
    {
        rotatePartyRight.interactable = isInteractible;
        rotatePartyLeft.interactable = isInteractible;
    }

    public void updateAbility(bool isInteractible)
    {
        ability.interactable = isInteractible;
    }

    // Furthur functions are intended to be used exclusivly by Abilties

    public void ABIkoRePosition(bool isEnemy)
    {
        if (isEnemy)
        {
            eField.rotate(false);
            enemyHUD.setHUD(eField);
        }
        else
        {
            pField.rotate(false);
            playerHUD.setHUD(pField);
        }
    }

    public PFieldManager ABIgetFields(bool isEnemy)
    {
        if (isEnemy)
        {
            return eField;
        }
        else
        {
            return pField;
        }
    }

    public int ABIgetTargetPosn()
    {
        return targetPosn;
    }

    public void ABIupdateHealth(int hp, bool isEnemy, int target)
    {
        print(target);
        if (isEnemy)
        {
            enemyHUD.setHP(hp, target);
        }
        else
        {
            playerHUD.setHP(hp, target);
        }
    }

    public int ABIcalcOffenseDefense(Attributes attacker, Attributes defender)
    {
        int difference = attacker.offense() - defender.defense();

        if (difference < 0)
        {
            return 0;
        }
        else
        {
            return difference;
        }
    }

    public int ABIcustAttackDefend(int attack, int defend)
    {
        int difference = attack - defend;

        if (difference < 0)
        {
            return 0;
        }
        else
        {
            return difference;
        }
    }

    public void ABIupdateDialogue(string text)
    {
        dialogue.text = text;
    }

    public void ABIsetHud(bool isEnemy)
    {
        if (isEnemy)
        {
            enemyHUD.setHUD(eField);
        }
        else
        {
            playerHUD.setHUD(pField);
        }
    }

    public void ABIwipe(bool isEnemy)
    {
        if (isEnemy)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.LOST;
            EndBattle();
        }
    }

    public bool isHit(Unit attacker, Unit defender)
    {
        int hitRate = attacker.accuracy - defender.evasion;

        Random.seed = System.DateTime.Now.Millisecond;
        return Random.Range(1, 101) <= hitRate;
    }

    public IEnumerator endABI(bool isEnemy)
    {
        if (isEnemy)
        {
            yield return eField.fieldPassive(PassiveState.ENDATTACK);

            print("scoot");

            endEnemyTurn();
        }
        else
        {
            yield return pField.fieldPassive(PassiveState.ENDATTACK);

            endTurn();
        }
    }
}
