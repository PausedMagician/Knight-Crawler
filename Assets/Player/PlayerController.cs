using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float movementSpeed = 5f;
    public Rigidbody2D rb;

    void Start()
    {
        
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput() {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position;
        if(direction.x < 0) {
            Debug.DrawRay(transform.position - new Vector3(0.25f, 0, 0), direction.normalized, Color.green, 1f);
        } else {
            Debug.DrawRay(transform.position + new Vector3(0.25f, 0, 0), direction.normalized, Color.green, 1f);
        }

        Vector2 movementDirection = new Vector2(0, 0);
        
        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.y = Input.GetAxisRaw("Vertical");

        rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * Time.deltaTime);
        
    }

    void Move() {

    }

    void Dodge() {

    }

    void Attack() {

    }



}
