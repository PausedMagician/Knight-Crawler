using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    #region Singleton
    public static Player instance;

    void Awake () {
        if(instance != null) {
            Debug.LogWarning("More than one instance of Player found!");
            return;
        }
        instance = this;
    }

    public static Player GetInstance() {
        if(instance == null)
            Debug.LogWarning("Player instance is null!\nAttempting to find one...");
            instance = FindObjectOfType<Player>();
        return instance;
    }

    #endregion

    public float movementSpeed = 5f;
    public float sprintspeed = 1.3f;
    public float dodge_multiply = 10f;
    public bool dodging = false;
    public Rigidbody2D rb;
    public Weapon equippedWeapon;
    public WeaponManifesto weaponManifesto;
    public Armor equippedArmor;
    public Inventory inventory;
    

    public int hearts = 3;

    void Start()
    {
        
    }

    Vector2 movementDirection = new Vector2(0, 0);

    Vector2 dodgeDirection = new Vector2(0,0);

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate() {
        Move();
        Dodge();
        Die();
    
    }

    Vector2 right_hand = new Vector2(0.25f, 0), left_hand = new Vector2(-0.25f, 0);

    void HandleInput() {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position;
        // if(direction.x < 0) {
        //     Debug.DrawRay((Vector2)transform.position + left_hand, direction.normalized, Color.green, 1f);
        //     // weaponManifesto.spriteRenderer.flipX = true;
        //     Vector2 weaponDirection = mousePosition - (Vector2)weaponManifesto.transform.position;
        //     weaponManifesto.gameObject.transform.localPosition = Vector3.Lerp(weaponManifesto.gameObject.transform.localPosition, left_hand, 0.1f*Time.deltaTime*100);
        //     //rotate weapon to face mouse
        //     Quaternion rotation = Quaternion.Slerp(weaponManifesto.transform.rotation, Quaternion.LookRotation(-Vector3.forward, weaponDirection), 0.1f*Time.deltaTime*100);
        //     weaponManifesto.transform.rotation = rotation;
        // } else {
        //     Debug.DrawRay((Vector2)transform.position + right_hand, direction.normalized, Color.green, 1f);
        //     weaponManifesto.spriteRenderer.flipX = true;
        //     Vector2 weaponDirection = mousePosition - (Vector2)weaponManifesto.transform.position;
        //     weaponManifesto.gameObject.transform.localPosition = Vector3.Lerp(weaponManifesto.gameObject.transform.localPosition, right_hand, 0.1f*Time.deltaTime*100);
        //     //rotate weapon to face mouse
        //     Quaternion rotation = Quaternion.Slerp(weaponManifesto.transform.rotation, Quaternion.LookRotation(Vector3.forward, weaponDirection), 0.1f*Time.deltaTime*100);
        //     weaponManifesto.transform.rotation = rotation;
        // }

        // Debug.DrawRay((Vector2)transform.position, direction.normalized, Color.green, 1f);
        //rotate weapon to face mouse
        Quaternion rotation = Quaternion.Slerp(weaponManifesto.transform.rotation, Quaternion.LookRotation(Vector3.forward, direction), 0.1f*Time.deltaTime*100);
        weaponManifesto.transform.rotation = rotation;
        weaponManifesto.container.transform.rotation = Quaternion.Slerp(weaponManifesto.container.transform.rotation, Quaternion.LookRotation(Vector3.forward, (mousePosition - (Vector2)weaponManifesto.container.transform.position)), 0.1f*Time.deltaTime*100);
        
        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.y = Input.GetAxisRaw("Vertical");

        if(Input.GetMouseButtonDown(0) && !Time.timeScale.Equals(0)) {
            Attack();
        }

        if(Input.GetKeyDown(KeyCode.I)) {
            InventoryUI.GetInstance().ToggleInventory();
        }
        
    }

    void Move() {
        if (Input.GetKey((KeyCode.LeftShift)))
        {
            rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * sprintspeed * Time.fixedDeltaTime);
        }else
        {
            rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * Time.fixedDeltaTime);
        }
    }

    void Dodge() {
        if(Input.GetKey(KeyCode.Space) && !dodging){
            // dodging = true;
            rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * Time.fixedDeltaTime * dodge_multiply);
            // dodging = false;
        } 
    }

    void Attack() {
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
            Debug.Log("Dead");
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
