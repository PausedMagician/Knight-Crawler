using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{

    public ItemData[] items;
    private float frequency = 0.5f;
    private float amplitude = 0.1f;
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();    

    public void Start()
    {
        posOffset = transform.position;
    }

    public void FixedUpdate() 
    {
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency)* amplitude;
        transform.position = tempPos;
    }

    public override void Interact()
    {
        base.Interact();

        items = new ItemData[Random.Range(1, 1)];
        for (int i = 0; i < items.Length; i++)
        {
            if (Random.Range(0, 2) == 0)
                items[i] = GameController.GetInstance().CreateArmor((Rarity)Random.Range(0, 5), Random.Range(1, 10));
            else
                items[i] = GameController.GetInstance().CreateMelee((Rarity)Random.Range(0, 5), Random.Range(1, 10));
        }
        Inventory.GetInstance().AddItems(items);
        Destroy(gameObject);
    }
}
