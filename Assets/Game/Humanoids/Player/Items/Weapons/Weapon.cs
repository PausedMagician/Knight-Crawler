using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public Weapon(string name, int cost, int level)
    {
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

    public string GetEffectsString()
    {
        string effectsString = "";
        foreach (Effect effect in effects)
        {
            effectsString += effect.ToString() + "\n";
        }
        return effectsString;
    }

    public float GetAttackSpeed()
    {
        float attackSpeed = 1f;
        foreach (Effect effect in this.effects.Where(effect => effect.specificType == Effector.Speed).ToArray())
        {
            if (effect.amountType == AmountType.Flat)
            {
                attackSpeed += effect.amount;
            }
            else
            {
                attackSpeed *= (1 + ((float)effect.amount / 100));
            }
        }
        if(this is Ranged || this is Magic) {
            return 1 / attackSpeed;
        } else {
            return attackSpeed;
        }
    }

    public override string ToString()
    {
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