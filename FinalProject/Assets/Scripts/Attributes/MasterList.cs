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
    // This is hideous programming


    // Offense and Defense list will return default when given name isn't supposed to be different than normal
    public int offenseList(string name)
    {
        switch (name)
        {
            default:
                return unitScript.damage + unitScript.getMODS("damage");
        }
    }

    public int defenseList(string name)
    {
        switch (name)
        {
            default:
                return unitScript.defense + unitScript.getMODS("defense");
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
            case "Hawking":
                if (!unitScript.isKO)
                {
                    switch (phase)
                    {
                        case PassiveState.STARTATTACK:
                            yield return StartCoroutine(AccuracyBoost(battleSystem.ABIgetFields(unitScript.isEnemy).getUnits()[0]));
                            break;
                        default:
                            break;
                    }
                }
                break;
            case "Deyece":
                if (!unitScript.isKO)
                {
                    switch (phase)
                    {
                        case PassiveState.ENDATTACK:
                            yield return StartCoroutine(RollofTheDice(battleSystem.ABIgetFields(unitScript.isEnemy).getUnits()));
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
                yield return StartCoroutine(WideSwing(battleSystem.ABIgetFields(unitScript.isEnemy), battleSystem.ABIgetFields(!unitScript.isEnemy), battleSystem.ABIgetTargetPosn()));
                break;
            case "Hawking":
                yield return StartCoroutine(WindStrike(battleSystem.ABIgetFields(unitScript.isEnemy), battleSystem.ABIgetFields(!unitScript.isEnemy)));
                break;
            case "Deyece":
                print("case successful");
                yield return StartCoroutine(heavensGamble(battleSystem.ABIgetFields(unitScript.isEnemy), battleSystem.ABIgetFields(!unitScript.isEnemy)));
                break;
            default:
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
            case "Hawking":
                yield return hawkingAI(pField, eField);
                break;
            case "Deyece":
                yield return deyeceAI(pField, eField);
                break;
            default:
                yield return defaultAI(pField, eField);
                break;
        }
    }

    //=========================================================================================================================================================================
    // Below is the master list of all AI actions in the game

    private IEnumerator defaultAI(PFieldManager playerField, PFieldManager enemyField)
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

            StartCoroutine(battleSystem.endABI(true));
        }
    }

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

                StartCoroutine(battleSystem.endABI(true));
            }
        }
    }

    IEnumerator hawkingAI(PFieldManager playerField, PFieldManager enemyField)
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
            // Use ability
            yield return StartCoroutine(enemyField.fieldPassive(PassiveState.STARTATTACK));

            StartCoroutine(WideSwing(playerField, enemyField, 0));
        }
    }

    IEnumerator deyeceAI(PFieldManager playerField, PFieldManager enemyField)
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
            // Use ability
            yield return StartCoroutine(enemyField.fieldPassive(PassiveState.STARTATTACK));

            StartCoroutine(WideSwing(playerField, enemyField, 0));
        }
    }

    // Below is a master list of all passive abilities in the game

    IEnumerator AttackBoost(Unit front)
    {
        battleSystem.ABIupdateDialogue(unitScript.unitName + "'s passive!");
        yield return new WaitForSeconds(1f);
        battleSystem.ABIupdateDialogue(unitScript.unitName + " Boosts damage!");
        front.setMODS("damage", 3);
        yield return new WaitForSeconds(1f);
    }

    IEnumerator AccuracyBoost(Unit front)
    {
        battleSystem.ABIupdateDialogue(unitScript.unitName + "'s passive!");
        yield return new WaitForSeconds(1f);
        battleSystem.ABIupdateDialogue(unitScript.unitName + " Boosts accuracy!");
        front.setMODS("accuracy", 50);
        yield return new WaitForSeconds(1f);
    }

    IEnumerator RollofTheDice(Unit[] targets)
    {
        battleSystem.ABIupdateDialogue(unitScript.unitName + "'s passive!");
        yield return new WaitForSeconds(1f);
        battleSystem.ABIupdateDialogue(unitScript.unitName + " heals randomly!");
        int healrate = -1 * Random.Range(1, 7);
        targets[0].GetAttributes().takeDamage(healrate);
        healrate = -1 * Random.Range(1, 7);
        targets[1].GetAttributes().takeDamage(healrate);
        healrate = -1 * Random.Range(1, 7);
        targets[2].GetAttributes().takeDamage(healrate);
    }

    // Below is a master list of all active abilities in the game

    IEnumerator WideSwing(PFieldManager playerField, PFieldManager enemyField, int targetPosn)
    {
        //Inflicts half damage to all opponent party members
        

        battleSystem.ABIupdateDialogue(unitScript.unitName + " uses their ability!");

        Unit[] targets = enemyField.getUnits();

        int frontHitRate = unitScript.accuracy - targets[0].evasion;
        int topHitRate = unitScript.accuracy - targets[1].evasion;
        int botHitRate = unitScript.accuracy - targets[2].evasion;

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) <= frontHitRate)
        {
            battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcustAttackDefend(att.offense() / 2, targets[0].GetAttributes().defense()) + " damage!");
            targets[0].GetAttributes().takeDamage(battleSystem.ABIcustAttackDefend(att.offense() / 2, targets[0].GetAttributes().defense()));
        }
        else
        {
            battleSystem.ABIupdateDialogue("Miss!");
        }

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) <= topHitRate)
        {
            battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcustAttackDefend(att.offense() / 2, targets[1].GetAttributes().defense()) + " damage!");
            targets[1].GetAttributes().takeDamage(battleSystem.ABIcustAttackDefend(att.offense() / 2, targets[1].GetAttributes().defense()));
        }
        else
        {
            battleSystem.ABIupdateDialogue("Miss!");
        }

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) <= botHitRate)
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

    IEnumerator WindStrike(PFieldManager attackerField, PFieldManager defenderField)
    {
        //Inflicts damage to backrow only


        battleSystem.ABIupdateDialogue(unitScript.unitName + " uses their ability!");

        Unit[] targets = defenderField.getUnits();

        int topHitRate = unitScript.accuracy - targets[1].evasion;
        int botHitRate = unitScript.accuracy - targets[2].evasion;

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) <= topHitRate)
        {
            battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcustAttackDefend(att.offense(), targets[1].GetAttributes().defense()) + " damage!");
            targets[1].GetAttributes().takeDamage(battleSystem.ABIcustAttackDefend(att.offense(), targets[1].GetAttributes().defense()));
        }
        else
        {
            battleSystem.ABIupdateDialogue("Miss!");
        }

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) <= botHitRate)
        {
            battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcustAttackDefend(att.offense(), targets[2].GetAttributes().defense()) + " damage!");
            targets[2].GetAttributes().takeDamage(battleSystem.ABIcustAttackDefend(att.offense(), targets[2].GetAttributes().defense()));
        }
        else
        {
            battleSystem.ABIupdateDialogue("Miss!");
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(battleSystem.endABI(unitScript.isEnemy));
    }

    IEnumerator heavensGamble(PFieldManager attackerField, PFieldManager defenderField)
    {
        // has a 50/50 chance of either dealing damage to all oppoenents or allies

        battleSystem.ABIupdateDialogue(unitScript.unitName + " uses their ability!");

        yield return new WaitForSeconds(1f);

        battleSystem.ABIupdateDialogue(unitScript.unitName + " rolls the dice!");

        yield return new WaitForSeconds(1f);

        if (Random.Range(0, 2) > 0)
        {
            Unit[] targets = defenderField.getUnits();

            battleSystem.ABIupdateDialogue(unitScript.unitName + " won!");

            yield return new WaitForSeconds(1f);

            battleSystem.ABIupdateDialogue("All opponent's took damage!");

            targets[0].GetAttributes().takeDamage(5);
            targets[1].GetAttributes().takeDamage(5);
            targets[2].GetAttributes().takeDamage(5);
        }
        else
        {
            Unit[] targets = attackerField.getUnits();

            battleSystem.ABIupdateDialogue(unitScript.unitName + " lost!");

            yield return new WaitForSeconds(1f);

            battleSystem.ABIupdateDialogue("All allies took damage!");

            targets[0].GetAttributes().takeDamage(5);
            targets[1].GetAttributes().takeDamage(5);
            targets[2].GetAttributes().takeDamage(5);
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(battleSystem.endABI(unitScript.isEnemy));
    }

}
