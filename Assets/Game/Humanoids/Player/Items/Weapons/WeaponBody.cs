using UnityEngine;

public class WeaponBody : MonoBehaviour {
    WeaponManifesto weaponManifesto;
    private void Awake() {
        weaponManifesto = gameObject.GetComponentInParent<WeaponManifesto>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        Humanoid humanoid;
        if(humanoid = other.GetComponent<Humanoid>()) {
            weaponManifesto.owner.Heal(humanoid.TakeDamage(weaponManifesto.owner.equippedWeapon, weaponManifesto.owner));
        }
    }
}