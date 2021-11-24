using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public int damage;
    public int defense;

    public int accuracy;
    public int evasion;

    private int position;
    public bool isEnemy;

    public int maxHP;
    public int currentHP;
    public HealthBar slider;

    public GameObject graphic;

    public Attributes attributeScript;

    public Attributes GetAttributes()
    {
        return attributeScript;
    }

    private void Start()
    {
        
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
