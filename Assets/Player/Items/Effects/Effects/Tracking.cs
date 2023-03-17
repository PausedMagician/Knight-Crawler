using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracking : Effect
{
    public Tracking(int amount, Item obj)
    {
        this.amount = amount;
        this.name = "Tracking";
        if(obj is Weapon) {
            this.description = "Increases tracking by +" + amount + "%";
        } else if(obj is Armor) {
            this.name = "Movement Speed";
            this.description = "Increases movement speed by +" + amount + "%";
        }
    }
}