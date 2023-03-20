using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humanoid : MonoBehaviour
{

    public float movementSpeed = 5f;
    public float sprintspeed = 1.3f;
    public float dodgeMultiplier = 10f;
    public bool dodging = false, sprinting = false;
    public Rigidbody2D rb;
    public Weapon equippedWeapon;
    public WeaponManifesto weaponManifesto;
    public Armor equippedArmor;
    

    public int hearts = 3;

    void Start()
    {
        
    }

    public Vector2 movementDirection = new Vector2(0, 0);

    public void FixedUpdate() {
        Move();
        Die();
    }

    void Move() {
        if (sprinting) {
            rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * sprintspeed * Time.fixedDeltaTime);
            sprinting = false;
        }else if (dodging) {
            rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * Time.fixedDeltaTime * dodgeMultiplier);
            dodging = false;
        } else {
            rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * Time.fixedDeltaTime);
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

}
