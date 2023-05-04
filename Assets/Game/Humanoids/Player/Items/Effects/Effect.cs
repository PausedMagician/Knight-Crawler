using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect
{
    public int amount;
    public string name;
    public string description;
    public Effector specificType;
    public EffectType type;
    public AmountType amountType;
    public static Effect CreateEffect<T>(Effector effector, T obj, int amount, AmountType amountType) where T : ItemData {
        switch ((effector, obj)) {
            case (Effector.Damage, Weapon):
            case (Effector.Damage, Armor):
                return new Damage(amount, obj, amountType);
            case (Effector.HealthBoost, Armor):
                return new HealthBoost(amount, obj, amountType);
            case (Effector.HealthRegen, Melee):
            case (Effector.HealthRegen, Magic):
            case (Effector.HealthRegen, Armor):
                return new HealthRegen(amount, obj, amountType);
            case (Effector.Speed, Weapon):
            case (Effector.Speed, Armor):
                return new Speed(amount, obj);
            case (Effector.Tracking, Ranged):
            case (Effector.Tracking, Magic):
                return new Tracking(amount, obj, amountType);
            default:
                return null;
        }
    }

    public static List<Effect> CreateEffects<T>(int points, int maxEffectors, T obj) where T : ItemData {
        List<Effect> effects = new List<Effect>();
        int effectors = 0;
        while (points > 0 && effectors < maxEffectors) {
            int amount = Random.Range(1, points + 1);
            if(effectors == maxEffectors - 1) {
                amount = points;
            }
            // Debug.Log(amount);
            Effector effector = (Effector)Random.Range(0, System.Enum.GetNames(typeof(Effector)).Length);
            Effect effect = CreateEffect(effector, obj, amount, (AmountType)Random.Range(0, 2));
            if (effect != null) {
                points -= amount;
                effectors++;
                effects.Add(effect);
            }
        }
        return effects;
    }

    public override string ToString() {
        string extra = "";
        if(this.amountType == AmountType.Percentage) {
            extra += " %";
        }
        return name + ": +" + amount + extra;
    }

}

public enum Effector
{
    Damage,
    HealthBoost,
    HealthRegen,
    Speed,
    Tracking
}

public enum EffectType
{
    Damage,
    Buff
}

public enum AmountType
{
    Flat,
    Percentage
}