using System;
using UnityEngine;

public class RangedManifesto : MonoBehaviour {
    public WeaponManifesto weaponManifesto;
    public static Action updateManifesto;
    Type rangedEnabled = null;

    private void OnEnable() {
        updateManifesto += Refresh;
    }
    private void OnDisable() {
        updateManifesto -= Refresh;
    }

    void Refresh() {
        if(weaponManifesto.owner.equippedWeapon is Ranged) {
            rangedEnabled = typeof(Ranged);
        } else if (weaponManifesto.owner.equippedWeapon is Magic) {
            rangedEnabled = typeof(Magic);
        } else {
            rangedEnabled = null;
        }
    }

    void Shoot() {
        if(rangedEnabled == typeof(Ranged)) {
            Ranged weapon = weaponManifesto.owner.equippedWeapon as Ranged;
            GameObject shot = new GameObject("Shot");
            SpriteRenderer renderer = shot.AddComponent<SpriteRenderer>();
            renderer.sprite = weapon.projectileSprite;
            Rigidbody2D rb = shot.AddComponent<Rigidbody2D>();
            rb.velocity = weaponManifesto.transform.forward * 2;
        } else if(rangedEnabled == typeof(Magic)) {
            Magic weapon = weaponManifesto.owner.equippedWeapon as Magic;
            GameObject shot = new GameObject("Shot");
            SpriteRenderer renderer = shot.AddComponent<SpriteRenderer>();
            renderer.sprite = weapon.projectileSprite;
            Rigidbody2D rb = shot.AddComponent<Rigidbody2D>();
            rb.velocity = weaponManifesto.transform.forward * 2;
        }
    }
}