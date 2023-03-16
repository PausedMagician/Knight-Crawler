using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item
{
    public List<Effect> effects = new List<Effect>();
    public ArmorType armorType;
    public int cost;
    public int level;
    public Rarity rarity;

    public string GetEffectsString() {
        string effectsString = "";
        foreach (Effect effect in effects) {
            effectsString += effect.name + ": " + effect.amount + "\n";
        }
        return effectsString;
    }

}

public enum ArmorType
{
    light,
    medium,
    heavy
}