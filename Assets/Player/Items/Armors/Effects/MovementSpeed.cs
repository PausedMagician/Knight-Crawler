using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeed : Effect
{
    public MovementSpeed(int amount)
    {
        this.amount = amount;
        this.name = "Movement Speed";
        this.description = "Increases movement speed by " + amount + "%";
    }
}