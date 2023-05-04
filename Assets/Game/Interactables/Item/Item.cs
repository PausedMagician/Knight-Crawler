using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Item : Interactable
{
    public Material[] materials; //0 is common, 1 is uncommon, 2 is rare, 3 is epic, 4 is legendary.
    public ItemData item;
    private float frequency = 0.5f;
    private float amplitude = 0.1f;
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    public void SetItem(ItemData item)
    {
        posOffset = transform.position;
        gameObject.GetComponent<SpriteRenderer>().material = materials[(int)item.rarity];
        gameObject.GetComponentInChildren<Light2D>().color = materials[(int)item.rarity].color;
        this.item = item;
        // Debug.Log((int)item.rarity);
    }

    public void RandomItem(int min = 0, int max = 4)
    {
        if (Random.Range(0, 2) == 0)
            item = GameController.GetInstance().CreateArmor(GameController.GetRarity(), Random.Range(min, max));
        else
        {
            item = GameController.GetInstance().CreateMelee(GameController.GetRarity(), Random.Range(min, max));
        }
        posOffset = transform.position;
        gameObject.GetComponent<SpriteRenderer>().material = materials[(int)item.rarity];
        gameObject.GetComponentInChildren<Light2D>().color = materials[(int)item.rarity].color;
        // Debug.Log((int)item.rarity);
    }

    public void FixedUpdate()
    {
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        transform.position = tempPos;
    }

    public override void Interact(Player player)
    {
        base.Interact(player);
        player.inventory.AddItem(item);
        Destroy(gameObject);
    }
}
