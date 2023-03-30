using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour
{
    public ItemData item;
    public Weapon weapon;
    public TextMeshProUGUI text;
    public Image image;

    public void SetItem(ItemData item) {
        if(item == null) {
            this.item = null;
            this.text.text = "";
            this.image.enabled = false;
            return;
        }
        this.item = item;
        this.text.text = item.itemName;
        this.image.sprite = item.sprite;
        this.image.enabled = true;
    }

    public void UseItem() {
        if(item is Weapon) {
            // Debug.Log("Equipping weapon");
            Inventory.GetInstance().EquipWeapon((Weapon)item);
        } else if(item is Armor) {
            Inventory.GetInstance().EquipArmor((Armor)item);
        }
    }

    public void RemoveItem() {
        Inventory.GetInstance().RemoveItem(item);
    }

    public void UnequipItem() {
        if(item is Weapon) {
            // Debug.Log("Unequipping weapon");
            Inventory.GetInstance().UnequipWeapon();
        } else if(item is Armor) {
            Inventory.GetInstance().UnequipArmor();
        }
    }

}
