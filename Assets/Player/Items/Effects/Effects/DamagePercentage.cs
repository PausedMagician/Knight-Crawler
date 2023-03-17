using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePercentage : Effect
{
    public DamagePercentage(int amount, Item obj)
    {
        this.amount = amount;
        if(obj is Weapon) {
            this.name = "Damage %";
            this.description = "Increases damage by +" + amount + "%";
        } else if(obj is Armor) {
            this.name = "Defense %";
            this.description = "Decreases damage taken by +" + amount + "%";
        }
    }
}