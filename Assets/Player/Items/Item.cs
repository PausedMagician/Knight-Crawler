using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    public string itemName;
    public Sprite sprite;

    public static string ToDebugString(Item[] items)
    {
        string s = "";
        foreach (Item item in items)
        {
            if (item is Weapon)
            {
                Weapon weapon = item as Weapon;
                s += "Name: " + weapon.itemName + " ";
                s += "Level: " + weapon.level + " ";
                s += "Rarity: " + weapon.rarity.ToString() + " ";
                s += "Type: " + weapon.animationSet.ToString() + " ";
                s += weapon.GetEffectsString() + " ";
                s += "Max Combo: " + weapon.maxCombo + " ";
                s += "Value: " + weapon.cost + "\n";
                Debug.Log(s);
            }
            else if (item is Armor)
            {
                Armor armor = item as Armor;
                s += "Name: " + armor.itemName + " ";
                s += "Level: " + armor.level + " ";
                s += "Rarity: " + armor.rarity.ToString() + " ";
                s += "Type: " + armor.armorType.ToString() + " ";
                s += armor.GetEffectsString() + " ";
                s += "Value: " + armor.cost + "\n";
                Debug.Log(s);
            }
            else
            {
                s += item.itemName + "\n";
                Debug.Log(s);
            }
        }
        return s;
    }
    public static string ToDebugStringShort(Item[] items)
    {
        string s = "";
        foreach (Item item in items)
        {
            if (item is Weapon)
            {
                Weapon weapon = item as Weapon;
                s += "Name: " + weapon.itemName + " ";
                s += "Lvl: " + weapon.level + "\n";
                Debug.Log(s);
            }
            else if (item is Armor)
            {
                Armor armor = item as Armor;
                s += "Name: " + armor.itemName + " ";
                s += "Lvl: " + armor.level + "\n";
                Debug.Log(s);
            }
            else
            {
                s += item.itemName + "\n";
                Debug.Log(s);
            }
        }
        return s;
    }

}
