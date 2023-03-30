using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Humanoid : MonoBehaviour
{
    [Header("Main Settings")]
    public string Name = "Humanoid";
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

    public void update() {
        if (Input.GetKey(KeyCode.G)) {
            hearts--;
        }
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

    protected void TurnWeapon(Vector2 _transform, Vector2 target, float _time) {
        if(target == Vector2.zero) {
            return;
        }
        Vector2 direction = target - _transform;

        Quaternion rotation = Quaternion.Slerp(weaponManifesto.transform.rotation, Quaternion.LookRotation(Vector3.forward, direction), 0.1f * _time * 100);
        weaponManifesto.transform.rotation = rotation;
        weaponManifesto.container.transform.rotation = Quaternion.Slerp(weaponManifesto.container.transform.rotation, Quaternion.LookRotation(Vector3.forward, (target - (Vector2)weaponManifesto.container.transform.position)), 0.1f * _time * 100);
    }

    protected virtual void Attack() {
        if(equippedWeapon != null) {
            weaponManifesto.Attack();
        }
    }

    public void TakeDamage(Weapon weapon) {
        this.health -= GameController.CalculateDamage(weapon, equippedArmor);
        if(health <= 0) {
            health = 0;
            Die();
        }
    }

    public virtual void Die() {
        if (hearts <= 0 && GameController.lastRested != null)
        {
            
        }else if(hearts <= 0 && GameController.lastRested == null) {

            transform.position = new Vector3(0, 0, 0);
        
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
