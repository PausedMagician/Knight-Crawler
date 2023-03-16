using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameController : MonoBehaviour
{
    private static volatile GameController instance;
    private static Object syncRootObject = new Object();

    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRootObject)
                {
                    if (instance == null)
                    {
                        instance = new GameController();
                    }
                }
            }
            return instance;
        }
    }

    public static Bonfire lastRested;
    public static Player player;

    public Sprite[] meleeSprites;
    public Sprite[] rangedSprites;
    public Sprite[] magicSprites;

    private void Awake() {
        UpdateSprites();   
    }

    private void Start() {
        player = GameObject.FindObjectOfType<Player>();
        Melee thing = null;
        for (int i = 0; i < 6; i++)
        {
            thing = ScriptableObject.CreateInstance<Melee>();
            int selected  = Random.Range(0, meleeSprites.Length);
            thing.itemName = meleeSprites[selected].name + " Sword";
            thing.damage = 10;
            thing.cost = 10;
            thing.level = 1;
            thing.sprite = meleeSprites[selected];
            thing.maxCombo = 3;
            thing.animationSet = (AnimationSet)Random.Range(0, 3);
            thing.rarity = (Rarity)Random.Range(0, 5);
            player.inventory.AddItem(thing);
        }
        player.inventory.EquipWeapon(thing);
    }

    private void UpdateSprites() {
        meleeSprites = Resources.LoadAll<Sprite>("Sprites/Melee/");
        rangedSprites = Resources.LoadAll<Sprite>("Sprites/Ranged/");
        magicSprites = Resources.LoadAll<Sprite>("Sprites/Magic/");
    }
}
