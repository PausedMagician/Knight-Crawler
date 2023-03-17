using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBoost : Effect
{
    public HealthBoost(int amount, Item obj)
    {
        this.amount = amount;
        this.name = "Max Health";
        if(obj is Weapon) {
            this.description = "Increases health by +" + amount;
        } else if(obj is Armor) {
            this.description = "Increases max health by +" + amount;
        }
    }
}