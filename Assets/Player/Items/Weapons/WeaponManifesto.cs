using UnityEngine;

public class WeaponManifesto : MonoBehaviour {
    [Header("References")]
    public Player player;
    public GameObject container;
    public GameObject weaponPrefab;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [Header("Weapon Stats")]
    [SerializeField] int combo = 0;
    [SerializeField] int maxCombo = 3;
    [SerializeField] AnimationType animationType = AnimationType.melee;
    [SerializeField] AnimationSet animationSet = AnimationSet.light;

    private void Start() {
        spriteRenderer = weaponPrefab.GetComponent<SpriteRenderer>();
        animator = weaponPrefab.GetComponent<Animator>();
    }

    public void Attack() {
        animator.enabled = true;
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        animator.SetTrigger("Attack");
        Debug.Log(string.Format("{0} {1} Attack {2}", animationType, animationSet, combo));
    }

    public void UpdateWeapon() {
        if(player.equippedWeapon != null) {
            spriteRenderer.sprite = player.equippedWeapon.sprite;
            maxCombo = player.equippedWeapon.maxCombo;
            animationSet = player.equippedWeapon.animationSet;
            if(player.equippedWeapon is Melee) {
                animationType = AnimationType.melee;
            } else if(player.equippedWeapon is Ranged) {
                animationType = AnimationType.ranged;
            } else if(player.equippedWeapon is Magic) {
                animationType = AnimationType.magic;
            }
            animator.SetInteger("MaxCombo", maxCombo);
            animator.SetInteger("AnimationSet", (int)animationSet);
            animator.SetInteger("AnimationType", (int)animationType);
            
            PolygonCollider2D coll;
            if(coll = weaponPrefab.GetComponent<PolygonCollider2D>()) {
                Destroy(coll);
                coll = weaponPrefab.AddComponent<PolygonCollider2D>();
                coll.enabled = false;
            } else {
                coll = weaponPrefab.AddComponent<PolygonCollider2D>();
                coll.enabled = false;
            }
            coll = null;
            Debug.Log(string.Format("{0} {1} Attack {2}", animationType, animationSet, combo));
        } else {
            spriteRenderer.sprite = null;
            combo = 0;
            maxCombo = 3;
            animationType = AnimationType.melee;
            PolygonCollider2D coll;
            if(coll = weaponPrefab.GetComponent<PolygonCollider2D>()) {
                Destroy(coll);
                coll = weaponPrefab.AddComponent<PolygonCollider2D>();
                coll.enabled = false;
            }
            coll = null;
        }
        animator.Rebind();
    }
}