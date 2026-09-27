using Unity.Mathematics;
using UnityEngine;

public class RangedEnemyController : EnemyController
{
    [Header("Ranged Enemy")]
    [SerializeField] private float speedWhileAiming;
    [SerializeField] private float range;
    [SerializeField] private float preferredRange;
    [SerializeField] private float minimumRange;
    [SerializeField] private float timeToCharge;
    [Header("Projectile")]
    [Header("Settings")]
    [SerializeField] private ProjectileController projectile;
    [SerializeField] private string projectileId;
    [SerializeField] private bool projectileSpins;
    [SerializeField] private int projectileTorque;
    [Header("Stats")]
    [SerializeField] private ProjectileStats projectileStats;

    bool lockedOnPlayer = false;
    // Used to "charge up" shots.
    float timeSpentCharging = 0;

    public override void FixedUpdate()
    {
        float distanceFromPlayer = ToPlayer().magnitude;

        // Only start aiming when we're in the preferred range.
        if (distanceFromPlayer >= preferredRange * 0.9
            && distanceFromPlayer <= preferredRange * 1.1)
        {
            lockedOnPlayer = true;
        }
        else if (distanceFromPlayer > range || distanceFromPlayer < minimumRange)
        {
            lockedOnPlayer = false;
        }

        Vector3 moveVector = ToPlayer().normalized;

        if (!lockedOnPlayer)
        {
            moveVector *= speed;

            timeSpentCharging = 0;
        }
        else
        {
            moveVector *= speedWhileAiming;

            timeSpentCharging += Time.deltaTime;

            if (timeSpentCharging >= timeToCharge / GameManager.instance.timeScale)
            {
                timeSpentCharging = 0;

                IPoolable bulletInstance = GameManager.instance.GetFromPool(projectileId) ?? Instantiate(projectile);
                if (!(bulletInstance is ProjectileController controller)) return;
                controller.FireProjectile(projectileStats,
                    transform.position,
                    moveVector.normalized,
                    projectileId,
                    projectileSpins,
                    projectileTorque,
                    true);
            }
        }

        if (distanceFromPlayer > preferredRange * 1.1f) { }
        // Moving backwards to remain the preferred range.
        else if (distanceFromPlayer < preferredRange * .9f) moveVector *= -1;
        // Stop moving while in the preferred range.
        else moveVector *= 0;

        rigidbody.linearVelocity = moveVector * GameManager.instance.timeScale;

        ReactToKnockback();
    }
}
