using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Humanoid
{

    #region Singleton
    public static Player instance;
    public GameController gamecontroller;


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
    public Interactable interactingWith;
    public Animator animator;

    void Update()
    {
        Dead();
        HandleInput();
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        if(!animator) {
            animator = this.gameObject.GetComponent<Animator>();
        }
        if(animator) {
            if(!sprinting) {
                animator.speed = this.movementSpeed / 7f;
            } else {
                animator.speed = (this.movementSpeed / 7f) * sprintspeed;
            }
            animator.SetFloat("X", movementDirection.x);
            animator.SetFloat("Y", movementDirection.y);
        }
    }

    // Vector2 right_hand = new Vector2(0.25f, 0), left_hand = new Vector2(-0.25f, 0);


    void HandleInput()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //rotate weapon to face mouse
        if (equippedWeapon)
        {
            if (equippedWeapon is Melee)
            {
                TurnWeapon(transform.position, mousePosition, Time.deltaTime);
            }
            else
            {
                TurnWeapon(transform.position, mousePosition, Time.deltaTime, 45f);
            }
        }
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

    public void Dead()
    {
        if (health == 0)
        {
            hearts--;
            if (hearts != 0)
            {
                foreach (Humanoid attacker in targetedBy)
                {
                    attacker.targetedBy = null;
                    targetedBy.Remove(attacker);
                }
                if (gamecontroller.lastRested != null)
                {
                    Bonfire lastRested = gamecontroller.lastRested;
                    this.transform.position = (Vector2)lastRested.transform.position + lastRested.spawnPoint;
                    health = maxHealth;
                }

            }
            else
            {
                GameController.ReturnToStart();
            }

            // Destroy(gameObject, 2f);
        }
    }
}
