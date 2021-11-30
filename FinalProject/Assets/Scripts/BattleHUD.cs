using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text frontName;
    public HealthBar frontHp;

    public Text topName;
    public HealthBar topHp;

    public Text botName;
    public HealthBar botHp;
    

    public void setHUD(PFieldManager field)
    {
        Unit[] list = field.getUnits();

        frontName.text = list[0].unitName;
        frontHp.setMaxHealth(list[0].maxHP);
        frontHp.setHealth(list[0].currentHP);

        topName.text = list[1].unitName;
        topHp.setMaxHealth(list[1].maxHP);
        topHp.setHealth(list[1].currentHP);

        botName.text = list[2].unitName;
        botHp.setMaxHealth(list[2].maxHP);
        botHp.setHealth(list[2].currentHP);
    }

    public void setHP(int hp, int target)
    {
        // target: 0 = front, 1 = top, 2 = bot 

        switch (target)
        {
            case 0:
                frontHp.setHealth(hp);
                break;
            case 1:
                topHp.setHealth(hp);
                break;
            case 2:
                botHp.setHealth(hp);
                break;
            default:
                break;
        }
    }
}
