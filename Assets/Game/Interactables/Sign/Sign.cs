using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Sign : Interactable
{
    public string text = "This is a sign";
    private void Awake() {
        OnLeave += DisableInteractText;
        interactedText.GetComponentInChildren<TextMeshProUGUI>().text = text;
        m_ObjectCollider.isTrigger = true;
    }
    public override void Interact(Player player)
    {
        // Debug.Log("Interacted with " + gameObject.name);
        interactPrompt.SetActive(false);
        if(interactedText != null) {
            interactedText.SetActive(true);
        }
    }
}
