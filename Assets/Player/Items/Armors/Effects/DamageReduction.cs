using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReduction : Effect
{
    public DamageReduction(int amount)
    {
        this.amount = amount;
        this.name = "Damage Reduction";
        this.description = "Reduces incoming damage by " + amount + "%";
    }
}