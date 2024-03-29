using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Linq;

[System.Serializable]
public struct HumanoidName
{
    public string fullName;
    public string firstName;
    public string[] surNames;
}
[RequireComponent(typeof(Rigidbody2D))]
public class Humanoid : MonoBehaviour
{
    [Header("Main Settings")]
    public HumanoidName Name = new HumanoidName();
    public int hearts = 3;
    public int maxHealth = 100;
    public int health = 100;
    public int regenSpeed = 2;
    public float movementSpeed = 7f;
    public float sprintspeed = 1.3f;
    public float dodgeMultiplier = 7f;
    public float dodgeCooldown = 0.3f;
    public int team = 0;
    public HumanoidClass _class;
    [Header("Script Variables")]
    public bool dodging = false, sprinting = false;
    public Rigidbody2D rb;
    public TextMeshProUGUI nameText;
    public Weapon equippedWeapon;
    public WeaponManifesto weaponManifesto;
    public Armor equippedArmor;
    public Vector2 movementDirection = new Vector2(0, 0);
    public float dodgeTimer = 0f;
    public List<Humanoid> targetedBy = new List<Humanoid>();
    public UnityAction onDamage;


    private void OnEnable()
    {
        GameController.Tick += Regen;
    }
    private void OnDisable()
    {
        GameController.Tick -= Regen;
    }

    // private void OnValidate() {
    //     if(!weaponManifesto) {
    //         // GameObject gam = Instantiate(Resources.Load<GameObject>("Prefabs/WeaponManifesto"), transform);
    //         // weaponManifesto = gam.GetComponent<WeaponManifesto>();
    //         // weaponManifesto.owner = this;
    //     }
    //     Start();
    // }

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        weaponManifesto = GetComponentInChildren<WeaponManifesto>();
        if (!weaponManifesto)
        {
            GameObject gam = Instantiate(Resources.Load<GameObject>("Prefabs/WeaponManifesto"), transform);
            weaponManifesto = gam.GetComponent<WeaponManifesto>();
            weaponManifesto.owner = this;
        }
        if (Name.fullName == "")
        {
            Name = GameController.GetRandomName();
        }
        nameText.text = Name.fullName;
        health = maxHealth;
    }

    public void FixedUpdate()
    {
        Move();
        if (dodgeTimer > 0)
        {
            dodgeTimer -= Time.fixedDeltaTime;
        }
    }

    void Move()
    {
        if (CanMove())
        {
            if (sprinting)
            {
                rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * sprintspeed * Time.fixedDeltaTime);
                sprinting = false;
            }
            else if (dodging)
            {
                dodgeTimer = dodgeCooldown;
                dodging = false;
            }
            else if (dodgeTimer > 0.25f)
            {
                rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * Time.fixedDeltaTime * dodgeMultiplier);
            }
            else
            {
                rb.MovePosition(rb.position + movementDirection.normalized * movementSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            movementDirection = Vector2.zero;
        }
    }

    public bool CanMove()
    {
        if(equippedWeapon == null) {
            return true;
        }
        bool canMove = false;
        if (equippedWeapon is Ranged or Magic)
        {
            canMove = true;
            if (weaponManifesto.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
            {
                canMove = false;
            }
        }
        else
        {
            if (weaponManifesto.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
            {
                canMove = true;
            }
        }
        return canMove;
    }

    protected void TurnWeapon(Vector2 _transform, Vector2 target, float _time, float extraDegrees = 0f)
    {
        if (target == Vector2.zero)
        {
            return;
        }
        Vector2 direction = target - _transform;

        Quaternion rotation = Quaternion.Slerp(weaponManifesto.transform.rotation, Quaternion.LookRotation(Vector3.forward, direction), 0.1f * _time * 100);
        // Debug.Log(rotation);
        weaponManifesto.transform.rotation = rotation;
        Quaternion rotation2 = Quaternion.Slerp(weaponManifesto.container.transform.rotation, Quaternion.LookRotation(Vector3.forward, (target - (Vector2)weaponManifesto.container.transform.position)) * Quaternion.Euler(0, 0, extraDegrees), 0.1f * _time * 100);
        // Debug.Log(rotation2);
        // if(extraDegrees != 0f) {
        //     rotation2 *= Quaternion.Euler(0, 0, extraDegrees);
        // }
        weaponManifesto.container.transform.rotation = rotation2;
    }

    protected virtual void Attack()
    {
        if (equippedWeapon != null)
        {
            weaponManifesto.Attack();
        }
    }

    public int TakeDamage(Weapon weapon, Humanoid attacker)
    {
        if (dodgeTimer > 0f || health == 0)
        {
            return 0;
        }
        onDamage?.Invoke();
        this.health -= GameController.CalculateDamage(weapon, equippedArmor, out int heal);
        attacker.Heal(heal);
        if (health <= 0)
        {
            health = 0;
            Die();
        }
        else if (this is AI2)
        {
            AI2 ai = this as AI2;
            if(ai.team != attacker.team) {
                ai.target = attacker;
                ai.timer = ai.timerMax * 1.5f;
            }
            if (ai.agressive || ai.defaultState == AI2.AIState.Patrol || ai.defaultState == AI2.AIState.Chase || ai.state == AI2.AIState.Chase)
            {
                ai.state = AI2.AIState.Chase;
                ai.patrolTimer = 0f;
            }
            else
            {
                ai.state = AI2.AIState.Flee;
            }
        }
        return heal;
    }
    public void Attacked(Humanoid attacker)
    {
        if (this is AI2)
        {
            AI2 ai = this as AI2;
            if(ai.team != attacker.team) {
                ai.target = attacker;
                ai.timer = ai.timerMax * 1.5f;
            }
            if (ai.agressive || ai.defaultState == AI2.AIState.Patrol || ai.defaultState == AI2.AIState.Chase || ai.state == AI2.AIState.Chase)
            {
                ai.state = AI2.AIState.Chase;
                ai.patrolTimer = 0f;
            }
            else
            {
                ai.state = AI2.AIState.Flee;
            }
        }
    }

    public void Heal(int heal)
    {
        this.health += heal;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        onDamage?.Invoke();
    }

    public void Regen()
    {
        if (health <= 0)
        {
            return;
        }
        if (this is AI2)
        {
            AI2 ai = this as AI2;
            if (ai.target || ai.state == AI2.AIState.Dead)
            {
                return;
            }
        }
        if (targetedBy.Count > 0)
        {
            return;
        }
        if (health == maxHealth)
        {
            return;
        }
        if (!equippedArmor)
        {
            health += regenSpeed;
            return;
        }
        Effect effect;
        float toRegen = regenSpeed;
        for (int i = 0; i < equippedArmor.effects.Length; i++)
        {
            effect = equippedArmor.effects[i];
            if (effect.specificType == Effector.HealthRegen)
            {
                if (effect.amountType == AmountType.Percentage)
                {
                    toRegen *= (1f + ((float)effect.amount / 100));
                }
                else
                {
                    toRegen += regenSpeed + effect.amount;
                }
            }
        }
        health += Mathf.RoundToInt(toRegen);
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public virtual void Die()
    {
        if (hearts <= 0)
        {
            Debug.LogWarning("Dead");
        }
    }

    public Weapon EquipWeapon(Weapon weapon)
    {
        Weapon tempWeapon = equippedWeapon;
        this.equippedWeapon = weapon;
        this.weaponManifesto.UpdateWeapon();
        return tempWeapon;
    }

    public Weapon UnEquipWeapon()
    {
        Weapon tempWeapon = equippedWeapon;
        this.equippedWeapon = null;
        this.weaponManifesto.UpdateWeapon();
        return tempWeapon;
    }

    public Armor EquipArmor(Armor armor)
    {
        Armor tempArmor = equippedArmor;
        this.equippedArmor = armor;
        maxHealth = 100;
        foreach (Effect effect in equippedArmor.effects.ToList().FindAll(effect => effect.specificType == Effector.HealthBoost))
        {
            if (effect.amountType == AmountType.Percentage)
            {
                maxHealth = (int)((float)maxHealth * (1f + ((float)effect.amount / 100)));
                continue;
            }
            else
            {
                maxHealth += effect.amount;
            }
        }
        movementSpeed = 7;
        foreach (Effect effect in equippedArmor.effects.ToList().FindAll(effect => effect.specificType == Effector.Speed))
        {
            if (effect.amountType == AmountType.Percentage)
            {
                movementSpeed = ((float)movementSpeed * (1f + ((float)effect.amount / 100)));
                continue;
            }
            else
            {
                movementSpeed += effect.amount;
            }
        }
        return tempArmor;
    }

    public Armor UnEquipArmor()
    {
        Armor tempArmor = equippedArmor;
        this.equippedArmor = null;
        maxHealth = 100;
        return tempArmor;
    }


    public override string ToString()
    {
        return $"Name: {Name}, Movement Speed: {movementSpeed}, Sprint Speed: {sprintspeed}, Dodge Multiplier: {dodgeMultiplier}, Hearts: {hearts}\n Equipped Weapon: {equippedWeapon}, Equipped Armor: {equippedArmor}";
    }


    private void OnMouseDown()
    {
        if (DebugMenu.selecting)
        {
            DebugMenu.GetInstance().OpenSelectMenu(this);
        }
    }

    public enum HumanoidClass
    {
        Warrior,
        Ranger,
        Mage,
    }
}