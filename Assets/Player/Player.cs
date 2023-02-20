using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float movementSpeed = 5f;
    public Rigidbody2D rb;
    public Weapon equippedWeapon;
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

    void HandleInput() {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position;
        if(direction.x < 0) {
            Debug.DrawRay(transform.position - new Vector3(0.25f, 0, 0), direction.normalized, Color.green, 1f);
        } else {
            Debug.DrawRay(transform.position + new Vector3(0.25f, 0, 0), direction.normalized, Color.green, 1f);
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
        return tempWeapon;
    }
    public Armor EquipArmor(Armor armor) {
        Armor tempArmor = equippedArmor;
        equippedArmor = armor;
        return tempArmor;
    }

}
