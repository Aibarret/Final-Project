using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterList : MonoBehaviour
{

    private BattleSystem battleSystem;
    public Unit unitScript;
    public Attributes att;

    //This is terrible implementation and I am deeply ashamed of it

    private void Start()
    {
        battleSystem = BattleSystem.FindObjectOfType<BattleSystem>();
    }

    // General idea of the lists is that it checks for the given unit's name and finds it's section.
    // This is hideous programming and I am deeply ashamed.

    public int offenseList(string name)
    {
        switch (name)
        {
            case "Dragomar":
                return unitScript.damage + unitScript.getMODS("damage");
            case "Deyece":
                return 0;
            default:
                return 0;
        }
    }

    public int defenseList(string name)
    {
        switch (name)
        {
            case "Dragomar":
                return unitScript.defense + unitScript.getMODS("defense");
            default:
                return 0;
        }
    }

    public IEnumerator passiveList(string name, PassiveState phase)
    {
        switch (name)
        {
            case "Dragomar":
                if (!unitScript.isKO)
                {
                    switch (phase)
                    {
                        case PassiveState.TURNSTART:
                            yield return StartCoroutine(AttackBoost(battleSystem.ABIgetFields(unitScript.isEnemy).getUnits()[0]));
                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }
    }

    public IEnumerator abilityList(string name)
    {
        switch (name)
        {
            case "Dragomar":
                print("ability SHOULD start");
                yield return StartCoroutine(WideSwing(battleSystem.ABIgetFields(unitScript.isEnemy), battleSystem.ABIgetFields(!unitScript.isEnemy), battleSystem.ABIgetTargetPosn()));
                break;
            default:
                print("failed to find ability");
                break;
        }
    }

    public IEnumerator aiList(string name, PFieldManager pField, PFieldManager eField)
    {
        switch (name)
        {
            case "Dragomar":
                yield return dragomarAI(pField, eField);
                break;
            default:
                break;
        }
    }

    //=========================================================================================================================================================================
    // Below is the master list of all AI actions in the game
    private IEnumerator dragomarAI(PFieldManager playerField, PFieldManager enemyField)
    {

        // Decides if will rotate party
        if (enemyField.isUnitLow(0) && !enemyField.isAllUnitLow())
        {
            enemyField.rotate(enemyField.AIrotateDirection());
            battleSystem.ABIsetHud(enemyField.getUnits()[0].isEnemy);

            battleSystem.ABIupdateDialogue("Enemy Rotates!");

            StartCoroutine(enemyField.getUnits()[0].GetAttributes().enemyAction(playerField, enemyField));
        }
        else
        {
            // decides action
            if (enemyField.isAnyUnitLow())
            {
                // Use ability
                yield return StartCoroutine(enemyField.fieldPassive(PassiveState.STARTATTACK));

                StartCoroutine(WideSwing(playerField, enemyField, 0));
            }
            else
            {
                // Use Attack
                battleSystem.ABIupdateDialogue(unitScript.unitName + " Attacks!");

                yield return new WaitForSeconds(1f);

                if (battleSystem.isHit(unitScript, enemyField.getUnits()[0]))
                {
                    battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcalcOffenseDefense(att, enemyField.getUnits()[0].attributeScript) + " damage!");
                    enemyField.getUnits()[0].attributeScript.takeDamage(battleSystem.ABIcalcOffenseDefense(att, enemyField.getUnits()[0].attributeScript));
                }
                else
                {
                    battleSystem.ABIupdateDialogue("They missed!");
                }

                yield return new WaitForSeconds(1f);

                print("doot");

                StartCoroutine(battleSystem.endABI(true));
            }
        }
    }

    // Below is a master list of all passive abilities in the game

    IEnumerator AttackBoost(Unit front)
    {
        battleSystem.ABIupdateDialogue(unitScript.unitName + "'s passive!");
        yield return new WaitForSeconds(1f);
        battleSystem.ABIupdateDialogue(unitScript.unitName + " Boosts damage by 3!");
        front.setMODS("damage", 3);
        yield return new WaitForSeconds(1f);
    }

    // Below is a master list of all active abilities in the game

    IEnumerator WideSwing(PFieldManager playerField, PFieldManager enemyField, int targetPosn)
    {
        //Inflicts half damage to all opponent party members
        print("ability starts");

        battleSystem.ABIupdateDialogue(unitScript.unitName + " uses their ability!");

        Unit[] targets = enemyField.getUnits();

        print(targets);

        int frontHitRate = unitScript.accuracy - targets[0].evasion;
        int topHitRate = unitScript.accuracy - targets[1].evasion;
        int botHitRate = unitScript.accuracy - targets[2].evasion;

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) >= frontHitRate)
        {
            battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcustAttackDefend(att.offense() / 2, targets[0].GetAttributes().defense()) + " damage!");
            targets[0].GetAttributes().takeDamage(battleSystem.ABIcustAttackDefend(att.offense() / 2, targets[0].GetAttributes().defense()));
        }
        else
        {
            battleSystem.ABIupdateDialogue("Miss!");
        }

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) >= topHitRate)
        {
            battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcustAttackDefend(att.offense() / 2, targets[1].GetAttributes().defense()) + " damage!");
            targets[1].GetAttributes().takeDamage(battleSystem.ABIcustAttackDefend(att.offense() / 2, targets[1].GetAttributes().defense()));
        }
        else
        {
            battleSystem.ABIupdateDialogue("Miss!");
        }

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) >= botHitRate)
        {
            battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcustAttackDefend(att.offense() / 2, targets[2].GetAttributes().defense()) + " damage!");
            targets[2].GetAttributes().takeDamage(battleSystem.ABIcustAttackDefend(att.offense() / 2, targets[2].GetAttributes().defense()));
        }
        else
        {
            battleSystem.ABIupdateDialogue("Miss!");
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(battleSystem.endABI(unitScript.isEnemy));
    }

}
