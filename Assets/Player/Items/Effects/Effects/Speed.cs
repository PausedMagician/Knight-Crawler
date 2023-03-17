using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : Effect
{
    public Speed(int amount, Item obj)
    {
        this.amount = amount;
        if(obj is Weapon) {
            this.name = "Attack Speed";
            this.description = "Increases attack speed by +" + amount + "%";
        } else if(obj is Armor) {
            this.name = "Movement Speed";
            this.description = "Increases movement speed by +" + amount + "%";
        }
    }
}