using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public sealed class GameController : MonoBehaviour
{
    #region Singleton
    public static GameController instance;

    public static JObject names;
    public static JArray firstNames, surNames;

    void Awake()
    {
        //Remove all listeners from OnBonfireUpdate.
        OnBonfireUpdate = null;
        UpdateSprites();
        names = JObject.Parse(GameController.LoadResourceTextfile("NamesLarger.json"));
        firstNames = names.GetValue("firstName") as JArray;
        surNames = names.GetValue("lastName") as JArray;
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

    public Bonfire lastRested;
    public void SetLastRested(Bonfire bonfire)
    {
        lastRested = bonfire;
        OnBonfireUpdate?.Invoke();
    }
    public Player player;
    public GameObject itemPrefab;

    public Sprite[] meleeSprites;
    public Sprite[] rangedSprites;
    public Sprite[] rangedProjectileSprites;
    public Sprite[] magicSprites;
    public Sprite[] magicProjectileSprites;

    public Sprite[] lightArmorSprites;
    public Sprite[] mediumArmorSprites;
    public Sprite[] heavyArmorSprites;
    public Sprite[][] armorSprites = new Sprite[3][];

    public static UnityAction OnBonfireUpdate;
    public static UnityAction Tick;
    public static float TickTimer;

    private void Start()
    {
        if (!player)
        {
            player = GameObject.FindObjectOfType<Player>();
        }
    }


    private void Update()
    {
        if (TickTimer <= 0)
        {
            Tick?.Invoke();
            TickTimer = 1f;
        }
        else
        {
            TickTimer -= Time.deltaTime;
        }
    }


    private void UpdateSprites()
    {
        meleeSprites = Resources.LoadAll<Sprite>("Sprites/Weapons/Melee/");
        rangedSprites = Resources.LoadAll<Sprite>("Sprites/Weapons/Ranged/");
        rangedProjectileSprites = Resources.LoadAll<Sprite>("Sprites/Weapons/Projectiles/Ranged/");
        magicSprites = Resources.LoadAll<Sprite>("Sprites/Weapons/Magic/");
        magicProjectileSprites = Resources.LoadAll<Sprite>("Sprites/Weapons/Projectiles/Magic/");

        lightArmorSprites = Resources.LoadAll<Sprite>("Sprites/Armor/Light/");
        mediumArmorSprites = Resources.LoadAll<Sprite>("Sprites/Armor/Medium/");
        heavyArmorSprites = Resources.LoadAll<Sprite>("Sprites/Armor/Heavy/");
        armorSprites[0] = lightArmorSprites;
        armorSprites[1] = mediumArmorSprites;
        armorSprites[2] = heavyArmorSprites;

    }


    public static Rarity GetRarity(System.Random random = null)
    {
        int rarity;
        if (random == null)
        {
            rarity = Random.Range(0, 100);
        }
        else
        {
            rarity = random.Next(0, 100);
        }
        if (rarity < 50)
        {
            return Rarity.Common;
        }
        else if (rarity < 75)
        {
            return Rarity.Uncommon;
        }
        else if (rarity < 90)
        {
            return Rarity.Rare;
        }
        else if (rarity < 99)
        {
            return Rarity.Epic;
        }
        else
        {
            return Rarity.Legendary;
        }
    }

    int GetPoints(Rarity rarity, int level)
    {
        // float k1 = 0.001f;
        // float k2 = -1;
        // float k3 = -0.6f;
        // int points = (int)Mathf.Round((level * Mathf.Pow((int)rarity + 1, k3) * Mathf.Pow(10, level * k1) + 5 * Mathf.Pow((int)rarity + 1, k2)));
        int points = Mathf.RoundToInt(level * ((int)rarity + 1 / 5 * 1.25f) + ((int)rarity + 1) * 5);
        return points;
    }

    public Weapon CreateWeapon(Rarity rarity, int level, int weaponType)
    {
        Weapon weapon = null;
        switch (weaponType)
        {
            case 0:
                weapon = CreateMelee(rarity, level);
                break;
            case 1:
                weapon = CreateRanged(rarity, level);
                break;
            case 2:
            // return CreateMagic(rarity, level);
            // break;
            default:
                weapon = CreateMelee(rarity, level);
                break;
        }
        DamageCalculation(weapon);
        return weapon;
    }
    public Weapon CreateWeapon(Rarity rarity, int level)
    {
        int weaponType = Random.Range(0, 3);
        Weapon weapon = null;
        switch (weaponType)
        {
            case 0:
                weapon = CreateMelee(rarity, level);
                break;
            case 1:
                weapon = CreateRanged(rarity, level);
                break;
            case 2:
            // return CreateMagic(rarity, level);
            // break;
            default:
                weapon = CreateMelee(rarity, level);
                break;
        }
        weapon = DamageCalculation(weapon);
        return weapon;
    }
    public Weapon CreateWeapon(Rarity rarity, int level, System.Random random)
    {
        int weaponType = random.Next(0, 3);
        Weapon weapon = null;
        switch (weaponType)
        {
            case 0:
                weapon = CreateMelee(rarity, level, random);
                break;
            case 1:
                weapon = CreateRanged(rarity, level, random);
                break;
            case 2:
            // return CreateMagic(rarity, level, random);
            // break;
            default:
                weapon = CreateMelee(rarity, level, random);
                break;
        }
        weapon = DamageCalculation(weapon);
        return weapon;
    }

    public Weapon DamageCalculation(Weapon weapon)
    {
        weapon.damage = 0;
        //For each effect in weapon.effects where effect.type == EffectType.Damage, add effect.amount to weapon.damage
        foreach (Effect effect in weapon.effects.ToList().FindAll(effect => effect.type == EffectType.Damage).ToList())
        {
            if (effect.amountType == AmountType.Flat)
            {
                weapon.damage += effect.amount;
            }
            else
            {
                weapon.damage = (int)(effect.amount * 0.01f * weapon.damage);
            }
        }
        //For each effect in weapon.effects where effect.type == EffectType.HealthRegen, add effect.amount to weapon.heal
        foreach (Effect effect in weapon.effects.ToList().FindAll(effect => effect.specificType == Effector.HealthRegen).ToList())
        {
            if (effect.amountType == AmountType.Flat)
            {
                weapon.heal += effect.amount;
            }
            else
            {
                weapon.heal += (int)(effect.amount * 0.01f * weapon.damage);
            }
        }
        return weapon;
    }
    public Melee CreateMelee(Rarity rarity, int level)
    {
        int points = GetPoints(rarity, level);
        int maxEffects = (int)rarity + 2;
        Melee melee = ScriptableObject.CreateInstance<Melee>();
        List<Effect> effects = new List<Effect>();
        effects.Add(Effect.CreateEffect(Effector.Damage, melee, (int)points / 4, AmountType.Flat));
        points -= (int)points / 4;
        effects.AddRange(Effect.CreateEffects(points, maxEffects, melee));
        effects.TrimEffects();
        int selected = Random.Range(0, meleeSprites.Length);
        string selectedEffect = "";
        if (effects.Count > 1)
        {
            selectedEffect = $" of {effects[1].name}";
        }
        melee.itemName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase($"{rarity.ToString()} {meleeSprites[selected].name}{selectedEffect}");
        melee.name = melee.itemName;
        melee.effects = effects.ToArray();
        if (points > 100) { melee.cost = (int)Mathf.Round(points * 0.1f) * 10; } else { melee.cost = points; }
        melee.level = level;
        melee.sprite = meleeSprites[selected];
        melee.maxCombo = Random.Range(2, 4);
        melee.animationSet = (AnimationSet)Random.Range(0, 3);
        melee.rarity = rarity;
        return melee;
    }
    public Melee CreateMelee(Rarity rarity, int level, System.Random random)
    {
        int points = GetPoints(rarity, level);
        int maxEffects = (int)rarity + 2;
        Melee melee = ScriptableObject.CreateInstance<Melee>();
        List<Effect> effects = new List<Effect>();
        effects.Add(Effect.CreateEffect(Effector.Damage, melee, (int)points / 4, AmountType.Flat));
        points -= (int)points / 4;
        effects.AddRange(Effect.CreateEffects(points, maxEffects, melee));
        effects.TrimEffects();
        int selected = random.Next(0, meleeSprites.Length);
        string selectedEffect = "";
        if (effects.Count > 1)
        {
            selectedEffect = $" of {effects[1].name}";
        }
        melee.itemName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase($"{rarity.ToString()} {meleeSprites[selected].name}{selectedEffect}");
        melee.name = melee.itemName;
        melee.effects = effects.ToArray();
        if (points > 100) { melee.cost = (int)Mathf.Round(points * 0.1f) * 10; } else { melee.cost = points; }
        melee.level = level;
        melee.sprite = meleeSprites[selected];
        melee.maxCombo = Random.Range(2, 4);
        melee.animationSet = (AnimationSet)Random.Range(0, 3);
        melee.rarity = rarity;
        return melee;
    }

    public Ranged CreateRanged(Rarity rarity, int level)
    {
        int points = GetPoints(rarity, level);
        int maxEffects = (int)rarity + 2;
        Ranged ranged = ScriptableObject.CreateInstance<Ranged>();
        List<Effect> effects = new List<Effect>();
        effects.Add(Effect.CreateEffect(Effector.Damage, ranged, (int)points / 4, AmountType.Flat));
        points -= (int)points / 4;
        effects.AddRange(Effect.CreateEffects(points, maxEffects, ranged));
        effects.TrimEffects();
        int selected = Random.Range(0, rangedSprites.Length);
        string selectedEffect = "";
        if (effects.Count > 1)
        {
            selectedEffect = $" of {effects[1].name}";
        }
        ranged.itemName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase($"{rarity.ToString()} {rangedSprites[selected].name}{selectedEffect}");
        ranged.name = ranged.itemName;
        ranged.effects = effects.ToArray();
        if (points > 100) { ranged.cost = (int)Mathf.Round(points * 0.1f) * 10; } else { ranged.cost = points; }
        ranged.level = level;
        ranged.sprite = rangedSprites[selected];
        ranged.maxCombo = Random.Range(2, 4);
        ranged.animationSet = (AnimationSet)Random.Range(0, 3);
        ranged.rarity = rarity;

        ranged.projectileSprite = rangedProjectileSprites[0];
        // rangedProjectileSprites.Length;

        return ranged;
    }
    public Ranged CreateRanged(Rarity rarity, int level, System.Random random)
    {
        int points = GetPoints(rarity, level);
        int maxEffects = (int)rarity + 2;
        Ranged ranged = ScriptableObject.CreateInstance<Ranged>();
        List<Effect> effects = new List<Effect>();
        effects.Add(Effect.CreateEffect(Effector.Damage, ranged, (int)points / 4, AmountType.Flat));
        points -= (int)points / 4;
        effects.AddRange(Effect.CreateEffects(points, maxEffects, ranged));
        effects.TrimEffects();
        int selected = random.Next(0, rangedSprites.Length);
        string selectedEffect = "";
        if (effects.Count > 1)
        {
            selectedEffect = $" of {effects[1].name}";
        }
        ranged.itemName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase($"{rarity.ToString()} {rangedSprites[selected].name}{selectedEffect}");
        ranged.name = ranged.itemName;
        ranged.effects = effects.ToArray();
        if (points > 100) { ranged.cost = (int)Mathf.Round(points * 0.1f) * 10; } else { ranged.cost = points; }
        ranged.level = level;
        ranged.sprite = rangedSprites[selected];
        ranged.maxCombo = Random.Range(2, 4);
        ranged.animationSet = (AnimationSet)Random.Range(0, 3);
        ranged.rarity = rarity;

        ranged.projectileSprite = rangedProjectileSprites[0];
        // rangedProjectileSprites.Length;

        return ranged;
    }


    public Armor CreateArmor(Rarity rarity, int level)
    {
        int points = GetPoints(rarity, level);
        int maxEffects = (int)rarity + 2;
        Armor armor = ScriptableObject.CreateInstance<Armor>();
        List<Effect> effects = new List<Effect>();
        effects.Add(Effect.CreateEffect(Effector.Damage, armor, (int)points / 4, AmountType.Flat));
        points -= (int)points / 4;
        effects.AddRange(Effect.CreateEffects(points, maxEffects, armor));
        effects.TrimEffects();
        int selected = Random.Range(0, armorSprites[(int)armor.armorType].Length);
        string selectedEffect = "";
        if (effects.Count > 1)
        {
            selectedEffect = $" of {effects[1].name}";
        }
        armor.itemName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase($"{rarity.ToString()} {armorSprites[(int)armor.armorType][selected].name}{selectedEffect}");
        armor.name = armor.itemName;
        armor.effects = effects.ToArray();
        if (points > 100) { armor.cost = (int)Mathf.Round(points * 0.1f) * 10; } else { armor.cost = points; }
        armor.level = level;
        armor.sprite = armorSprites[(int)armor.armorType][selected];
        armor.rarity = rarity;
        //Get all effects of defense type and add them to the armor variable
        armor.defense = 0;
        foreach (Effect effect in effects.FindAll(effect => effect.specificType == Effector.Damage).ToList())
        {
            if (effect.amountType == AmountType.Flat)
            {
                armor.defense += effect.amount;
            }
            else
            {
                armor.defense += (int)(effect.amount * 0.01f * armor.defense);
            }
        }
        return armor;
    }
    public Armor CreateArmor(Rarity rarity, int level, System.Random random)
    {
        int points = GetPoints(rarity, level);
        int maxEffects = (int)rarity + 2;
        Armor armor = ScriptableObject.CreateInstance<Armor>();
        List<Effect> effects = new List<Effect>();
        effects.Add(Effect.CreateEffect(Effector.Damage, armor, (int)points / 4, AmountType.Flat));
        points -= (int)points / 4;
        effects.AddRange(Effect.CreateEffects(points, maxEffects, armor));
        effects.TrimEffects();
        int selected = random.Next(0, armorSprites[(int)armor.armorType].Length);
        string selectedEffect = "";
        if (effects.Count > 1)
        {
            selectedEffect = $" of {effects[1].name}";
        }
        armor.itemName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase($"{rarity.ToString()} {armorSprites[(int)armor.armorType][selected].name}{selectedEffect}");
        armor.name = armor.itemName;
        armor.effects = effects.ToArray();
        if (points > 100) { armor.cost = (int)Mathf.Round(points * 0.1f) * 10; } else { armor.cost = points; }
        armor.level = level;
        armor.sprite = armorSprites[(int)armor.armorType][selected];
        armor.rarity = rarity;
        //Get all effects of defense type and add them to the armor variable
        armor.defense = 0;
        foreach (Effect effect in effects.FindAll(effect => effect.specificType == Effector.Damage).ToList())
        {
            if (effect.amountType == AmountType.Flat)
            {
                armor.defense += effect.amount;
            }
            else
            {
                armor.defense += (int)(effect.amount * 0.01f * armor.defense);
            }
        }
        return armor;
    }


    public static int CalculateDamage(Weapon weapon, Armor hitting, out int heal)
    {
        int Damage = weapon.damage;
        heal = 0;
        if (weapon == null)
        {
            return Damage;
        }
        if (hitting == null)
        {
            return Damage;
        }
        Damage -= hitting.defense;
        if (Damage < 0)
        {
            Damage = 0;
        }
        return Damage;
    }

    public static HumanoidName GetRandomName()
    {
        HumanoidName returnName = new HumanoidName();
        returnName.firstName = firstNames[Random.Range(0, firstNames.Count)].ToString();
        List<string> surnames = new List<string>();
        for (var i = 0; i < Random.Range(1, 3); i++)
        {
            surnames.Add(surNames[Random.Range(0, surNames.Count)].ToString());
        }
        returnName.surNames = surnames.ToArray();
        returnName.fullName = returnName.firstName;
        for (var i = 0; i < surnames.Count; i++)
        {
            returnName.fullName += " " + surnames[i];
        }
        return returnName;
    }

    public static string LoadResourceTextfile(string path)
    {

        string filePath = "Info/" + path.Replace(".json", "");

        TextAsset targetFile = Resources.Load<TextAsset>(filePath);

        return targetFile.text;
    }

    public void StartGame()
    {
        Debug.Log("Starting game");
        if (player.equippedArmor != null || player.equippedWeapon != null || player.inventory.items.Count > 0)
        {

        }
        else
        {
            player.gameObject.SetActive(true);
            player.enabled = true;

            //Give player items based on class
            switch (player._class)
            {
                case Humanoid.HumanoidClass.Warrior:
                default:
                    player.EquipWeapon(this.CreateWeapon(Rarity.Common, 1, 0));
                    player.EquipArmor(this.CreateArmor(Rarity.Common, 1));
                    break;
                case Humanoid.HumanoidClass.Ranger:
                    player.EquipWeapon(this.CreateWeapon(Rarity.Common, 1, 1));
                    player.EquipArmor(this.CreateArmor(Rarity.Common, 1));
                    break;
                case Humanoid.HumanoidClass.Mage:
                    player.EquipWeapon(this.CreateWeapon(Rarity.Common, 1, 2));
                    player.EquipArmor(this.CreateArmor(Rarity.Common, 1));
                    break;
            }
        }

        if (this.lastRested != null)
        {
            Debug.Log("Loading last rested");
            player.transform.position = (Vector2)lastRested.transform.position + lastRested.spawnPoint;
        }
    }

    public void EndGame()
    {
        Debug.Log("Ending game");
        player.enabled = false;
        player.gameObject.SetActive(false);
        lastRested = null;
    }

    public static void ReturnToStart()
    {
        SceneManager.UnloadSceneAsync("Main Game");
        SceneManager.LoadScene("Start");
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

    public static string ToDebugString(this List<Vector2> vectors)
    {
        string s = "";
        foreach (Vector2 vector in vectors)
        {
            s += vector.ToString() + "\n";
        }
        return s;
    }

    public static string ToDebugString(this List<Effect> effects)
    {
        string s = "";
        foreach (Effect effect in effects)
        {
            s += effect.ToString() + "\n";
        }
        return s;
    }
}