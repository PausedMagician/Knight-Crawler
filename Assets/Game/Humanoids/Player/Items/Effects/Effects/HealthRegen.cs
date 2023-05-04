using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealthRegen : Effect
{
    public HealthRegen(int amount, ItemData obj, AmountType amountType)
    {
        this.specificType = Effector.HealthRegen;
        this.type = EffectType.Buff;
        this.amountType = amountType;
        this.amount = amount;
        string extra = "";
        if(amountType == AmountType.Percentage) {
            extra += " %";
        }
        if(obj is Weapon) {
            this.name = "Vampirism";
            this.description = "Increases health gained on hit by +" + amount + extra;
        } else if(obj is Armor) {
            this.name = "Health Regen";
            this.description = "Increases health regeneration by +" + amount + extra + " per health tick";
        }
    }
}