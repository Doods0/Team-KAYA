using System.Collections.Generic;
using UnityEngine;

public class ExploderEnemyController : EnemyController
{
    public bool ticking;
    public float timeToExplode;
    public int explosionDamageToEnemies;
    public int explosionDamageToPlayer;
    public float explosionRange;
    public float startTickingRange;
    public float explosionKnockback;
    public float speedWhileTicking;
    float timeSpentTicking = 0f;

    // Update is called once per frame
    public override void FixedUpdate()
    {
        rigidbody.linearVelocity = ToPlayer().normalized * ScaledSpeed();

        if (ToPlayer().magnitude <= startTickingRange)
            StartTicking();
        if (ticking)
            timeSpentTicking += Time.deltaTime * GameManager.instance.timeScale;

        if (timeSpentTicking > timeToExplode)
            Explode();
    }

    public override void ApplyKnockback(Vector2 impulse)
    {
        base.ApplyKnockback(impulse);

        StartTicking();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (health <= 0)
            Explode();
    }

    void StartTicking()
    {
        if (ticking)
            return;

        ticking = true;
        timeSpentTicking = 0f;
        speed = speedWhileTicking;
    }

    void Explode()
    {
        ContactFilter2D filter = new()
        {
            layerMask = GameUtils.instance.enemyLayer + GameUtils.instance.playerLayer,
        };

        List<Collider2D> hitsBuffer = new();
        int hitCount = Physics2D.OverlapCircle(
            transform.position,
            explosionRange,
            filter,
            hitsBuffer
        );

        // Iterate through nearby enemies + the player.
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D col = hitsBuffer[i];

            if (col.TryGetComponent(out EnemyController controller))
            {
                Vector2 direction = (
                    controller.rigidbody.transform.position - transform.position
                ).normalized;

                controller.ApplyKnockback(direction * explosionKnockback);
                controller.TakeDamage(explosionDamageToEnemies);
            }
            else if (
                col.TryGetComponent(out PlayerStats playerStats)
                && col.TryGetComponent(out PlayerController playerController)
            )
            {
                Vector2 direction = ToPlayer().normalized;

                playerController.ApplyKnockback(direction * explosionKnockback);
                playerStats.TakeDamage(explosionDamageToPlayer);
            }
        }

        base.TakeDamage(maxHealth);
    }
}
