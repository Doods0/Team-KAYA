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
    public readonly List<Collider2D> hitsBuffer = new List<Collider2D>();
    public ContactFilter2D enemyFilter = new ContactFilter2D();
    // Moved the enemy filter from here to the mono script so it's reusable for knockback
}
#endregion

public class PlayerStats : MonoBehaviour
{
    [Header("Inventory")]
    public HeavyWeaponSO heavyWeapon;
    public LightWeaponSO lightWeapon;

    [Header("Stats")]
    public float walkspeed;
    public float knockbackOnDamaged;
    public float cooldownOnDamaged;
    public int health;
    public int maxHealth;
    public bool isImmune = false;

    [Header("Settings")]
    [SerializeField] private float damageImpactTime;
    [SerializeField] private float damageImpactRange;

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
        if (currentCooldown > 0) return;

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

        if (withHeavy) cooldown = heavyWeapon.Slash(localWeaponsData);
        else
        {
            if (!isThrowMode) cooldown = lightWeapon.Slash(localWeaponsData);
            else cooldown = lightWeapon.Throw();
        }

        animator.TriggerWeaponAnimation(weaponInUse, otherWeapon);
        currentCooldown = cooldown;
    }

    public void TakeDamage(int damageTaken)
    {
        if (isImmune) return;

        health -= damageTaken;

        HUD.UpdateHealth(health, maxHealth);

        if (health <= 0)
        {
            GameManager.instance.TriggerGameOver();
            return;
        }

        // Pause game
        GameManager.instance.isTimeBypassed = true;
        Time.timeScale = 0;
        isImmune = true;

        GameUtils.instance.audioSource.PlayOneShot(shockwave);

        List<Collider2D> hitsBuffer = new List<Collider2D>();
        int hitCount = Physics2D.OverlapCircle(transform.position, damageImpactRange, enemyFilter, hitsBuffer);

        // Iterate through nearby enemies
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D col = hitsBuffer[i];

            if (col.TryGetComponent<EnemyController>(out EnemyController controller))
            {
                Vector2 direction = (controller.rigidbody.transform.position - transform.position).normalized;

                controller.ApplyKnockback(direction * knockbackOnDamaged);
            }
        }

        IEnumerator ResumeGameAfterDelay()
        {
            yield return new WaitForSecondsRealtime(damageImpactTime);

            GameManager.instance.isTimeBypassed = false;

            yield return new WaitForSecondsRealtime(cooldownOnDamaged);

            isImmune = false;
        }

        StartCoroutine(ResumeGameAfterDelay());
    }

}
