using Unity.VisualScripting;
using UnityEngine;

public class RangedEnemyController : EnemyController
{
    [SerializeField] private float speedWhileAiming;
    [SerializeField] private int rangedDamage;
    [SerializeField] private float range;
    [SerializeField] private float preferredRange;
    [SerializeField] private float minimumRange;
    [SerializeField] private float timeToCharge;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Object bullet;
    [SerializeField] private string bulletId;
    [SerializeField] private Sprite bulletTexture;

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
        } else {
            moveVector *= speedWhileAiming;

            timeSpentCharging += Time.deltaTime;

            if (timeSpentCharging >= timeToCharge)
            {
                timeSpentCharging = 0;

                Object bulletInstance = GameManager.instance.GetFromPool(bulletId);

                if (bulletInstance is null)
                {
                    bulletInstance = Instantiate(bullet,
                        transform.position,
                        Quaternion.identity);
                }

                ProjectileController bulletScript = bulletInstance.GetComponent<ProjectileController>();
                if (bulletScript == null)
                {
                    print("EnemyBullet script not found");
                    return;
                }

                bulletScript.speed = bulletSpeed;
                bulletScript.damage = rangedDamage;
                bulletScript.moveDirection = moveVector.normalized;
                bulletScript.torque = Random.Range(-10, 11);
                bulletScript.id = bulletId;
                bulletScript.renderer.sprite = bulletTexture;
                bulletScript.isEnemy = true;
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
