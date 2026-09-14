using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSO : ScriptableObject
{
    [Header("Visuals")]
    public Sprite texture;
    public float inUseGripOffset;
    public float concealedGripOffset;

    [Header("Slash")]
    [Header("Stats")]
    public MeleeStats meleeStats;
    [Header("Animations and Sounds")]
    public AnimationClip[] slashAnimations; // need to exist already in the AnimationController
    public AudioClip[] slashSounds;

    // Function must return a float (Cooldown value)
    // code slashing here as it's common between both types
    public virtual void Slash(LocalWeaponsData weaponMemory, WeaponsBuffs buffs)
    {
        MeleeStats localMeleeStats = meleeStats * buffs.meleeBuffs;

        List<Collider2D> hitBuffer = weaponMemory.hitsBuffer;
        Vector3 playerPos = GameUtils.instance.playerPosition;

        hitBuffer.Clear();
        int count = Physics2D.OverlapCircle(playerPos, localMeleeStats.slashRadius, weaponMemory.enemyFilter, hitBuffer);

        for (int i = 0; i < count; i++)
        {
            Vector2 directionToEnemy = (hitBuffer[i].transform.position - playerPos).normalized;

            // Calculate angle between aim direction and enemy
            float angle = Vector2.Angle(CameraController.cursorDirectionVector, directionToEnemy);

            if (angle > localMeleeStats.slashAngle / 2f) continue;

            // Assuming enemy controller is the health handler
            if (hitBuffer[i].TryGetComponent<EnemyController>(out var target))
            {
                target.TakeDamage(localMeleeStats.slashDamage);
                Vector2 direction = (target.rigidbody.transform.position - GameUtils.instance.playerPosition).normalized;
                target.ApplyKnockback(direction * localMeleeStats.slashKnockback);
            }
        }
    }
}

[Serializable]
public class MeleeStats
{
    public int slashDamage;
    public float slashKnockback;
    public float slashCooldown;
    public float slashRadius;
    public float slashAngle;

    public static MeleeStats operator +(MeleeStats a, MeleeStats b)
    {
        return new MeleeStats
        {
            slashDamage = a.slashDamage + b.slashDamage,
            slashKnockback = a.slashKnockback + b.slashKnockback,
            slashCooldown = a.slashCooldown + b.slashCooldown,
            slashRadius = a.slashRadius + b.slashRadius,
            slashAngle = a.slashAngle + b.slashAngle
        };
    }

    public static MeleeStats operator *(MeleeStats a, MeleeStats b)
    {
        return new MeleeStats
        {
            slashDamage = (int)(a.slashDamage * (1f + b.slashDamage)),
            slashKnockback = a.slashKnockback * (1f + b.slashKnockback),
            slashCooldown = a.slashCooldown * (1f + b.slashCooldown),
            slashRadius = a.slashRadius * (1f + b.slashRadius),
            slashAngle = a.slashAngle * (1f + b.slashAngle)
        };
    }
}