using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Humanoid : MonoBehaviour
{
    [Header("Main Settings")]
    public string Name = "Humanoid";
    public int hearts = 3;
    public int maxHealth = 100;
    public int health = 100;
    public int regenSpeed = 2;
    public float movementSpeed = 5f;
    public float sprintspeed = 1.3f;
    public float dodgeMultiplier = 10f;
    public float dodgeCooldown = 1f;
    [Header("Script Variables")]
    public bool dodging = false, sprinting = false;
    public Rigidbody2D rb;
    public Weapon equippedWeapon;
    public WeaponManifesto weaponManifesto;
    public Armor equippedArmor;
    public Vector2 movementDirection = new Vector2(0, 0);
    public float dodgeTimer = 0f;
    public List<Humanoid> targetedBy = new List<Humanoid>();
    public UnityAction onDamage;


    private void OnEnable() {
        GameController.Tick += Regen;
    }
    private void OnDisable() {
        GameController.Tick -= Regen;
    }

    private void OnValidate() {
        if(!weaponManifesto) {
            // GameObject gam = Instantiate(Resources.Load<GameObject>("Prefabs/WeaponManifesto"), transform);
            // weaponManifesto = gam.GetComponent<WeaponManifesto>();
            // weaponManifesto.owner = this;
        }
        Start();
    }

    public void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        weaponManifesto = GetComponentInChildren<WeaponManifesto>();
        if(!weaponManifesto) {
            GameObject gam = Instantiate(Resources.Load<GameObject>("Prefabs/WeaponManifesto"), transform);
            weaponManifesto = gam.GetComponent<WeaponManifesto>();
            weaponManifesto.owner = this;
        }
        health = maxHealth;
    }

    public void FixedUpdate() {
        Move();
        if (dodgeTimer > 0) {
            dodgeTimer -= Time.fixedDeltaTime;
        }
    }

    void Move() {
        if(weaponManifesto.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f) {
            if (sprinting) {
                rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * sprintspeed * Time.fixedDeltaTime);
                sprinting = false;
            }else if (dodging) {
                dodgeTimer = dodgeCooldown;
                dodging = false;
            } else if (dodgeTimer > 0.25f) {
                rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * Time.fixedDeltaTime * dodgeMultiplier);
            } else {
                rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * Time.fixedDeltaTime);
            }
        } else {
            movementDirection = Vector2.zero;
        }
    }

    protected void TurnWeapon(Vector2 _transform, Vector2 target, float _time, float extraDegrees = 0f) {
        if(target == Vector2.zero) {
            return;
        }
        Vector2 direction = target - _transform;

        Quaternion rotation = Quaternion.Slerp(weaponManifesto.transform.rotation, Quaternion.LookRotation(Vector3.forward, direction), 0.1f * _time * 100);
        // Debug.Log(rotation);
        weaponManifesto.transform.rotation = rotation;
        Quaternion rotation2 = Quaternion.Slerp(weaponManifesto.container.transform.rotation, Quaternion.LookRotation(Vector3.forward, (target - (Vector2)weaponManifesto.container.transform.position)), 0.1f * _time * 100);
        // Debug.Log(rotation2);
        weaponManifesto.container.transform.rotation = rotation2;
    }

    protected virtual void Attack() {
        if(equippedWeapon != null) {
            weaponManifesto.Attack();
        }
    }

    public int TakeDamage(Weapon weapon, Humanoid attacker) {
        if(dodgeTimer > 0f || health == 0) {
            return 0;
        }
        onDamage?.Invoke();
        this.health -= GameController.CalculateDamage(weapon, equippedArmor, out int heal);
        if(health <= 0) {
            health = 0;
            Die();
        }
        if(this is AI2) {
            AI2 ai = this as AI2;
            ai.target = attacker;
        }
        return heal;
    }

    public void Heal(int heal) {
        this.health += heal;
        if(health > maxHealth) {
            health = maxHealth;
        }
        onDamage?.Invoke();
    }

    public void Regen() {
        if(targetedBy.Count > 0) {
            return;
        }
        if(health == maxHealth) {
            return;
        }
        if(!equippedArmor) {
            health += regenSpeed;
            return;
        }
        Effect effect;
        float toRegen = regenSpeed;
        for (int i = 0; i < equippedArmor.effects.Count; i++)
        {
            effect = equippedArmor.effects[i];
            if(effect.specificType == Effector.HealthRegen) {
                if(effect.amountType == AmountType.Percentage) {
                    toRegen *= (1f + ((float)effect.amount / 100));
                } else {
                    toRegen += regenSpeed + effect.amount;
                }
            }
        }
        health += Mathf.RoundToInt(toRegen);
        if(health > maxHealth) {
            health = maxHealth;
        }
    }

    public virtual void Die() {
        if (hearts <= 0 )
        {
            Debug.LogWarning("Dead");
        }
    }
    
    public Weapon EquipWeapon(Weapon weapon) {
        Weapon tempWeapon = equippedWeapon;
        this.equippedWeapon = weapon;
        this.weaponManifesto.UpdateWeapon();
        return tempWeapon;
    }

    public Weapon UnEquipWeapon() {
        Weapon tempWeapon = equippedWeapon;
        this.equippedWeapon = null;
        this.weaponManifesto.UpdateWeapon();
        return tempWeapon;
    }

    public Armor EquipArmor(Armor armor) {
        Armor tempArmor = equippedArmor;
        this.equippedArmor = armor;
        return tempArmor;
    }

    public Armor UnEquipArmor() {
        Armor tempArmor = equippedArmor;
        this.equippedArmor = null;
        return tempArmor;
    }


    public override string ToString() {
        return $"Name: {Name}, Movement Speed: {movementSpeed}, Sprint Speed: {sprintspeed}, Dodge Multiplier: {dodgeMultiplier}, Hearts: {hearts}\n Equipped Weapon: {equippedWeapon}, Equipped Armor: {equippedArmor}";
    }


    private void OnMouseDown() {
        if(DebugMenu.selecting) {
            DebugMenu.GetInstance().OpenSelectMenu(this);
        }
    }


}
