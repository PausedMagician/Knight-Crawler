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
        Melee thing = ScriptableObject.CreateInstance<Melee>();
        thing.name = "Sword";
        thing.damage = 10;
        thing.cost = 10;
        thing.level = 1;
        thing.sprite = meleeSprites[0];
        player.inventory.AddItem(thing);
        player.inventory.AddItem(thing);
        player.inventory.EquipWeapon(thing);
    }

    private void UpdateSprites() {
        meleeSprites = Resources.LoadAll<Sprite>("Sprites/Melee/");
        rangedSprites = Resources.LoadAll<Sprite>("Sprites/Ranged/");
        magicSprites = Resources.LoadAll<Sprite>("Sprites/Magic/");
    }
}
