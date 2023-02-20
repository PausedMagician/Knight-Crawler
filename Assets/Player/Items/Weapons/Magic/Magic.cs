using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Magic : Weapon
{
    public int manaConsumption;

    public Magic(string name, int damage, int cost, int level) : base(name, damage, cost, level) {
        this.name = name;
        this.damage = damage;
        this.cost = cost;
        this.level = level;
    }
}