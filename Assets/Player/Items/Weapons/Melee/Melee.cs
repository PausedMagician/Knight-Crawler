using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee", menuName = "Knight-Crawler/Items/Weapons", order = 0)]
public class Melee : Weapon
{
    public Melee(string name, int cost, int level) : base(name, cost, level) {
        this.itemName = name;
        this.cost = cost;
        this.level = level;
    }
}