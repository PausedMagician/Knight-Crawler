using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item
{
    public int damage;
    public int maxCombo;
    int combo;
    public int cost;
    public int level;
    public Rarity rarity;
    public AnimationSet animationSet;
    

    public Weapon(string name, int damage, int cost, int level) {
        this.name = name;
        this.damage = damage;
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