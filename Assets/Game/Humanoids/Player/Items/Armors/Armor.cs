using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Armor : ItemData
{
    public int defense;
    public Effect[] effects = new Effect[0];
    public ArmorType armorType;
    public int cost;
    public int level;

    public string GetEffectsString() {
        string effectsString = "";
        foreach (Effect effect in effects) {
            effectsString += effect.ToString() + "\n";
        }
        return effectsString;
    }

    public override string ToString() {
        return name + "\n" + "\nCost: " + cost + "\nLevel: " + level + "\nRarity: " + rarity + "\nEffects: " + GetEffectsString();
    }

}

public enum ArmorType
{
    light,
    medium,
    heavy
}