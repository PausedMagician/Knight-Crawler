using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    #region Singleton
    public static TooltipManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public static TooltipManager GetInstance()
    {
        if (Instance == null)
            Debug.LogWarning("TooltipManager instance is null!\nAttempting to find one...");
        Instance = FindObjectOfType<TooltipManager>();
        return Instance;
    }
    #endregion

    public RectTransform tooltip;

    public static Action<Item> OnMouseHover;
    public static Action OnMouseExit;

    private void OnEnable() {
        OnMouseHover += ShowTooltip;
        OnMouseExit += HideTooltip;
    }
    private void OnDisable() {
        OnMouseHover -= ShowTooltip;
        OnMouseExit -= HideTooltip;
    }


    private void Update() {
        if(tooltip.gameObject.activeInHierarchy) {
            if(Input.mousePosition.x + tooltip.rect.width > Screen.width) {
                tooltip.position = new Vector2(Screen.width - tooltip.rect.width, Input.mousePosition.y);
            } else if(Input.mousePosition.y - tooltip.rect.height*2 < 0) {
                tooltip.position = new Vector2(Input.mousePosition.x, tooltip.rect.height*2);
            } else {
                tooltip.position = Input.mousePosition;
            }
        }
    }

    public void ShowTooltip(Item item)
    {
        if (item is Weapon)
        {
            ShowWeaponTooltip((Weapon)item);
        }
        else if (item is Armor)
        {
            ShowArmorTooltip((Armor)item);
        }
        tooltip.gameObject.SetActive(true);
    }

    public void ShowWeaponTooltip(Weapon weapon)
    {
        // Debug.Log("Showing weapon tooltip");
        tooltip.Find("Name").GetComponent<TextMeshProUGUI>().text = weapon.itemName;
        // tooltip.Find("Description").GetComponent<TextMeshProUGUI>().text = weapon.description;
        string finalString = "";
        finalString += "Level: " + weapon.level + "\n";
        finalString += "Rarity: " + weapon.rarity + "\n";
        finalString += "Type: " + weapon.animationSet.ToString() + "\n";
        finalString += weapon.GetEffectsString() + "\n";
        finalString += "Max Combo: " + weapon.maxCombo + "\n";
        finalString += "Value: " + weapon.cost + "\n";
        tooltip.Find("Stats").GetComponent<TextMeshProUGUI>().text = finalString;

    }

    public void ShowArmorTooltip(Armor armor)
    {
        // Debug.Log("Showing armor tooltip");
        tooltip.Find("Name").GetComponent<TextMeshProUGUI>().text = armor.itemName;
        // tooltip.Find("Description").GetComponent<TextMeshProUGUI>().text = armor.description;
        string finalString = "";
        finalString += "Level: " + armor.level + "\n";
        finalString += "Rarity: " + armor.rarity + "\n";
        finalString += "Type: " + armor.armorType.ToString() + "\n";
        finalString += armor.GetEffectsString() + "\n";
        finalString += "Value: " + armor.cost + "\n";
        tooltip.Find("Stats").GetComponent<TextMeshProUGUI>().text = finalString;
    }

    public void HideTooltip()
    {
        // Debug.Log("Hiding tooltip");
        tooltip.gameObject.SetActive(false);
    }
}