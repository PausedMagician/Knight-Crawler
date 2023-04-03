using UnityEngine;

public class WeaponManifesto : MonoBehaviour
{
    [Header("References")]
    public Humanoid owner;
    public GameObject container;
    public GameObject weaponPrefab;
    [SerializeField] SpriteRenderer spriteRenderer;
    public Animator animator;
    [Header("Weapon Stats")]
    // [SerializeField] int combo = 0;
    [SerializeField] int maxCombo = 3;
    [SerializeField] AnimationType animationType = AnimationType.melee;
    [SerializeField] AnimationSet animationSet = AnimationSet.light;

    bool canAttack = true;

    private void Start()
    {
        spriteRenderer = weaponPrefab.GetComponent<SpriteRenderer>();
        animator = weaponPrefab.GetComponent<Animator>();
    }

    public void Attack()
    {
        if (owner.equippedWeapon is Melee)
        {
            if (rebind)
            {
                animator.Rebind();
                animator.SetInteger("MaxCombo", maxCombo);
                animator.SetInteger("AnimationSet", (int)animationSet);
                animator.SetInteger("AnimationType", (int)animationType);
                rebind = false;
            }
            animator.SetTrigger("Attack");
        }   
        else if (owner.equippedWeapon is Ranged)
        {
            Ranged weapon = owner.equippedWeapon as Ranged;
            if(canAttack) {
                canAttack = false;
                // Instantiate and shoot projectile
                GameObject obj = new GameObject("Proejct", typeof(SpriteRenderer), typeof(Projectile));
                obj.transform.position = container.transform.position + ((container.transform.up + container.transform.right).normalized * 0.25f);
                Projectile projectile = obj.GetComponent<Projectile>();
                projectile.shooter = owner;
                projectile.speed = 5f;
                projectile.direction = (container.transform.up + container.transform.right).normalized;
                projectile.Rotate();
                // Debug.Log($"F: {container.transform.forward}");
                // Debug.DrawLine(transform.position, transform.position + container.transform.forward, Color.blue, 1f);
                // Debug.Log($"U: {container.transform.up}");
                // Debug.DrawLine(transform.position, transform.position + container.transform.up, Color.red, 1f);
                // Debug.Log($"R: {container.transform.right}");
                // Debug.DrawLine(transform.position, transform.position + container.transform.right, Color.green, 1f);
                // Debug.DrawLine(transform.position, transform.position + (container.transform.up + container.transform.right).normalized, Color.cyan, 1f);
                obj.gameObject.GetComponent<SpriteRenderer>().sprite = weapon.projectileSprite;
                Invoke("ResetAttack", weapon.GetAttackSpeed());
            }
    
        }
        else
        {

        }
    }

    void ResetAttack() {
        canAttack = true;
    }

    PolygonCollider2D coll;
    bool rebind;
    public void UpdateWeapon()
    {
        if (owner.equippedWeapon != null)
        {
            // Debug.Log("Weapon not null");
            spriteRenderer.sprite = owner.equippedWeapon.sprite;
            maxCombo = owner.equippedWeapon.maxCombo;
            animationSet = owner.equippedWeapon.animationSet;
            if (owner.equippedWeapon is Melee)
            {
                animationType = AnimationType.melee;
            }
            else if (owner.equippedWeapon is Ranged)
            {
                animationType = AnimationType.ranged;
            }
            else if (owner.equippedWeapon is Magic)
            {
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
        }
        else
        {
            // Debug.Log("Weapon null");
            spriteRenderer.sprite = null;
            // combo = 0;
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
        // RangedManifesto.updateManifesto();
    }
}