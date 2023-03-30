using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : Effect
{
    public Damage(int amount, Item obj, AmountType amountType)
    {
        this.specificType = Effector.Damage;
        this.type = EffectType.Damage;
        this.amountType = amountType;
        this.amount = amount;
        string extra = "";
        if(amountType == AmountType.Percentage) {
            extra += " %";
        }
        if(obj is Weapon) {
            this.name = "Damage";
            this.description = "Increases damage by +" + amount + extra;
        } else if(obj is Armor) {
            this.name = "Defense";
            this.description = "Decreases damage taken by +" + amount + extra;
        }
    }
}