using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Weapon : ItemData
{
    public int damage;
    public int heal;
    public int maxCombo;
    int combo;
    public int cost;
    public int level;
    public AnimationSet animationSet;
    public Effect[] effects;

    public Weapon(string name, int cost, int level) {
        this.name = name;
        this.cost = cost;
        this.level = level;
    }

    public void Attack()
    {

        if (combo < maxCombo)
        {
            combo++;
        }
        else
        {
            combo = 0;
        }
    }

    public string GetEffectsString() {
        string effectsString = "";
        foreach (Effect effect in effects) {
            effectsString += effect.ToString() + "\n";
        }
        return effectsString;
    }

    public float GetAttackSpeed() {
        float attackSpeed = 0.5f;
        for (var i = 0; i < effects.Length; i++)
        {
            Effect effect = effects[i];
            if(effect.specificType == Effector.Speed) {
                if(effect.amountType == AmountType.Flat) {
                    attackSpeed += effect.amount;
                } else {
                    attackSpeed *= (1 + ((float)effect.amount / 100));
                }
            }
        }
        return 1/attackSpeed;
    }

    public override string ToString() {
        return name + "\n" + "\nCost: " + cost + "\nLevel: " + level + "\nRarity: " + rarity + "\nEffects: " + GetEffectsString();
    }


}

public enum AnimationSet
{
    light,
    medium,
    heavy
}

public enum AnimationType
{
    melee,
    ranged,
    magic
}