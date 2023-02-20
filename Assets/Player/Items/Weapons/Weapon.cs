using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item
{
    public int damage;
    public int maxCombo;
    int combo;
    public Sprite sprite;
    new public string name;
    public int cost;
    public int level;
    public int rarity;

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
