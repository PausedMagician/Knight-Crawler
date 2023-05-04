using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tracking : Effect
{
    public Tracking(int amount, ItemData obj, AmountType amountType)
    {
        this.specificType = Effector.Tracking;
        this.type = EffectType.Buff;
        this.amountType = amountType;
        this.amount = amount;
        string extra = "";
        if(amountType == AmountType.Percentage) {
            extra += " %";
        }
        this.name = "Tracking";
        if(obj is Weapon) {
            this.description = "Increases tracking by +" + amount + extra;
        } else if(obj is Armor) {
            this.name = "Movement Speed";
            this.description = "Increases movement speed by +" + amount + extra;
        }
    }
}