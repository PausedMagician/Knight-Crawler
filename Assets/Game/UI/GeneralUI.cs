using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneralUI : MonoBehaviour
{
    public Player owner;

    int prevHealth = 100;
    public Image healthMask;
    public TextMeshProUGUI healthText;


    private void OnEnable() {
        owner.onDamage += UpdateUI;
        GameController.Tick += UpdateHealthChange;
        GameController.Tick += UpdateUI;
    }
    private void OnDisable() {
        owner.onDamage -= UpdateUI;
        GameController.Tick -= UpdateHealthChange;
        GameController.Tick -= UpdateUI;
    }

    private void Awake() {
        UpdateUI();
    }

    string healthChange = "";

    void UpdateUI() {
        healthMask.fillAmount = (float)owner.health/(float)owner.maxHealth;
        healthText.text = $"{owner.health}/{owner.maxHealth}{healthChange}";
        prevHealth = owner.health;
    }
    void UpdateHealthChange() {
        healthChange = "";
        if(owner.health != prevHealth) {
            int healthChangeN = owner.health - prevHealth;
            string extra = "+";
            if(healthChangeN < 0) {
                extra = "";
            }
            healthChange = $" {extra} {owner.health - prevHealth}";
        }
    }
}
