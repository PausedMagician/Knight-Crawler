using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    public int amount;
    public string name;
    public string description;
    public static Effect CreateEffect(ArmorEffector effector, int amount) {
        switch (effector) {
            case ArmorEffector.Damage:
                return new Damage(amount);
            case ArmorEffector.DamageIncrease:
                return new DamageIncrease(amount);
            case ArmorEffector.Defense:
                return new Defense(amount);
            case ArmorEffector.DamageReduction:
                return new DamageReduction(amount);
            case ArmorEffector.HealthBoost:
                return new HealthBoost(amount);
            case ArmorEffector.HealthRegen:
                return new HealthRegen(amount);
            case ArmorEffector.MovementSpeed:
                return new MovementSpeed(amount);
            default:
                return null;
        }
    }
}

public enum ArmorEffector
{
    Damage,
    DamageIncrease,
    Defense,
    DamageReduction,
    HealthBoost,
    HealthRegen,
    MovementSpeed,
}