using UnityEngine;

public class WeaponManifesto : MonoBehaviour {
    public Player player;
    
    public SpriteRenderer spriteRenderer;

    public void Attack() {
        
    }

    public void UpdateWeapon() {
        spriteRenderer.sprite = player.equippedWeapon.sprite;
    }
}