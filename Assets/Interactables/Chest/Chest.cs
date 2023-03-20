using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chest : Interactable
{
    public Item[] items;

    public override void Interact()
    {
        base.Interact();
        if (items.Length == 0)
        {
            items = new Item[Random.Range(1, 4)];
            for (int i = 0; i < items.Length; i++)
            {
                if(Random.Range(0, 2) == 0)
                    items[i] = GameController.GetInstance().CreateArmor((Rarity)Random.Range(0, 5), Random.Range(1, 10));
                else
                    items[i] = GameController.GetInstance().CreateMelee((Rarity)Random.Range(0, 5), Random.Range(1, 10));
            }
        }
        Inventory.GetInstance().AddItems(items);
        interactedText.GetComponentInChildren<TextMeshProUGUI>().text = Item.ToDebugStringShort(items);
        isLocked = true;
    }
}