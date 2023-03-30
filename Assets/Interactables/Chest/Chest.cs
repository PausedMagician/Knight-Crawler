using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chest : Interactable
{
    public Item[] items;

    [SerializeField] GameObject drop;
    [SerializeField] int dropCount;
    [SerializeField] float spread = 3f;

    public override void Interact()
    {
        base.Interact();
        dropCount = Random.Range(2, 5);
        Debug.Log(dropCount);
        if (items.Length == 0)
        {
            while (dropCount > 0)
            {
                dropCount -= 1;
                Vector3 pos = transform.position;
                pos.x += spread * UnityEngine.Random.value - spread / 2;
                pos.y += spread * UnityEngine.Random.value - spread / 2;
                GameObject go = Instantiate(drop);
                go.transform.position = pos;

            }
            // items = new Item[Random.Range(1, 4)];
            // for (int i = 0; i < items.Length; i++)
            // {
            //     if(Random.Range(0, 2) == 0)
            //         items[i] = GameController.GetInstance().CreateArmor((Rarity)Random.Range(0, 5), Random.Range(1, 10));
            //     else
            //         items[i] = GameController.GetInstance().CreateMelee((Rarity)Random.Range(0, 5), Random.Range(1, 10));
            // }
        }
        // Inventory.GetInstance().AddItems(items);
        // interactedText.GetComponentInChildren<TextMeshProUGUI>().text = Item.ToDebugStringShort(items);
        isLocked = true;
    }
}