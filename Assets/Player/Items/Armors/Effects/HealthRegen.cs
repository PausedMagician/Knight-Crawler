using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegen : Effect
{
    public HealthRegen(int amount)
    {
        this.amount = amount;
        this.name = "Health Regen";
        this.description = "Regenerates " + amount + " health per turn";
    }
}