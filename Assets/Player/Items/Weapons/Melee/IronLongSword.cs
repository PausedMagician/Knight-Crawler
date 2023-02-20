using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronLongSword : Melee
{
    public IronLongSword() : base("Iron Long Sword", 10, 10, 1) {
        this.name = "Iron Long Sword";
        this.damage = 10;
        this.cost = 10;
        this.level = 1;
    }
}