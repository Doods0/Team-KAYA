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
    public ProjectileController projectile;
    public string projectileId;
    public bool projectileSpins;
    public int projectileTorque;

    public virtual void Throw(LocalWeaponsData weaponsData, WeaponsBuffs buffs)
    {
        ThrowStats localThrowStats = throwStats * buffs.lightThrowBuffs;
        weaponsData.lightThrows++;
        weaponsData.lightMeleeSlashes = 0;
        weaponsData.heavyMeleeSlashes = 0;

        Vector3 currentPos = GameUtils.instance.playerPosition;
        Vector3 direction = CameraController.cursorDirectionVector;
        // PROJECTILE MUST FACE UP IN ART

        IPoolable proj = GameManager.instance.GetFromPool(projectileId) ?? Instantiate(projectile);
        if (!(proj is ProjectileController controller)) return;

        controller.FireProjectile
            (throwStats.projectileStats,
            currentPos, direction,
            projectileId,
            projectileSpins,
            projectileTorque);
    }
}

[Serializable]
public class ThrowStats
{
    public float throwDamage;
    public float throwKnockback;
    public float throwCooldown;
    public ProjectileStats projectileStats;

    public static ThrowStats operator +(ThrowStats a, ThrowStats b)
    {
        return new ThrowStats
        {
            throwDamage = a.throwDamage + b.throwDamage,
            throwKnockback = a.throwKnockback + b.throwKnockback,
            throwCooldown = a.throwCooldown + b.throwCooldown,
            projectileStats = a.projectileStats + b.projectileStats
        };
    }

    public static ThrowStats operator *(ThrowStats a, ThrowStats b)
    {
        return new ThrowStats
        {
            throwDamage = (int)(a.throwDamage * (1f + b.throwDamage)),
            throwKnockback = a.throwKnockback * (1f + b.throwKnockback),
            throwCooldown = a.throwCooldown * (1f + b.throwCooldown),
            projectileStats = a.projectileStats * b.projectileStats
        };
    }
}