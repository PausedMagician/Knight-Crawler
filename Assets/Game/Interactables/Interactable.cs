using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.Events;

public class Interactable : MonoBehaviour {

    public Collider2D m_ObjectCollider;
    public GameObject interactPrompt;
    public GameObject interactedText;
    public float interactTextTime = 2f;
    public bool isInteractable = false;
    public Player interacting;
    public bool isLocked = false;

    public UnityAction<Player> OnInteract, OnEnter, OnLeave;


    private void OnEnable() {
        OnInteract += Interact;
        OnEnter += Entering;
        OnLeave += Leaving;
    }
    private void OnDisable() {
        if (OnInteract != null) {
            OnInteract -= Interact;
        }
        if (OnEnter != null) {
            OnEnter -= Entering;
        }
        if (OnLeave != null) {
            OnLeave -= Leaving;
        }
    }


    private void Update() {
        if (isInteractable && !isLocked) {
            if (Input.GetKeyDown(KeyCode.E)) {
                OnInteract?.Invoke(interacting);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        Player player = coll.gameObject.GetComponent<Player>();
        if (player && !isLocked && player.interactingWith == null)
        {
            player.interactingWith = this;
            OnEnter?.Invoke(player);
        }
    }
    private void OnTriggerStay2D(Collider2D other) {
        Player player = other.gameObject.GetComponent<Player>();
        if (player && !isLocked)
        {
            if(player.interactingWith == null) {
                player.interactingWith = this;
                OnEnter?.Invoke(player);
            } else if(player.interactingWith != this) {
                OnLeave?.Invoke(player);
            }
        }
    }
    void OnTriggerExit2D(Collider2D coll)
    {
        Player player = coll.gameObject.GetComponent<Player>();
        if (player)
        {
            if(player.interactingWith == this) {
                player.interactingWith = null;
            }
            if(!isLocked) {
                OnLeave?.Invoke(player);
            }
        }
    }
    public virtual void Interact(Player player) {
        // Debug.Log("Interacted with " + gameObject.name);
        interactPrompt.SetActive(false);
        if(interactedText != null) {
            interactedText.SetActive(true);
            Invoke("DisableInteractText", interactTextTime);
        }
    }
    public virtual void Entering(Player player) {
        isInteractable = true;
        interacting = player;
        interactPrompt.SetActive(true);
    }
    public virtual void Leaving(Player player) {
        isInteractable = false;
        interactPrompt.SetActive(false);
    }
    public void DisableInteractText(Player player)
    {
        interactedText.SetActive(false);
    }
}