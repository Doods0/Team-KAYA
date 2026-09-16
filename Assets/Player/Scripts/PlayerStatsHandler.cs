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

[Serializable]
public class WeaponsBuffs
{
    public MeleeStats meleeBuffs = new();
    public ThrowStats throwBuffs = new();
    // Any new stats of custom weapons will have to be added here
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

#region Player Stats
[Serializable]
public class PlayerStats
{
    [Header("Physical")]
    public float walkspeed;
    public float knockbackOnShockwave;
    public float cooldownOnShockwave;
    public int health;
    public int maxHealth;

    [Header("Economy")]
    public float pickupRange;
    public float speedupDropInterval;
    public float slowdownDropInterval;

    public static PlayerStats operator +(PlayerStats a, PlayerStats b)
    {
        return new PlayerStats
        {
            walkspeed = a.walkspeed + b.walkspeed,
            knockbackOnShockwave = a.knockbackOnShockwave + b.knockbackOnShockwave,
            cooldownOnShockwave = a.cooldownOnShockwave + b.cooldownOnShockwave,
            health = a.health + b.health,
            maxHealth = a.maxHealth + b.maxHealth,
            pickupRange = a.pickupRange + b.pickupRange,
            speedupDropInterval = a.speedupDropInterval + b.speedupDropInterval,
            slowdownDropInterval = a.slowdownDropInterval + b.slowdownDropInterval
        };
    }

    public static PlayerStats operator *(PlayerStats a, PlayerStats b)
    {
        return new PlayerStats
        {
            walkspeed = a.walkspeed * (1f + b.walkspeed),
            knockbackOnShockwave = a.knockbackOnShockwave * (1f + b.knockbackOnShockwave),
            cooldownOnShockwave = a.cooldownOnShockwave * (1f + b.cooldownOnShockwave),
            health = (int)(a.health * (1f + b.health)), // Cast back to int
            maxHealth = (int)(a.maxHealth * (1f + b.maxHealth)), // Cast back to int
            pickupRange = a.pickupRange * (1f + b.pickupRange),
            speedupDropInterval = a.speedupDropInterval * (1f + b.speedupDropInterval),
            slowdownDropInterval = a.slowdownDropInterval * (1f + b.slowdownDropInterval)
        };
    }
}
#endregion

public class PlayerStatsHandler : MonoBehaviour
{
    [Header("Inventory")]
    public HeavyWeaponSO heavyWeapon;
    public LightWeaponSO lightWeapon;

    // Upgradable PlayerStats Below !!
    public PlayerStats stats;

    [Header("Settings")]
    [SerializeField] private float shockwaveTime;
    [SerializeField] private float shockwaveRange;
    private readonly float shopTriggerBuffer = 0.2f;

    [Header("Utils")]
    public PlayerAnimator animator;
    [SerializeField] private HUDManager HUD;

    [Header("Sounds")]
    [Header("Damage")]
    public AudioClip damageSound;
    public AudioClip shockwaveSound;
    [Header("Pickups")]
    public AudioClip pointSound;
    public AudioClip speedupSound;
    public AudioClip slowdownSound;
    public AudioClip shopSound;

    [Header("Session")]
    [HideInInspector] public bool isImmune = false;
    [HideInInspector] public int points;
    [HideInInspector] public LocalWeaponsData localWeaponsData;
    // Will be passed to all weapon types and values and will be added to values here (the one below)
    [HideInInspector] public WeaponsBuffs weaponBuffs;
    [HideInInspector] public List<UpgradeSO> upgrades;
    [HideInInspector] public ContactFilter2D enemyFilter;

    private float currentShopTriggerPoint = Mathf.Infinity;
    private float currentCooldown;
    private void Update() 
    {
        currentCooldown = Mathf.Max(0f, currentCooldown - Time.deltaTime);
        if (Math.Abs(GameManager.instance.timeScale - currentShopTriggerPoint) <= shopTriggerBuffer) TriggerShop();
    }

    private void Awake()
    {
        HUD.UpdateHealth(stats.health, stats.maxHealth);

        enemyFilter = new ContactFilter2D
        {
            layerMask = GameUtils.instance.enemyLayer,
            useLayerMask = true,
            useTriggers = false
        };

        localWeaponsData.enemyFilter = enemyFilter;
    }

    public void UpdateAllStats() // Run it all the time, and for "once" upgrades, run those upon purchase 
    {
        foreach (UpgradeSO upgrade in upgrades)
        {
            if (upgrade.frequency != Frequency.Update) return;
            upgrade.ApplyEffect(stats, weaponBuffs);
        }
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
            cooldown = heavyWeapon.meleeStats.slashCooldown;
            heavyWeapon.Slash(localWeaponsData, weaponBuffs);
        }
        else
        {
            if (!isThrowMode)
            {
                cooldown = lightWeapon.meleeStats.slashCooldown;
                lightWeapon.Slash(localWeaponsData, weaponBuffs);
            }
            else
            {
                cooldown = lightWeapon.throwStats.throwCooldown;
                lightWeapon.Throw(weaponBuffs);
            }
        }

        currentCooldown = cooldown;
        animator.TriggerWeaponAnimation(weaponInUse, otherWeapon, isThrowMode);
    }

    public void TakeDamage(int damageTaken)
    {
        if (isImmune || damageTaken == 0) return;

        stats.health = Mathf.Clamp(stats.health - damageTaken, 0, stats.maxHealth);
        animator.OnDamageTaken(shockwaveTime);

        HUD.UpdateHealth(stats.health, stats.maxHealth);

        // Pause game
        GameManager.instance.isTimeBypassed = true;
        GameManager.instance.timeScale = 0;
        isImmune = true;

        GameUtils.instance.audioSource.PlayOneShot(damageSound);

        List<Collider2D> hitsBuffer = new();
        int hitCount = Physics2D.OverlapCircle(transform.position, shockwaveRange, enemyFilter, hitsBuffer);

        IEnumerator ResumeGameAfterDelay()
        {
            yield return new WaitForSecondsRealtime(shockwaveTime);

            while (GameManager.instance.isGamePaused) yield return null;

            GameManager.instance.isTimeBypassed = false;

            // Iterate through nearby enemies
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D col = hitsBuffer[i];

                if (col.TryGetComponent(out EnemyController controller))
                {
                    Vector2 direction = (controller.rigidbody.transform.position - transform.position).normalized;

                    controller.ApplyKnockback(direction * stats.knockbackOnShockwave);
                }
            }

            GameUtils.instance.audioSource.PlayOneShot(shockwaveSound);

            if (stats.health <= 0)
            {
                GameManager.instance.TriggerGameOver();
                Destroy(gameObject);
            };

            yield return new WaitForSecondsRealtime(stats.cooldownOnShockwave);

            isImmune = false;
        }

        StartCoroutine(ResumeGameAfterDelay());
    }

    public void CollectPickup(PickupType type)
    {
        if (type is PickupType.Point)
        {
            points++;
            HUD.CollectPoint();
            GameUtils.instance.audioSource.PlayOneShot(pointSound);
        }
        else if (type is PickupType.SpeedUp)
        {
            GameManager.instance.runtimeScale += stats.speedupDropInterval / GameManager.instance.timeScale;
            GameUtils.instance.audioSource.PlayOneShot(speedupSound);
        }
        else if (type is PickupType.SlowDown)
        {
            GameManager.instance.runtimeScale -= stats.slowdownDropInterval * GameManager.instance.timeScale;
            GameUtils.instance.audioSource.PlayOneShot(slowdownSound);
        }
        else if (type is PickupType.Shop)
        {
            AssignShopTriggerPoint();
            GameUtils.instance.audioSource.PlayOneShot(shopSound);
        }
    }

    public void AssignShopTriggerPoint()
    {
        if (currentShopTriggerPoint != Mathf.Infinity) return;

        float shopTriggerPoint = UnityEngine.Random.Range(
            GameManager.instance.minTimeScale + shopTriggerBuffer,
            GameManager.instance.maxTimeScale - shopTriggerBuffer);
        currentShopTriggerPoint = shopTriggerPoint;

        HUD.AssignShopTriggerPoint(currentShopTriggerPoint);
    }

    private void TriggerShop()
    {
        GameManager.instance.isGamePaused = true;
        GameManager.instance.isTimeBypassed = true;
        GameManager.instance.timeScale = 0;
        GameManager.instance.runtimeScale = 1;

        // Play some shop sound idk

        StartCoroutine(HUD.ShowShopMenu());
        currentShopTriggerPoint = Mathf.Infinity;
        HUD.AssignShopTriggerPoint(currentShopTriggerPoint); // if infinity, hide the shop point completely
    }

}
