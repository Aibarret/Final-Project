using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFieldManager : MonoBehaviour
{
    BattleSystem battleSystem;

    private GameObject frontGO;
    private GameObject topGO;
    private GameObject botGO;

    private Unit frontUnit;
    private Unit topUnit;
    private Unit botUnit;

    private Attributes frontAtt;
    private Attributes topAtt;
    private Attributes botAtt;


    public Transform frontSlot;
    public Transform topSlot;
    public Transform botSlot;

    private void Start()
    {
        battleSystem = BattleSystem.FindObjectOfType<BattleSystem>();
    }

    public void initializeField(GameObject[] units, bool isEnemy)
    {
        frontGO = Instantiate(units[0], frontSlot);
        topGO = Instantiate(units[1], topSlot);
        botGO = Instantiate(units[2], botSlot);

        frontUnit = frontGO.GetComponent<Unit>();
        frontAtt = frontUnit.GetAttributes();
        frontUnit.setPosition(0);

        topUnit = topGO.GetComponent<Unit>();
        topAtt = topUnit.GetAttributes();
        topUnit.setPosition(1);
        
        botUnit = botGO.GetComponent<Unit>();
        botAtt = botUnit.GetAttributes();
        botUnit.setPosition(2);

        frontUnit.isEnemy = isEnemy;
        topUnit.isEnemy = isEnemy;
        botUnit.isEnemy = isEnemy;

        if (isEnemy)
        {
            frontUnit.flip();
            topUnit.flip();
            botUnit.flip();
        }
    }

    public void rotate(bool isRight)
    {
        if (isRight)
        {
            frontGO.transform.position = botSlot.position;
            botGO.transform.position = topSlot.position;
            topGO.transform.position = frontSlot.position;

            GameObject tempGO = botGO;
            Unit tempUnit = botUnit;
            Attributes tempAtt = botAtt;

            botGO = frontGO;
            botUnit = frontUnit;
            botAtt = frontAtt;

            frontGO = topGO;
            frontUnit = topUnit;
            frontAtt = topAtt;

            topGO = tempGO;
            topUnit = tempUnit;
            topAtt = tempAtt;
        }
        else
        {
            frontGO.transform.position = topSlot.position;
            topGO.transform.position = botSlot.position;
            botGO.transform.position = frontSlot.position;

            GameObject tempGO = botGO;
            Unit tempUnit = botUnit;
            Attributes tempAtt = botAtt;

            botGO = topGO;
            botUnit = topUnit;
            botAtt = topAtt;

            topGO = frontGO;
            topUnit = frontUnit;
            topAtt = frontAtt;

            frontGO = tempGO;
            frontUnit = tempUnit;
            frontAtt = tempAtt;
        }

        frontUnit.position = 0;
        topUnit.position = 1;
        botUnit.position = 2;

        if (frontUnit.isKO)
        {
            if (checkLoss())
            {
                battleSystem.ABIwipe(frontUnit.isEnemy);
            }
            else
            {
                rotate(isRight);
            }
            
        }
    }

    public void resetFieldMODS()
    {
        frontUnit.resetMODS();
        topUnit.resetMODS();
        botUnit.resetMODS();
    }

    public bool checkLoss()
    {
        return frontUnit.isKO && topUnit.isKO && botUnit.isKO;
    }

    public IEnumerator fieldPassive(PassiveState phase)
    {
        yield return topAtt.passive(phase);
        yield return botAtt.passive(phase);
    }

    public Unit[] getUnits()
    {
        return new Unit[3] { frontUnit, topUnit, botUnit };   
    }

    public Attributes[] GetAttributes()
    {
        return new Attributes[3] { frontAtt, topAtt, botAtt};
    }

    public bool isUnitLow(int posn)
    {
        return getUnits()[posn].currentHP < getUnits()[posn].maxHP / 4;
    }

    public bool isAnyUnitLow()
    {
        return 
            frontUnit.currentHP < frontUnit.maxHP / 4 ||
            topUnit.currentHP < topUnit.maxHP / 4 ||
            botUnit.currentHP < botUnit.maxHP / 4;
    }

    public bool isAllUnitLow()
    {
        return 
            frontUnit.currentHP < frontUnit.maxHP / 4 &&
            topUnit.currentHP < topUnit.maxHP / 4 &&
            botUnit.currentHP < botUnit.maxHP / 4;
    }

    public bool AIrotateDirection()
    {
        if (isUnitLow(1) && isUnitLow(2))
        {
            return true;
        } 
        else if (isUnitLow(1))
        {
            return true;
        }
        else if (isUnitLow(2))
        {
            return false;
        }
        else
        {
            return false;
        }
    }
}
