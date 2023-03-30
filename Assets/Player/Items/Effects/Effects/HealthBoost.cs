using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBoost : Effect
{
    public HealthBoost(int amount, ItemData obj, AmountType amountType)
    {
        this.specificType = Effector.HealthBoost;
        this.type = EffectType.Buff;
        this.amountType = amountType;
        this.amount = amount;
        string extra = "";
        if(amountType == AmountType.Percentage) {
            extra += " %";
        }
        if(obj is Weapon) {
            this.name = "Health";
            this.description = "Increases health by +" + amount + extra;
        } else if(obj is Armor) {
            this.name = "Health";
            this.description = "Increases max health by +" + amount + extra;
        }

    }
}