using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Inventory Singleton

    public static Inventory instance;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }

    public static Inventory GetInstance()
    {
        if (instance == null)
            Debug.LogWarning("Inventory instance is null!\nAttempting to find one...");
        instance = FindObjectOfType<Inventory>();
        return instance;
    }

    #endregion


    public Player player;
    public List<Item> items = new List<Item>();

    private void Start()
    {

    }

    //EquipWeapon function
    public Weapon EquipWeapon(Weapon weapon)
    {
        Weapon tempWeapon = player.EquipWeapon(weapon);
        if (tempWeapon != null)
        {
            items.Add(tempWeapon);
        }
        RemoveItem(weapon);
        return weapon;
    }

    public void UnequipWeapon()
    {
        AddItem(player.UnEquipWeapon());
    }

    //EquipArmor function
    public Armor EquipArmor(Armor armor)
    {
        Armor tempArmor = player.EquipArmor(armor);
        if (tempArmor != null)
        {
            items.Add(tempArmor);
        }
        RemoveItem(armor);
        return armor;
    }

    public void UnequipArmor()
    {
        AddItem(player.UnEquipArmor());
    }

    //AddItem function
    public void AddItem(Item item)
    {
        items.Add(item);
        InventoryUI.OnInventoryChanged();
    }
    //AddItems function
    public void AddItems(Item[] items)
    {
        this.items.AddRange(items);
        InventoryUI.OnInventoryChanged();
    }
    //RemoveItem function
    public void RemoveItem(Item item)
    {
        items.Remove(item);
        InventoryUI.OnInventoryChanged();
    }
    //RemoveItems function
    public void RemoveItems(Item[] items)
    {
        foreach (Item item in items)
        {
            this.items.Remove(item);
        }
        InventoryUI.OnInventoryChanged();
    }

    void CalculateStats()
    {

    }

}