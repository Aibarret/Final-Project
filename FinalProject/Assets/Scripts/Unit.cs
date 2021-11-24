using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public int damage;

    private int position;

    public int maxHP;
    public int currentHP;
    public HealthBar slider;

    public GameObject graphic;

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

    public bool takeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
        {
            return true;
        } else
        {
            return false;
        }
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
