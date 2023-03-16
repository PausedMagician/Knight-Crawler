using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : Effect
{
    public Damage(int amount)
    {
        this.amount = amount;
        this.name = "Damage";
        this.description = "Increases damage by " + amount;
    }
}