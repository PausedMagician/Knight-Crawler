using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ranged", menuName = "Knight-Crawler/Items/Weapons", order = 0)]
public class Ranged : Weapon
{
    public float range;
    public Sprite projectileSprite;
    
    public Ranged(string name, int cost, int level) : base(name, cost, level) {
        this.itemName = name;
        this.cost = cost;
        this.level = level;
    }
}