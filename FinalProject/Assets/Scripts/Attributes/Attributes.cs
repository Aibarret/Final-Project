using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Attributes : MonoBehaviour
{

    public Unit unitScript;
    private BattleSystem battleSystem;


    private void Start()
    {
        battleSystem = BattleSystem.FindObjectOfType<BattleSystem>();
    }

    public int offense()
    {
        return unitScript.damage + unitScript.getMODS("damage");
    }

    public int defense()
    {
        return unitScript.defense + unitScript.getMODS("defense");
    }

    public void takeDamage(int dmg)
    {
        if (unitScript.currentHP - dmg <= 0)
        {
            unitScript.currentHP = 0;
            battleSystem.ABIupdateHealth(unitScript.currentHP, unitScript.isEnemy, unitScript.getPosition());
            knockOut();
        }
        else
        {
            unitScript.currentHP -= dmg;
            battleSystem.ABIupdateHealth(unitScript.currentHP, unitScript.isEnemy, unitScript.getPosition());
        }

        

    }

    public void knockOut()
    {
        unitScript.isKO = true;

        if (unitScript.getPosition() == 0)
        {
            battleSystem.ABIkoRePosition(unitScript.isEnemy);
        }   
    }

    public IEnumerator passive(PassiveState phase)
    {
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
    }

    public void ability()
    {
        StartCoroutine(WideSwing(battleSystem.ABIgetFields(unitScript.isEnemy), battleSystem.ABIgetFields(!unitScript.isEnemy), battleSystem.ABIgetTargetPosn()));
    }

    IEnumerator AttackBoost(Unit front)
    {
        battleSystem.ABIupdateDialogue(unitScript.unitName + "'s passive!");

        yield return new WaitForSeconds(1f);

        battleSystem.ABIupdateDialogue(unitScript.unitName + " Boosts damage by 3!");

        front.setMODS("damage", 3);

        yield return new WaitForSeconds(1f);
    }

    IEnumerator WideSwing(PFieldManager playerField, PFieldManager enemyField, int targetPosn)
    {
        //Inflicts half damage to all opponent party members

        battleSystem.ABIupdateDialogue(unitScript.unitName + " uses their ability!");

        Unit[] targets = enemyField.getUnits();

        print(targets);

        int frontHitRate = unitScript.accuracy - targets[0].evasion;
        int topHitRate = unitScript.accuracy - targets[1].evasion;
        int botHitRate = unitScript.accuracy - targets[2].evasion;

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) >= frontHitRate)
        {
            battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcustAttackDefend(offense() / 2, targets[0].GetAttributes().defense()) + " damage!");
            targets[0].GetAttributes().takeDamage(battleSystem.ABIcustAttackDefend(offense() / 2, targets[0].GetAttributes().defense()));
        }
        else
        {
            battleSystem.ABIupdateDialogue("Miss!");
        }

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) >= topHitRate)
        {
            battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcustAttackDefend(offense() / 2, targets[1].GetAttributes().defense()) + " damage!");
            targets[1].GetAttributes().takeDamage(battleSystem.ABIcustAttackDefend(offense() / 2, targets[1].GetAttributes().defense()));
        }
        else
        {
            battleSystem.ABIupdateDialogue("Miss!");
        }

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) >= botHitRate)
        {
            battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcustAttackDefend(offense() / 2, targets[2].GetAttributes().defense()) + " damage!");
            targets[2].GetAttributes().takeDamage(battleSystem.ABIcustAttackDefend(offense() / 2, targets[2].GetAttributes().defense()));
        }
        else
        {
            battleSystem.ABIupdateDialogue("Miss!");
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(battleSystem.endABI(unitScript.isEnemy));
    }

    public IEnumerator enemyAction(PFieldManager playerField, PFieldManager enemyField)
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
                    battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcalcOffenseDefense(this, enemyField.getUnits()[0].attributeScript) + " damage!");
                    enemyField.getUnits()[0].attributeScript.takeDamage(battleSystem.ABIcalcOffenseDefense(this, enemyField.getUnits()[0].attributeScript));
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
}
