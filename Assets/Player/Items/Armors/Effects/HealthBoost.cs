using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBoost : Effect
{
    public HealthBoost(int amount)
    {
        this.amount = amount;
        this.name = "Health Boost";
        this.description = "Increases max health by " + amount;
    }
}