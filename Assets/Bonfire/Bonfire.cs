
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D), typeof(Animator))]
public class Bonfire : MonoBehaviour
{

    public bool active = false;
    public Vector2 spawnPoint;

    public Collider2D m_ObjectCollider;
    public Animator animator;


    private void OnValidate() {
        Awake();
    }
    private void Awake() {
        m_ObjectCollider = GetComponent<Collider2D>();
        m_ObjectCollider.isTrigger = true;
        animator = GetComponent<Animator>();
        if(active) {
            GameController.SetLastRested(this);
        }
        GameController.OnBonfireUpdate += UpdateBonfire;
    }
    public void UpdateBonfire() {
        if(active) {
            GameController.SetLastRested(this);
        }
        animator.SetBool("Active", active);
    }
    // Update is called once per frame
    void OnTriggerStay2D(Collider2D coll)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.LogWarning("HI");
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + spawnPoint, 0.5f);
    }
}
