
using UnityEngine;

public class Bonfire : MonoBehaviour
{
    Collider m_ObjectCollider;

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnTriggerStay2D(Collider2D coll)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.LogWarning("HI");
            hearts = 3;
        }
    }
}
