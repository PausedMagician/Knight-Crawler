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

    public Sprite[] lightArmorSprites;
    public Sprite[] mediumArmorSprites;
    public Sprite[] heavyArmorSprites;
    public Sprite[][] armorSprites = new Sprite[3][];

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
        Armor armor = null;
        for (int i = 0; i < 3; i++)
        {
            armor = ScriptableObject.CreateInstance<Armor>();
            armor.armorType = (ArmorType)Random.Range(0, 3);
            int selected = Random.Range(0, armorSprites[(int)armor.armorType].Length);
            armor.itemName = armorSprites[(int)armor.armorType][selected].name + " Armor";
            armor.cost = 10;
            armor.level = 1;
            armor.sprite = armorSprites[(int)armor.armorType][selected];
            armor.rarity = (Rarity)Random.Range(0, 5);
            armor.effects.Add(Effect.CreateEffect(ArmorEffector.Defense, 10));
            for (int eff = 0; eff < 3; eff++)
            {
                armor.effects.Add(Effect.CreateEffect((ArmorEffector)Random.Range(0, 7), 10));
            }
            player.inventory.AddItem(armor);
        }
        player.inventory.EquipArmor(armor);
    }

    private void UpdateSprites() {
        meleeSprites = Resources.LoadAll<Sprite>("Sprites/Weapons/Melee/");
        rangedSprites = Resources.LoadAll<Sprite>("Sprites/Weapons/Ranged/");
        magicSprites = Resources.LoadAll<Sprite>("Sprites/Weapons/Magic/");

        lightArmorSprites = Resources.LoadAll<Sprite>("Sprites/Armor/Light/");
        mediumArmorSprites = Resources.LoadAll<Sprite>("Sprites/Armor/Medium/");
        heavyArmorSprites = Resources.LoadAll<Sprite>("Sprites/Armor/Heavy/");
        armorSprites[0] = lightArmorSprites;
        armorSprites[1] = mediumArmorSprites;
        armorSprites[2] = heavyArmorSprites;

    }
}
