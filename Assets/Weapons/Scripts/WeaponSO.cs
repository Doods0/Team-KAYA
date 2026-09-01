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
    public virtual float Slash(LocalWeaponsData weaponMemory, SpatialTracker spatialData) 
    {
        List<Collider2D> hitBuffer = weaponMemory.hitBuffer;
        Vector3 playerPos = spatialData.playerPosition;

        hitBuffer.Clear();
        int count = Physics2D.OverlapCircle(playerPos, slashRadius, weaponMemory.contactFilter, hitBuffer);

        for (int i = 0; i < count; i++)
        {
            Vector2 directionToEnemy = (hitBuffer[i].transform.position - playerPos).normalized;

            // Calculate angle between aim direction and enemy
            float angle = Vector2.Angle(spatialData.cursorDirection, directionToEnemy);

            if (angle <= slashAngle / 2f)
            {
                if (hitBuffer[i].TryGetComponent<EnemyController>(out var target)) // Assuming enemy controller is the health handler
                {
                    target.TakeDamage((int)slashDamage);
                }
            }
        }

        return slashCooldown; 
    }
}
