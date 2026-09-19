using System;
using UnityEngine;

// To create a special light weapon, please inherit from this SO

[CreateAssetMenu(menuName = "Weapons/Basic Light Weapon")]
public class LightWeaponSO : WeaponSO
{
    [Header("Throw")]
    [Header("Stats")]
    public ThrowStats throwStats;

    [Header("Animations and Sounds")]
    public AnimationClip[] throwAnimations; // need to exist already in the AnimationController
    public AudioClip[] throwSounds;

    [Header("Projectile")]
    public GameObject projectile;
    public Sprite projectileTexture;
    public string projectileId;
    public bool projectileSpins;

    public virtual void Throw(LocalWeaponsData weaponsData, WeaponsBuffs buffs)
    {
        ThrowStats localThrowStats = throwStats * buffs.lightThrowBuffs;
        weaponsData.lightThrows++;
        weaponsData.lightMeleeSlashes = 0;
        weaponsData.heavyMeleeSlashes = 0;

        Vector3 currentPos = GameUtils.instance.playerPosition;
        Vector3 direction = CameraController.cursorDirectionVector;
        float angleToDirection = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        // PROJECTILE MUST FACE UP IN ART

        GameObject proj = GameManager.instance.GetFromPool(projectileId);
        if (!proj) proj = Instantiate(projectile);

        proj.SetActive(true);
        proj.transform.position = currentPos;
        proj.transform.rotation = Quaternion.Euler(0f, 0f, angleToDirection);
        proj.transform.localScale = Vector3.one * localThrowStats.projectileSize;

        ProjectileController controller = proj.GetComponent<ProjectileController>();

        controller.id = projectileId;
        controller.damage = (int)localThrowStats.throwDamage;
        controller.speed = localThrowStats.projectileSpeed;
        controller.knockback = localThrowStats.throwKnockback;
        controller.moveDirection = direction;
        controller.lifetime = localThrowStats.projectileLifetime;
        controller.spins = projectileSpins;
        controller.renderer.sprite = projectileTexture;
        controller.isEnemy = false;
    }
}

[Serializable]
public class ThrowStats
{
    public float throwDamage;
    public float throwKnockback;
    public float throwCooldown;
    public float projectileLifetime;
    public int projectileSpeed;
    public float projectileSize;

    public static ThrowStats operator +(ThrowStats a, ThrowStats b)
    {
        return new ThrowStats
        {
            throwDamage = a.throwDamage + b.throwDamage,
            throwKnockback = a.throwKnockback + b.throwKnockback,
            throwCooldown = a.throwCooldown + b.throwCooldown,
            projectileLifetime = a.projectileLifetime + b.projectileLifetime,
            projectileSpeed = a.projectileSpeed + b.projectileSpeed,
            projectileSize = a.projectileSize + b.projectileSize
        };
    }

    public static ThrowStats operator *(ThrowStats a, ThrowStats b)
    {
        return new ThrowStats
        {
            throwDamage = (int)(a.throwDamage * (1f + b.throwDamage)),
            throwKnockback = a.throwKnockback * (1f + b.throwKnockback),
            throwCooldown = a.throwCooldown * (1f + b.throwCooldown),
            projectileLifetime = a.projectileLifetime * (1f + b.projectileLifetime),
            projectileSpeed = (int)(a.projectileSpeed * (1f + b.projectileSpeed)),
            projectileSize = a.projectileSize * (1f + b.projectileSize)
        };
    }
}