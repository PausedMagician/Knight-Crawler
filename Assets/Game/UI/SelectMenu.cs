using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectMenu : MonoBehaviour
{
    public Humanoid selected;
    public TextMeshProUGUI humanoidName;
    public TextMeshProUGUI health;
    public TextMeshProUGUI weapon;
    public TextMeshProUGUI armor;

    public void SetUp() {
        humanoidName.text = selected.Name.firstName + " " + selected.Name.surNames;
        health.text = $"{selected.health}/{selected.maxHealth}";
        if(selected.equippedWeapon) {
            weapon.text = selected.equippedWeapon.ToString();
        } else {
            weapon.text = "No Weapon Equipped";
        }
        if(selected.equippedArmor) {
            armor.text = selected.equippedArmor.ToString();
        } else {
            armor.text = "No Armor Equipped";
        }
    }

    public void NewWeapon() {
        int level;
        if(selected is AI2) {
            AI2 ai = selected as AI2;
            level = Random.Range((int)ai.levelRange.x, (int)ai.levelRange.y);
        } else {
            level = Random.Range(1, 100);
        }
        selected.EquipWeapon(GameController.GetInstance().CreateRanged((Rarity)Random.Range(0, 5), level));
        SetUp();
    }
    public void NewArmor() {
        int level;
        if(selected is AI2) {
            AI2 ai = selected as AI2;
            level = Random.Range((int)ai.levelRange.x, (int)ai.levelRange.y);
        } else {
            level = Random.Range(1, 100);
        }
        selected.EquipArmor(GameController.GetInstance().CreateArmor((Rarity)Random.Range(0, 5), level));
        SetUp();
    }

    public void Kill() {
        selected.health = 0;
        selected.Die();
        SetUp();
        Invoke("Close", 1f);
    }

    public void Close() {
        gameObject.SetActive(false);
    }
}
