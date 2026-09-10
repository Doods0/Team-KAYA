using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Heavy and light weapons inherit from WeaponSO
// WeaponSO holds the common funcs among all weapons, including slash considering both weapons can melee slash
// This script is meant to hold the weapons and manage their damage and any active upgrades and buffs

// Things that are used by the weapons' SOs (because SOs can't store things)
// Add to this when you want to ship weapons more data without needing to modify the base functions' parameters
#region Weapon's Memory
[Serializable]
public class LocalWeaponsData
{
    public readonly List<Collider2D> hitsBuffer = new();
    public ContactFilter2D enemyFilter = new();
    // Moved the enemy filter from here to the mono script so it's reusable for knockback
}
#endregion

#region Pickups
[Serializable]
public struct PickupChance
{
    public GameObject pickup;
    public int weight;
    public string id;
}
#endregion

public class PlayerStats : MonoBehaviour
{
    [Header("Inventory")]
    public HeavyWeaponSO heavyWeapon;
    public LightWeaponSO lightWeapon;

    [Header("Stats")]
    [Header("Physical")]
    public float walkspeed;
    public float knockbackOnShockwave;
    public float cooldownOnShockwave;
    public int health;
    public int maxHealth;
    public bool isImmune = false;
    [Header("Economy")]
    public float pickupRange;
    public bool hasShopAccess;
    public int points;
    public float speedupDropInterval;
    public float slowdownDropInterval;

    [Header("Settings")]
    [SerializeField] private float shockwaveTime;
    [SerializeField] private float shockwaveRange;

    [Header("Utils")]
    [SerializeField] private PlayerAnimator animator;
    [SerializeField] private HUDManager HUD;

    [Header("Sounds")]
    public AudioClip shockwave;

    [HideInInspector] public LocalWeaponsData localWeaponsData;
    [HideInInspector] public ContactFilter2D enemyFilter;

    private float currentCooldown;
    private void Update() => currentCooldown = Mathf.Max(0f, currentCooldown - Time.deltaTime);
    // Or fixedDeltaTime? should it change according to time speed?

    private void Awake()
    {
        HUD.UpdateHealth(health, maxHealth);

        enemyFilter = new ContactFilter2D
        {
            layerMask = GameUtils.instance.enemyLayer,
            useLayerMask = true,
            useTriggers = false
        };

        localWeaponsData.enemyFilter = enemyFilter;
    }

    public void Attack(bool withHeavy, bool isThrowMode)
    {
        if (currentCooldown > 0 || GameManager.instance.timeScale == 0) return;

        WeaponSO weaponInUse;
        WeaponSO otherWeapon;

        if (withHeavy)
        {
            weaponInUse = heavyWeapon;
            otherWeapon = lightWeapon;
        }
        else
        {
            weaponInUse = lightWeapon;
            otherWeapon = heavyWeapon;
        }

        float cooldown;

        if (withHeavy)
        {
            cooldown = heavyWeapon.slashCooldown;
            heavyWeapon.Slash(localWeaponsData);
        }
        else
        {
            if (!isThrowMode)
            {
                cooldown = heavyWeapon.slashCooldown;
                lightWeapon.Slash(localWeaponsData);
            }
            else
            {
                cooldown = lightWeapon.throwCooldown;
                lightWeapon.Throw();
            }
        }

        currentCooldown = cooldown;
        animator.TriggerWeaponAnimation(weaponInUse, otherWeapon, isThrowMode);
    }

    public void TakeDamage(int damageTaken)
    {
        if (isImmune || damageTaken == 0) return;

        health = Mathf.Clamp(health - damageTaken, 0, maxHealth);

        HUD.UpdateHealth(health, maxHealth);

        if (health <= 0)
        {
            GameManager.instance.TriggerGameOver();
            return;
        }

        // Pause game
        GameManager.instance.isTimeBypassed = true;
        GameManager.instance.timeScale = 0;
        isImmune = true;

        GameUtils.instance.audioSource.PlayOneShot(shockwave);

        List<Collider2D> hitsBuffer = new();
        int hitCount = Physics2D.OverlapCircle(transform.position, shockwaveRange, enemyFilter, hitsBuffer);

        IEnumerator ResumeGameAfterDelay()
        {
            yield return new WaitForSecondsRealtime(shockwaveTime);

            GameManager.instance.isTimeBypassed = false;

            // Iterate through nearby enemies
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D col = hitsBuffer[i];

                if (col.TryGetComponent(out EnemyController controller))
                {
                    Vector2 direction = (controller.rigidbody.transform.position - transform.position).normalized;

                    controller.ApplyKnockback(direction * knockbackOnShockwave);
                }
            }

            yield return new WaitForSecondsRealtime(cooldownOnShockwave);

            isImmune = false;
        }

        StartCoroutine(ResumeGameAfterDelay());
    }
}
