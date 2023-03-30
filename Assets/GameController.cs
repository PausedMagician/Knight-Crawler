using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.Events;

public sealed class GameController : MonoBehaviour
{
    #region Singleton
    public static GameController instance;

    void Awake()
    {
        UpdateSprites();
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Player found!");
            return;
        }
        instance = this;
    }

    public static GameController GetInstance()
    {
        if (instance == null)
            Debug.LogWarning("Player instance is null!\nAttempting to find one...");
        instance = FindObjectOfType<GameController>();
        return instance;
    }

    #endregion

    public static Bonfire lastRested;
    public static void SetLastRested(Bonfire bonfire) {
        lastRested = bonfire;
        OnBonfireUpdate?.Invoke();
    }
    public static Player player;

    public Sprite[] meleeSprites;
    public Sprite[] rangedSprites;
    public Sprite[] magicSprites;

    public Sprite[] lightArmorSprites;
    public Sprite[] mediumArmorSprites;
    public Sprite[] heavyArmorSprites;
    public Sprite[][] armorSprites = new Sprite[3][];

    public static UnityAction OnBonfireUpdate;
    public static UnityAction Tick;
    public static float TickTimer;

    private void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        Melee thing = null;
        for (int i = 0; i < 15; i++)
        {
            thing = CreateMelee((Rarity)Random.Range(0, 5), Random.Range(1, 10));
            player.inventory.AddItem(thing);
        }
        player.inventory.EquipWeapon(thing);
        Armor armor = null;
        for (int i = 0; i < 10; i++)
        {
            armor = CreateArmor((Rarity)Random.Range(0, 5), Random.Range(1, 10));
            player.inventory.AddItem(armor);
        }
        player.inventory.EquipArmor(armor);

        StartGame();

    }


    private void Update() {
        if(TickTimer <= 0) {
            Tick?.Invoke();
            TickTimer = 1f;
        } else {
            TickTimer -= Time.deltaTime;
        }
    }


    private void UpdateSprites()
    {
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


    int GetPoints(Rarity rarity, int level)
    {
        // float k1 = 0.001f;
        // float k2 = -1;
        // float k3 = -0.6f;
        // int points = (int)Mathf.Round((level * Mathf.Pow((int)rarity + 1, k3) * Mathf.Pow(10, level * k1) + 5 * Mathf.Pow((int)rarity + 1, k2)));
        int points = Mathf.RoundToInt(level * ((int)rarity+1 / 5 * 1.25f) + ((int)rarity + 1) * 5);
        return points;
    }

    public Melee CreateMelee(Rarity rarity, int level)
    {
        int points = GetPoints(rarity, level);
        int maxEffects = (int)rarity + 2;
        Melee melee = ScriptableObject.CreateInstance<Melee>();
        List<Effect> effects = new List<Effect>();
        effects.Add(Effect.CreateEffect(Effector.Damage, melee, (int)points/4, AmountType.Flat));
        points -= (int)points/4;
        effects.AddRange(Effect.CreateEffects(points, maxEffects, melee));
        effects.TrimEffects();
        int selected = Random.Range(0, meleeSprites.Length);
        string selectedEffect = "";
        if (effects.Count > 1)
        {
            selectedEffect = $" of {effects[1].name}";
        }
        melee.itemName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase($"{rarity.ToString()} {meleeSprites[selected].name}{selectedEffect}");
        melee.effects = effects;
        if(points > 100) {melee.cost = (int)Mathf.Round(points * 0.1f) * 10;} else {melee.cost = points;}
        melee.level = level;
        melee.sprite = meleeSprites[selected];
        melee.maxCombo = Random.Range(2, 4);
        melee.animationSet = (AnimationSet)Random.Range(0, 3);
        melee.rarity = rarity;
        return melee;
    }




    public Armor CreateArmor(Rarity rarity, int level)
    {
        int points = GetPoints(rarity, level);
        int maxEffects = (int)rarity + 2;
        Armor armor = ScriptableObject.CreateInstance<Armor>();
        List<Effect> effects = new List<Effect>();
        effects.Add(Effect.CreateEffect(Effector.Damage, armor, (int)points/4, AmountType.Flat));
        points -= (int)points/4;
        effects.AddRange(Effect.CreateEffects(points, maxEffects, armor));
        effects.TrimEffects();
        int selected = Random.Range(0, armorSprites[(int)armor.armorType].Length);
        string selectedEffect = "";
        if (effects.Count > 1)
        {
            selectedEffect = $" of {effects[1].name}";
        }
        armor.itemName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase($"{rarity.ToString()} {armorSprites[(int)armor.armorType][selected].name}{selectedEffect}");;
        armor.effects = effects;
        if(points > 100) {armor.cost = (int)Mathf.Round(points * 0.1f) * 10;} else {armor.cost = points;}
        armor.level = level;
        armor.sprite = armorSprites[(int)armor.armorType][selected];
        armor.rarity = rarity;
        return armor;
    }

    public static int CalculateDamage(Weapon weapon, Armor hitting, out int heal) {
        int Damage = 0;
        heal = 0;
        if(weapon == null) {
            return Damage;
        }
        for (int i = 0; i < weapon.effects.Count; i++)
        {
            Effect effect = weapon.effects[i];
            if(effect.type == EffectType.Damage) {
                if(effect.amountType == AmountType.Flat) {
                    Damage += effect.amount;
                } else {
                    Damage *= (1 + (effect.amount/100));
                }
            } else if (effect.specificType == Effector.HealthRegen) {
                if(effect.amountType == AmountType.Flat) {
                    heal += effect.amount;
                } else {
                    heal *= (1 + (effect.amount/100));
                }
            }
        }
        if(hitting == null) {
            return Damage;
        }
        for (int i = 0; i < hitting.effects.Count; i++)
        {
            Effect effect = hitting.effects[i];
            if(effect.type == EffectType.Damage) {
                if(effect.amountType == AmountType.Flat) {
                    Damage -= effect.amount;
                } else {
                    Damage /= (1 + (effect.amount/100));
                }
            }
        }
        if(Damage < 0) {
            Damage = 0;
        }
        return Damage;
    }

    public static void StartGame() {
        if(lastRested != null) {
            Player.GetInstance().transform.position = (Vector2)lastRested.transform.position + lastRested.spawnPoint;
        }
    }


}

public static class ListExtensions
{
    public static void TrimEffects(this List<Effect> effects)
    {
        //Find all duplicate effects and merge them, then remove the duplicates
        for (int i = 0; i < effects.Count; i++)
        {
            for (int j = i + 1; j < effects.Count; j++)
            {
                if (effects[i].GetType() == effects[j].GetType())
                {
                    effects[i].amount += effects[j].amount;
                    effects.RemoveAt(j);
                    j--;
                }
            }
        }
    }

    public static string ToDebugString(this List<Vector2> vectors) {
        string s = "";
        foreach (Vector2 vector in vectors)
        {
            s += vector.ToString() + "\n";
        }
        return s;
    }

    public static string ToDebugString(this List<Effect> effects) {
        string s = "";
        foreach (Effect effect in effects)
        {
            s += effect.ToString() + "\n";
        }
        return s;
    }
}