using UnityEngine;

public class WeaponBody : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Triggered");
        if(other.GetComponent<Humanoid>()) {
            Debug.Log("Hit enemy");
            Debug.Log(other.gameObject.GetComponent<Humanoid>());
        }
    }
}