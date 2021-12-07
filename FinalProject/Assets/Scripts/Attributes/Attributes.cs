using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Attributes : MonoBehaviour
{

    public Unit unitScript;
    private BattleSystem battleSystem;
    public MasterList ml;


    private void Start()
    {
        battleSystem = BattleSystem.FindObjectOfType<BattleSystem>();
    }

    public int offense()
    {
        return ml.offenseList(unitScript.unitName);
    }

    public int defense()
    {
        return ml.defenseList(unitScript.unitName);
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
        yield return ml.passiveList(unitScript.unitName, phase);
    }

    public void ability()
    {
        print("reached attributes");
        StartCoroutine(ml.abilityList(unitScript.unitName));
    }


   

    public IEnumerator enemyAction(PFieldManager playerField, PFieldManager enemyField)
    {
        yield return ml.aiList(unitScript.unitName, playerField, enemyField);
    }
}
