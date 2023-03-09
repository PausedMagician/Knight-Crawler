using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Player player;
    public List<Item> items = new List<Item>();


    private void Start() {

    }

    //EquipWeapon function
    public Weapon EquipWeapon(Weapon weapon) {
        Weapon tempWeapon = player.EquipWeapon(weapon);
        if(tempWeapon != null) {
            items.Add(tempWeapon);
        }
        RemoveItem(weapon);
        return weapon;
    }

    //EquipArmor function
    public Armor EquipArmor(Armor armor) {
        player.EquipArmor(armor);
        return armor;
    }

    //AddItem function
    public void AddItem(Item item) {
        items.Add(item);
    }

    //RemoveItem function
    public void RemoveItem(Item item) {
        items.Remove(item);
    }

}