using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : ItemData
{
    public int damage;
    public int maxCombo;
    int combo;
    public int cost;
    public int level;
    public Rarity rarity;
    public AnimationSet animationSet;
    public List<Effect> effects;

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

public enum Rarity {
    common,
    uncommon,
    rare,
    epic,
    legendary
}