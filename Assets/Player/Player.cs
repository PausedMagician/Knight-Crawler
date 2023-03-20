using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Humanoid
{

    #region Singleton
    public static Player instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Player found!");
            return;
        }
        instance = this;
    }

    public static Player GetInstance()
    {
        if (instance == null)
            Debug.LogWarning("Player instance is null!\nAttempting to find one...");
        instance = FindObjectOfType<Player>();
        return instance;
    }

    #endregion

    public Inventory inventory;


    void Update()
    {
        HandleInput();
    }

    // Vector2 right_hand = new Vector2(0.25f, 0), left_hand = new Vector2(-0.25f, 0);


    void HandleInput()
    {
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
        Quaternion rotation = Quaternion.Slerp(weaponManifesto.transform.rotation, Quaternion.LookRotation(Vector3.forward, direction), 0.1f * Time.deltaTime * 100);
        weaponManifesto.transform.rotation = rotation;
        weaponManifesto.container.transform.rotation = Quaternion.Slerp(weaponManifesto.container.transform.rotation, Quaternion.LookRotation(Vector3.forward, (mousePosition - (Vector2)weaponManifesto.container.transform.position)), 0.1f * Time.deltaTime * 100);
        if (dodgeTimer <= 0)
        {
            movementDirection.x = Input.GetAxisRaw("Horizontal");
            movementDirection.y = Input.GetAxisRaw("Vertical");
        }

        // Left Click or R2 on controller
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Joystick1Button7)) && !Time.timeScale.Equals(0))
        {
            Attack();
        }
        // Left Shift or Circle on controller
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Joystick1Button2))
        {
            sprinting = true;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1)) && !Time.timeScale.Equals(0) && dodgeTimer <= 0 && movementDirection.magnitude > 0)
        {
            dodging = true;
        }

        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Joystick1Button9))
        {
            InventoryUI.GetInstance().ToggleInventory();
        }

    }


}
