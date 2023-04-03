using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed; //Speed at the moment, this will go down over time.
    public Vector2 direction; //The direction the projectile is flying in.
    public float tracking; //Tracking in percentage, if tracking is 1 it will completely hone in on nearest enemy, if 0 it will go straight.
    public bool lasting; //If the sprite is everlasting af the projectile dies. If true it will leave a sprite behind forever.
    bool rotationcheck_ = false;

    public Humanoid shooter; //Who shot the projectile

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        Vector2 nextStep = Vector2.Lerp(transform.position, (Vector2)transform.position + (direction * speed), 0.1f);

        if (!CheckDirection(transform.position, nextStep))
        {
            Move(nextStep);
        }
        else
        {
            Die();
        }
    }

    public void Rotate() {
        var dir = direction;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward) * Quaternion.Euler(0, 0, -45);
        rotationcheck_ = true;
    }

    void Move(Vector2 to)
    {
        transform.position = to;
        if (speed <= 0)
        {
            Die();
            return;
        }
        Rotate();
        speed -= 1 * Time.fixedDeltaTime;
    }
    void Die()
    {
        speed = 0;
        this.enabled = false;
    }

    bool CheckDirection(Vector2 from, Vector2 to)
    {
        Vector2 trueDirection = (to - from);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, trueDirection, trueDirection.magnitude);
        if (hit)
        {
            Humanoid attacked = hit.collider.GetComponent<Humanoid>();
            if(attacked) {
                if(attacked == shooter) {
                    return false;
                } else {
                    attacked.TakeDamage(shooter.equippedWeapon, shooter);
                    transform.SetParent(attacked.transform);
                }
            }
            Debug.Log($"{hit.collider.gameObject.name} was hit");
            transform.position = hit.point;
            return true;
        }
        else
        {
            return false;
        }
    }

}
