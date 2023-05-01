
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D), typeof(Animator))]
public class Bonfire : Interactable
{

    public bool active = false;
    public Vector2 spawnPoint;
    public Animator animator;
    public GameController gameController;

    
    public override void Interact(Player player)
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
            gameController.SetLastRested(this);
        }
    }

    private void Awake()
    {
        m_ObjectCollider = GetComponent<Collider2D>();
        m_ObjectCollider.isTrigger = true;
        animator = GetComponent<Animator>();
        if (active)
        {
            gameController.SetLastRested(this);
        }
        GameController.OnBonfireUpdate += UpdateBonfire;
    }
    private void OnEnable() {
        GameController.OnBonfireUpdate += UpdateBonfire;
    }
    private void OnDisable() {
        GameController.OnBonfireUpdate -= UpdateBonfire;
    }
    public void StartBonfire() {
        if (active)
        {
            gameController.SetLastRested(this);
        }
    }
    public void UpdateBonfire()
    {
        Debug.Log(this);
        if(!this.animator) {
            this.animator = this.gameObject.GetComponent<Animator>();
        }
        if (gameController.lastRested == this)
        {
            active = true;
        }
        else
        {
            active = false;
        }
        animator.SetBool("Active", active);
    }

    private void OnDestroy() {
        GameController.OnBonfireUpdate -= UpdateBonfire;
        if(active) {
            gameController.SetLastRested(null);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + spawnPoint, 0.5f);
    }
}
