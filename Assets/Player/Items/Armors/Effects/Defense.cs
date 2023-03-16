using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : Effect
{
    public Defense(int amount)
    {
        this.amount = amount;
        this.name = "Defense";
        this.description = "Reduces incoming damage by " + amount;
    }
}