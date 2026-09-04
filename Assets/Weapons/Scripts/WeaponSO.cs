using System.Collections.Generic;
using UnityEngine;

public class WeaponSO : ScriptableObject
{
    [Header("Visuals")]
    public Sprite texture;
    public float inUseGripOffset;
    public float concealedGripOffset;

    [Header("Slash")]
    public float slashDamage;
    public float slashCooldown;
    public float slashRadius;
    public float slashAngle;

    // Function must return a float (Cooldown value)
    // code slashing here as it's common between both types
    public virtual float Slash(LocalWeaponsData weaponMemory)
    {
        List<Collider2D> hitBuffer = weaponMemory.hitsBuffer;
        Vector3 playerPos = GameUtils.instance.playerPosition;

        hitBuffer.Clear();
        int count = Physics2D.OverlapCircle(playerPos, slashRadius, weaponMemory.enemyFilter, hitBuffer);

        for (int i = 0; i < count; i++)
        {
            Vector2 directionToEnemy = (hitBuffer[i].transform.position - playerPos).normalized;

            // Calculate angle between aim direction and enemy
            float angle = Vector2.Angle(GameUtils.instance.cursorWorldLocation, directionToEnemy);

            if (angle <= slashAngle / 2f)
            {
                // Assuming enemy controller is the health handler
                if (hitBuffer[i].TryGetComponent<EnemyController>(out var target))
                {
                    target.TakeDamage((int)slashDamage);
                }
            }
        }

        return slashCooldown;
    }
}
