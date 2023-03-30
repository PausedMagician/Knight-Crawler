using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralUI : MonoBehaviour
{
    public Player owner;

    private void OnEnable() {
        owner.onDamage += UpdateUI;
    }
    private void OnDisable() {
        owner.onDamage -= UpdateUI;
    }

    void UpdateUI() {
        
    }
}
