
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class Bonfire : MonoBehaviour
{

    public bool active = false;
    public Vector2 spawnPoint;

    public Collider2D m_ObjectCollider;


    private void OnValidate() {
        m_ObjectCollider = GetComponent<Collider2D>();
        m_ObjectCollider.isTrigger = true;
        if(active) {
            GameController.lastRested = this;
        }
    }
    private void Awake() {
        m_ObjectCollider = GetComponent<Collider2D>();
        m_ObjectCollider.isTrigger = true;
        if(active) {
            GameController.lastRested = this;
        }
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
