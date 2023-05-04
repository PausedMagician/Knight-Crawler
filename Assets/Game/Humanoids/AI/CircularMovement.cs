using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMovement : MonoBehaviour
{

    public Transform rotationCenter;
    public GameObject target;
    AI2 AI2_;
    public float rotationRadius = 1f;


    void FixedUpdate()
    {
        if (target == null)
        {
            target = GameObject.FindWithTag("Player");
        }
        
        if (target)
        {
            transform.position = Vector2.MoveTowards(rotationCenter.position, target.transform.position, rotationRadius);
            Checkfor();
        }
    }


    public bool Checkfor()
    {
        if (target)
        {
            Vector2 froms = transform.position;
            Vector2 tos = target.transform.position;
            Vector2 trueDirection = (tos - froms);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, trueDirection);
            // Debug.DrawRay(transform.position, trueDirection, Color.green);
            if (hit.collider.gameObject.tag == "Player" || hit.collider.isTrigger)
            {
                // Debug.Log("Player");
                // Debug.Log($"{hit.collider.gameObject.name} was hit");
                return true;
            }
            else if (hit.collider.gameObject.tag != "Player")
            {
                // Debug.Log("Not Player");
                // Debug.Log($"{hit.collider.gameObject.name} was hit");
                return false;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
