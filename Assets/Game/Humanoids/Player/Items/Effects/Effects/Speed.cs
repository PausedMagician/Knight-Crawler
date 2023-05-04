using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Speed : Effect
{
    public Speed(int amount, ItemData obj)
    {
        AmountType amountType = AmountType.Percentage;
        this.specificType = Effector.Speed;
        this.type = EffectType.Buff;
        this.amountType = amountType;
        this.amount = amount;
        string extra = "";
        if(amountType == AmountType.Percentage) {
            extra += " %";
        }
        if(obj is Weapon) {
            this.name = "Attack Speed";
            this.description = "Increases attack speed by +" + amount + extra;
        } else if(obj is Armor) {
            this.name = "Movement Speed";
            this.description = "Increases movement speed by +" + amount + extra;
        }
    }
}