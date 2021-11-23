using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFieldManager : MonoBehaviour
{
    public GameObject frontGO;
    public GameObject topGO;
    public GameObject botGO;

    private Unit frontUnit;
    private Unit topUnit;
    private Unit botUnit;

    public Transform frontSlot;
    public Transform topSlot;
    public Transform botSlot;

    public void initializeField(GameObject[] units, bool isEnemy)
    {
        frontGO = Instantiate(units[0], frontSlot);
        topGO = Instantiate(units[1], topSlot);
        botGO = Instantiate(units[2], botSlot);

        frontUnit = frontGO.GetComponent<Unit>();
        topUnit = topGO.GetComponent<Unit>();
        botUnit = botGO.GetComponent<Unit>();

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

            botGO = frontGO;
            botUnit = frontUnit;

            frontGO = topGO;
            frontUnit = topUnit;

            topGO = tempGO;
            topUnit = tempUnit;
        }
        else
        {
            frontGO.transform.position = topSlot.position;
            topGO.transform.position = botSlot.position;
            botGO.transform.position = frontSlot.position;

            GameObject tempGO = botGO;
            Unit tempUnit = botUnit;

            botGO = topGO;
            botUnit = topUnit;

            topGO = frontGO;
            topUnit = frontUnit;

            frontGO = tempGO;
            frontUnit = tempUnit;
        }
        return;
    }

    public Unit[] getUnits()
    {
        return new Unit[3] { frontUnit, topUnit, botUnit };   
    }
}
