using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    public int amount;
    public string name;
    public string description;
    public static Effect CreateEffect<T>(Effector effector, T obj, int amount) where T : Item {
        switch ((effector, obj)) {
            case (Effector.Damage, Weapon):
            case (Effector.Damage, Armor):
                return new Damage(amount, obj);
            case (Effector.DamagePercentage, Weapon):
            case (Effector.DamagePercentage, Armor):
                return new DamagePercentage(amount, obj);
            case (Effector.HealthBoost, Armor):
                return new HealthBoost(amount, obj);
            case (Effector.HealthRegen, Melee):
            case (Effector.HealthRegen, Magic):
            case (Effector.HealthRegen, Armor):
                return new HealthRegen(amount, obj);
            case (Effector.Speed, Weapon):
            case (Effector.Speed, Armor):
                return new Speed(amount, obj);
            case (Effector.Tracking, Ranged):
            case (Effector.Tracking, Magic):
                return new Tracking(amount, obj);
            default:
                return null;
        }
    }

    public static List<Effect> CreateEffects<T>(int points, int maxEffectors, T obj) where T : Item {
        List<Effect> effects = new List<Effect>();
        int effectors = 0;
        while (points > 0 && effectors < maxEffectors) {
            int amount = Random.Range(1, points + 1);
            if(effectors == maxEffectors - 1) {
                amount = points;
            }
            Debug.Log(amount);
            Effector effector = (Effector)Random.Range(0, System.Enum.GetNames(typeof(Effector)).Length);
            Effect effect = CreateEffect(effector, obj, amount);
            if (effect != null) {
                points -= amount;
                effectors++;
                effects.Add(effect);
            }
        }
        return effects;
    }

    public override string ToString() {
        return name + ": " + amount;
    }

}

public enum Effector
{
    Damage,
    DamagePercentage,
    HealthBoost,
    HealthRegen,
    Speed,
    Tracking
}