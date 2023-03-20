
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D), typeof(Animator))]
public class Bonfire : Interactable
{

    public bool active = false;
    public Vector2 spawnPoint;
    public Animator animator;

    
    public override void Interact()
    {
        if (active)
        {
            active = false;
            animator.SetBool("Active", active);
        }
        else
        {
            Debug.Log("Bonfire Rested");
            active = true;
            animator.SetBool("Active", active);
            animator.SetTrigger("Light");
            GameController.SetLastRested(this);
        }
    }

    private void Awake()
    {
        m_ObjectCollider = GetComponent<Collider2D>();
        m_ObjectCollider.isTrigger = true;
        animator = GetComponent<Animator>();
        if (active)
        {
            GameController.SetLastRested(this);
        }
        GameController.OnBonfireUpdate += UpdateBonfire;
    }
    public void UpdateBonfire()
    {
        animator.SetBool("Active", active);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + spawnPoint, 0.5f);
    }
}
