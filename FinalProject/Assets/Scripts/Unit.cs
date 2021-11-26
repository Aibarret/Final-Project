using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;

    public int damage;
    private int damageMOD;

    public int defense;
    private int defenseMOD;

    public int accuracy;
    private int accuracyMOD;

    public int evasion;
    private int evasionMOD;

    public int position;
    public bool isEnemy;

    public bool isKO;

    public int maxHP;
    public int currentHP;
    public HealthBar slider;

    public GameObject graphic;

    public Attributes attributeScript;

    public Attributes GetAttributes()
    {
        return attributeScript;
    }

    public int getMODS(string stat)
    {
        switch (stat)
        {
            case "damage":
                return damageMOD;
            case "defense":
                return defenseMOD;
            case "accuracy":
                return accuracyMOD;
            case "evasion":
                return evasionMOD;
            default:
                return 0;
        }
    }

    public void setMODS(string stat, int mod)
    {
        switch (stat)
        {
            case "damage":
                damageMOD += mod;
                break;
            case "defense":
                defenseMOD += mod;
                break;
            case "accuracy":
                accuracyMOD += mod;
                break;
            case "evasion":
                evasionMOD += mod;
                break;
            default:
                break;
        }
    }

    public void resetMODS()
    {
        damageMOD = 0;
        defenseMOD = 0;
        accuracyMOD = 0;
        evasionMOD = 0;
    }

    public int getPosition()
    {
        return position;
    }

    public void setPosition(int posn)
    {
        position = posn;
    }

    public void flip()
    {
        graphic.GetComponent<SpriteRenderer>().flipX = true;
    }

    

    public void heal(int amount)
    {
        if(currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        else
        {
            currentHP += amount;
        }
    }
}
