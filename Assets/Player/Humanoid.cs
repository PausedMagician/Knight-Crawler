using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Humanoid : MonoBehaviour
{
    [Header("Main Settings")]
    public string name = "Humanoid";
    public int hearts = 3;
    public int maxHealth = 100;
    public int health = 100;
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

    private void OnValidate() {
        if(!weaponManifesto) {
            GameObject gam = Instantiate(Resources.Load<GameObject>("Prefabs/WeaponManifesto"), transform);
            weaponManifesto = gam.GetComponent<WeaponManifesto>();
            weaponManifesto.owner = this;
        }
        Start();
    }

    public void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        weaponManifesto = GetComponentInChildren<WeaponManifesto>();
        weaponManifesto.owner = this;
        health = maxHealth;
    }

    public void FixedUpdate() {
        Move();
        Die();
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

    protected void Attack() {
        if(equippedWeapon != null) {
            weaponManifesto.Attack();
        }
    }

    void Die() {
        if (Input.GetKeyDown((KeyCode.G)) && dodging == false)
        {
            hearts -- ;
        }
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
        return $"Name: {name}, Movement Speed: {movementSpeed}, Sprint Speed: {sprintspeed}, Dodge Multiplier: {dodgeMultiplier}, Hearts: {hearts}\n Equipped Weapon: {equippedWeapon}, Equipped Armor: {equippedArmor}";
    }


}
