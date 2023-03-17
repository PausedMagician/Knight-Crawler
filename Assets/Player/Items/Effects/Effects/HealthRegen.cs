using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegen : Effect
{
    public HealthRegen(int amount, Item obj)
    {
        this.amount = amount;
        if(obj is Weapon) {
            this.name = "Vampirism";
            this.description = "Increases health gained on hit by +" + amount;
        } else if(obj is Armor) {
            this.name = "Health Regen";
            this.description = "Increases health regeneration by +" + amount + " per health tick";
        }
    }
}