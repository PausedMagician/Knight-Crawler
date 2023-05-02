using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chest : Interactable
{
    public ItemData[] items;
    int dropCount;
    public Sprite OpenChest;
    public ParticleSystem particles;

    public int level = 1;

    public override void Interact(Player player)
    {
        
        base.Interact(player);
        dropCount = Random.Range(1, 10);
        // Debug.Log(dropCount);
        if (items.Length == 0)
        {
            while (dropCount > 0)
            {
                dropCount -= 1;
                Vector2 pos = transform.position;
                pos += new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.2f, -1f)).normalized * Random.Range(1.2f, 1.5f);
                GameObject go = Instantiate(GameController.GetInstance().itemPrefab, pos, Quaternion.identity);
                go.transform.position = pos;
                go.GetComponent<Item>().RandomItem(level, level + 2);
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
        particles.Play();
        this.gameObject.GetComponent<SpriteRenderer>().sprite = OpenChest;
    }
}