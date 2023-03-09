using UnityEngine;

public class WeaponManifesto : MonoBehaviour {
    [Header("References")]
    public Player player;
    public GameObject weaponPrefab;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [Header("Weapon Stats")]
    [SerializeField] int combo = 0;
    [SerializeField] int maxCombo = 3;
    [SerializeField] string animationType = "";
    [SerializeField] AnimationSet animationSet;

    private void Start() {
        spriteRenderer = weaponPrefab.GetComponent<SpriteRenderer>();
        animator = weaponPrefab.GetComponent<Animator>();
    }

    public void Attack() {
        animator.PlayInFixedTime(string.Format("{0} {1} Attack {2}", animationType, animationSet, combo), -1, 0f);
    }

    public void UpdateWeapon() {
        spriteRenderer.sprite = player.equippedWeapon.sprite;
        maxCombo = player.equippedWeapon.maxCombo;
        animationSet = player.equippedWeapon.animationSet;
        if(player.equippedWeapon is Melee) {
            animationType = "Melee";
        } else if(player.equippedWeapon is Ranged) {
            animationType = "Ranged";
        } else if(player.equippedWeapon is Magic) {
            animationType = "Magic";
        }
        Debug.Log(string.Format("{0} {1} Attack {2}", animationType, animationSet, combo));
    }
}