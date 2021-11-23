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

    public Transform playerOne;

    public void setHUD(PFieldManager field)
    {
        Unit[] list = field.getUnits();
        frontName.text = list[0].unitName;

        frontName.text = list[0].unitName;
        frontHp.GetComponent<Slider>().maxValue = list[0].maxHP;
        frontHp.GetComponent<Slider>().value = list[0].currentHP;
        
        topName.text = list[1].unitName;
        topHp.GetComponent<Slider>().maxValue = list[1].maxHP;
        topHp.GetComponent<Slider>().value = list[1].currentHP;
        
        botName.text = list[2].unitName;
        botHp.GetComponent<Slider>().maxValue = list[2].maxHP;
        botHp.GetComponent<Slider>().value = list[2].currentHP;
    }

    public void setHP(int hp)
    {
        //hpSlider.value = hp;
    }
}
