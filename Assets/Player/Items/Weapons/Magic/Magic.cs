using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Magic", menuName = "Knight-Crawler/Items/Weapons", order = 1)]
public class Magic : Weapon
{
    public int manaConsumption;
    public Sprite projectileSprite;

    public Magic(string name, int cost, int level) : base(name, cost, level) {
        this.itemName = name;
        this.cost = cost;
        this.level = level;
    }
}