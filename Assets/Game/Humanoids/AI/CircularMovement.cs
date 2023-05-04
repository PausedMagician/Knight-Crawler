using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CircularMovement : MonoBehaviour
{

    public AI2 parent;
    public float rotationRadius = 1f;


    void FixedUpdate()
    {
        if (parent.target)
        {
            transform.position = Vector2.MoveTowards(parent.transform.position, parent.target.transform.position, rotationRadius);
        }
    }


    public bool Checkfor(Humanoid target)
    {
        if (target != null)
        {
            Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position);
            Debug.DrawLine(transform.position, transform.position + (Vector3)direction, Color.red, 1f);
            Debug.DrawRay(transform.position, direction * 1.25f, Color.blue, 1f);
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, direction.magnitude * 1.25f).OrderBy(h => h.distance).ToArray();
            foreach (RaycastHit2D hit in hits)
            {
                if(hit.collider.transform.parent) {
                    if(hit.collider.transform.parent.gameObject.name == "Floors") {
                        continue;
                    }
                }
                if(hit.collider.isTrigger || hit.collider.gameObject == parent.gameObject) {
                    continue;
                }
                else if (hit.collider.gameObject.GetComponent<Humanoid>() == target)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }
}
