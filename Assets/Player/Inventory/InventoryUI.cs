using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    #region Inventory Singleton

    public static InventoryUI instance;
    void Awake() {
        if(instance != null) {
            Debug.LogWarning("More than one instance of InventoryUI found!");
            return;
        }
        instance = this;
    }

    public static InventoryUI GetInstance() {
        if(instance == null)
            Debug.LogWarning("Inventory instance is null!\nAttempting to find one...");
            instance = FindObjectOfType<InventoryUI>();
        return instance;
    }

    #endregion

    Inventory inventory;
    Canvas canvas;
    public Transform itemsParent;

    public Transform equippedWeaponParent;
    public Transform equippedArmorParent;

    public static Action OnInventoryChanged;

    void Start()
    {
        this.inventory = Inventory.GetInstance();
        this.canvas = this.GetComponentInChildren<Canvas>();
    }

    private void OnEnable() {
        OnInventoryChanged += UpdateUI;
    }
    private void OnDisable() {
        OnInventoryChanged -= UpdateUI;
    }

    void DebugList(List<Item> list, string name = "List") {
        Debug.Log(name + string.Join(", ", list.Select(item => item.itemName).ToArray()));
    }

    void UpdateUI() {
        // Debug.Log("Updating UI");
        this.inventory = Inventory.GetInstance();
        // List.except method for items that are in the inventory but not in the UI.

        // List.intersect method for items that are in the UI but not in the inventory.

        // List.union method for items that are in the UI and in the inventory.
        List<Item> diff = new List<Item>();
        List<Item> existing = this.itemsParent.GetComponentsInChildren<ItemUI>().Select(itemUI => itemUI.item).ToList();
        
        // DebugList(existing, "Existing: ");
        // DebugList(this.inventory.items, "Inventory: ");

        this.inventory.items.ForEach(item => {
            // Debug.Log($"Name {item.itemName}");
            if(!existing.Contains(item)) {
                // Debug.Log($"Didn't have item already.");
                diff.Add(item);
            }
        });
        existing.ForEach(item => {
            if(!this.inventory.items.Contains(item)) {
                // Find the ItemUI gameObject in itemsParent that has the item.
                this.itemsParent.GetComponentsInChildren<ItemUI>().ToList().ForEach(itemUI => {
                    if(itemUI.item == item) {
                        Destroy(itemUI.gameObject);
                    }
                });
            }
        });
        
        // DebugList(diff, "Diff: ");

        for (int i = 0; i < diff.Count; i++)
        {
            GameObject gO = Instantiate(Resources.Load<GameObject>("Prefabs/Item"), this.itemsParent);
            gO.GetComponent<ItemUI>().SetItem(diff[i]);
        }

        // Update equipped weapon and armor.
        // Update equipped weapon.
        if(Player.GetInstance().equippedWeapon) {
            this.equippedWeaponParent.GetComponentInChildren<ItemUI>().SetItem(Player.GetInstance().equippedWeapon);
            this.equippedWeaponParent.Find("Placeholder").GetComponent<Image>().enabled = false;
        } else {
            this.equippedWeaponParent.GetComponentInChildren<ItemUI>().SetItem(null);
            this.equippedWeaponParent.Find("Placeholder").GetComponent<Image>().enabled = true;
        }
        // Update equipped armor.
        if(Player.GetInstance().equippedArmor) {
            this.equippedArmorParent.GetComponentInChildren<ItemUI>().SetItem(Player.GetInstance().equippedArmor);
            this.equippedArmorParent.Find("Placeholder").GetComponent<Image>().enabled = false;
        } else {
            this.equippedArmorParent.GetComponentInChildren<ItemUI>().SetItem(null);
            this.equippedArmorParent.Find("Placeholder").GetComponent<Image>().enabled = true;
        }

    }

    public void ToggleInventory() {
        // Debug.Log("Opening Inventory");
        if(!this.canvas) {
            Debug.LogWarning("No canvas found!\nAttempting to find one...");
            this.canvas = this.GetComponentInChildren<Canvas>();
            return;
        }
        if(this.canvas.enabled) {
            this.canvas.enabled = false;
            Time.timeScale = 1f;
            return;
        }
        this.canvas.enabled = true;
        Time.timeScale = 0f;
        return;
    }
}
