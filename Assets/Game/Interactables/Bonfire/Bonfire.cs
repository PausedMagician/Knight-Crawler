
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
        base.Interact(player);
        if (!active)
        {
            Debug.Log("Bonfire Rested");
            active = true;
            isLocked = true;
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
    }
    new private void OnEnable()
    {
        base.OnEnable();
        GameController.OnBonfireUpdate += UpdateBonfire;
    }
    new private void OnDisable()
    {
        base.OnDisable();
        GameController.OnBonfireUpdate -= UpdateBonfire;
    }
    public void StartBonfire()
    {
        if (active)
        {
            gameController.SetLastRested(this);
            isLocked = true;
        }
    }
    public void UpdateBonfire()
    {
        Debug.Log(this);
        if (!this.animator)
        {
            this.animator = this.gameObject.GetComponent<Animator>();
        }
        if (gameController.lastRested == this)
        {
            active = true;
            isLocked = true;
        }
        else
        {
            active = false;
            isLocked = false;
        }
        animator.SetBool("Active", active);
    }

    private void OnDestroy()
    {
        GameController.OnBonfireUpdate -= UpdateBonfire;
        if (active)
        {
            gameController.SetLastRested(null);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + spawnPoint, 0.5f);
    }
}
