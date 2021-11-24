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


    public void ability(Unit target)
    {
       // offense() + 10;
    }
}
