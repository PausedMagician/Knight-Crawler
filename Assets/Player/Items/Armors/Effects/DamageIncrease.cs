using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIncrease : Effect
{
    public DamageIncrease(int amount)
    {
        this.amount = amount;
        this.name = "Damage Increase";
        this.description = "Increases damage by " + amount + "%";
    }
}