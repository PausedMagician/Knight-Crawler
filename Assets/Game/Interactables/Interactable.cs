using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour {

    public Collider2D m_ObjectCollider;
    public Player player;
    public GameObject interactPrompt;
    public GameObject interactedText;
    public float interactTextTime = 2f;
    public bool isInteractable = false;
    public bool isLocked = false;
    private void Update() {
        if (isInteractable && !isLocked) {
            if (Input.GetKeyDown(KeyCode.E)) {
                Interact();
            }
        }
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.GetComponent<Player>() && !isLocked)
        {
            isInteractable = true;
            interactPrompt.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.GetComponent<Player>() && !isLocked)
        {
            isInteractable = false;
            interactPrompt.SetActive(false);
        }
    }
    public virtual void Interact() {
        // Debug.Log("Interacted with " + gameObject.name);
        interactPrompt.SetActive(false);
        if(interactedText != null) {
            interactedText.SetActive(true);
            Invoke("DisableInteractText", interactTextTime);
        }
    }
    void DisableInteractText()
    {
        interactedText.SetActive(false);
    }
}