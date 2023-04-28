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

    public void Start()
    {
        if (Random.Range(0, 2) == 0)
            item = GameController.GetInstance().CreateArmor((Rarity)Random.Range(0, 5), Random.Range(1, 10));
        else {
            int randomint = Random.Range(0, 4);
            if(randomint == 0) {
                item = GameController.GetInstance().CreateMelee((Rarity)Random.Range(0, 5), Random.Range(1, 10));
            } else if (randomint == 1) {
                item = GameController.GetInstance().CreateRanged((Rarity)Random.Range(0, 5), Random.Range(1, 10));
            } else {
                item = GameController.GetInstance().CreateRanged((Rarity)Random.Range(0, 5), Random.Range(1, 10));
            }
        }
        posOffset = transform.position;
        gameObject.GetComponent<SpriteRenderer>().material = materials[(int)item.rarity];
        gameObject.GetComponentInChildren<Light2D>().color = materials[(int)item.rarity].color;
        // Debug.Log((int)item.rarity);
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
        Inventory.GetInstance().AddItem(item);
        Destroy(gameObject);
    }
}
