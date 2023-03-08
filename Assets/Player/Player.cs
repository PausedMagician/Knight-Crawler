using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float movementSpeed = 5f;
    public Rigidbody2D rb;
    public Weapon equippedWeapon;
    public WeaponManifesto weaponManifesto;
    public Armor equippedArmor;
    public Inventory inventory;

    void Start()
    {
        
    }

    Vector2 movementDirection = new Vector2(0, 0);

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate() {
        Move();
    }

    Vector2 right_hand = new Vector2(0.25f, 0), left_hand = new Vector2(-0.25f, 0);

    void HandleInput() {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position;
        if(direction.x < 0) {
            Debug.DrawRay((Vector2)transform.position + left_hand, direction.normalized, Color.green, 1f);
            weaponManifesto.spriteRenderer.flipX = true;
            Vector2 weaponDirection = mousePosition - (Vector2)weaponManifesto.transform.position;
            weaponManifesto.gameObject.transform.localPosition = Vector3.Lerp(weaponManifesto.gameObject.transform.localPosition, left_hand, 0.1f);
            //rotate weapon to face mouse
            Quaternion rotation = Quaternion.Slerp(weaponManifesto.transform.rotation, Quaternion.LookRotation(-Vector3.forward, weaponDirection), 0.1f);
            weaponManifesto.transform.rotation = rotation;
        } else {
            Debug.DrawRay((Vector2)transform.position + right_hand, direction.normalized, Color.green, 1f);
            weaponManifesto.spriteRenderer.flipX = false;
            Vector2 weaponDirection = mousePosition - (Vector2)weaponManifesto.transform.position;
            weaponManifesto.gameObject.transform.localPosition = Vector3.Lerp(weaponManifesto.gameObject.transform.localPosition, right_hand, 0.1f);
            //rotate weapon to face mouse
            Quaternion rotation = Quaternion.Slerp(weaponManifesto.transform.rotation, Quaternion.LookRotation(Vector3.forward, weaponDirection), 0.1f);
            weaponManifesto.transform.rotation = rotation;
        }

        
        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.y = Input.GetAxisRaw("Vertical");
        
    }

    void Move() {
        rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * Time.fixedDeltaTime);
    }

    void Dodge() {
        
    }

    void Attack() {

    }

    void Die() {

    }
    
    public Weapon EquipWeapon(Weapon weapon) {
        Weapon tempWeapon = equippedWeapon;
        equippedWeapon = weapon;
        weaponManifesto.UpdateWeapon();
        return tempWeapon;
    }
    public Armor EquipArmor(Armor armor) {
        Armor tempArmor = equippedArmor;
        equippedArmor = armor;
        return tempArmor;
    }

}
