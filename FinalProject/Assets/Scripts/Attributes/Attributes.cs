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
        return unitScript.damage;
    }

    public int defense()
    {
        return unitScript.defense;
    }

    public void takeDamage(int dmg)
    {
        if (unitScript.currentHP - dmg <= 0)
        {
            unitScript.currentHP = 0;
        }
        else
        {
            unitScript.currentHP -= dmg;
        }

        battleSystem.ABIupdateHealth(unitScript.currentHP, unitScript.isEnemy, unitScript.getPosition());

    }

    public void ability(PFieldManager playerField, PFieldManager enemyField, int targetPosn)
    {
        StartCoroutine(wideSwing(playerField, enemyField, targetPosn));
    }

    IEnumerator wideSwing(PFieldManager playerField, PFieldManager enemyField, int targetPosn)
    {
        //Inflicts half damage to all opponent party members

        battleSystem.ABIupdateDialogue(unitScript.name + " uses their ability!");

        Unit[] targets = enemyField.getUnits();

        int frontHitRate = unitScript.accuracy - targets[0].evasion;
        int topHitRate = unitScript.accuracy - targets[1].evasion;
        int botHitRate = unitScript.accuracy - targets[2].evasion;

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) >= frontHitRate)
        {
            battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcustAttackDefend(unitScript.damage / 3, targets[0].defense) + " damage!");
            targets[0].GetAttributes().takeDamage(battleSystem.ABIcustAttackDefend(unitScript.damage / 3, targets[0].defense));
        }
        else
        {
            battleSystem.ABIupdateDialogue("Miss!");
        }

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) >= topHitRate)
        {
            battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcustAttackDefend(unitScript.damage / 3, targets[1].defense) + " damage!");
            targets[1].GetAttributes().takeDamage(battleSystem.ABIcustAttackDefend(unitScript.damage / 3, targets[1].defense));
        }
        else
        {
            battleSystem.ABIupdateDialogue("Miss!");
        }

        yield return new WaitForSeconds(1f);

        if (Random.Range(1, 101) >= botHitRate)
        {
            battleSystem.ABIupdateDialogue("Deals " + battleSystem.ABIcustAttackDefend(unitScript.damage / 3, targets[2].defense) + " damage!");
            targets[2].GetAttributes().takeDamage(battleSystem.ABIcustAttackDefend(unitScript.damage / 3, targets[2].defense));
        }
        else
        {
            battleSystem.ABIupdateDialogue("Miss!");
        }

        yield return new WaitForSeconds(1f);

        battleSystem.endABI();
    }
}
