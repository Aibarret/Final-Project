using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFieldManager : MonoBehaviour
{
    private GameObject frontGO;
    private GameObject topGO;
    private GameObject botGO;

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
}
