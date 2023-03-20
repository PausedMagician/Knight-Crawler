using UnityEngine;

public class WeaponManifesto : MonoBehaviour {
    [Header("References")]
    public Player player;
    public GameObject container;
    public GameObject weaponPrefab;
    [SerializeField] SpriteRenderer spriteRenderer;
    public Animator animator;
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
        if(rebind) {
            animator.Rebind();
            animator.SetInteger("MaxCombo", maxCombo);
            animator.SetInteger("AnimationSet", (int)animationSet);
            animator.SetInteger("AnimationType", (int)animationType);
            rebind = false;
        }
        animator.SetTrigger("Attack");
    }

    PolygonCollider2D coll;
    bool rebind;
    public void UpdateWeapon() {
        if(player.equippedWeapon != null) {
            // Debug.Log("Weapon not null");
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
            foreach (PolygonCollider2D collider2D in weaponPrefab.GetComponents<PolygonCollider2D>())
            {
                Destroy(collider2D);
            }
            coll = weaponPrefab.AddComponent<PolygonCollider2D>();
        } else {
            // Debug.Log("Weapon null");
            spriteRenderer.sprite = null;
            combo = 0;
            maxCombo = 3;
            animationType = AnimationType.melee;
            foreach (PolygonCollider2D collider2D in weaponPrefab.GetComponents<PolygonCollider2D>())
            {
                Destroy(collider2D);
            }
            coll = weaponPrefab.AddComponent<PolygonCollider2D>();
        }
        coll.enabled = false;
        coll.isTrigger = true;
        rebind = true;
    }
}