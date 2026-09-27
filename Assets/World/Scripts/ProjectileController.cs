using System;
using UnityEngine;

public class ProjectileController : MonoBehaviour, IPoolable
{
    [Header("Stats")]
    [HideInInspector] public ProjectileStats localStats = new();

    [Header("Session")]
    private Rigidbody2D rigidBody;
    private Vector3 moveDirection;
    private string localId;
    private bool isEnemy;

    private float lifetimeCounter;

    private void Update() => lifetimeCounter += Time.deltaTime;
    private void Awake() => rigidBody = GetComponent<Rigidbody2D>();

    public void FireProjectile
        (ProjectileStats stats,
        Vector3 startPosition,
        Vector3 direction,
        string id,
        bool spins = false,
        int torque = 0,
        bool isEnemyProjectile = false)
    {
        gameObject.SetActive(true);
        if (spins) rigidBody.AddTorque(torque);

        transform.position = startPosition;
        float directionAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, directionAngle);
        transform.localScale = Vector3.one * (1 + stats.sizeIncrease);

        localStats = stats;

        moveDirection = direction;
        isEnemy = isEnemyProjectile;
        localId = id;
    }

    private void FixedUpdate()
    {
        // now the projectile can be coded to follow!
        rigidBody.linearVelocityX = moveDirection.x * localStats.speed * GameManager.instance.timeScale;
        rigidBody.linearVelocityY = moveDirection.y * localStats.speed * GameManager.instance.timeScale;

        if (lifetimeCounter >= localStats.lifetime / GameManager.instance.timeScale) Despawn();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnemy)
        {
            PlayerStatsHandler playerController = other.gameObject.GetComponent<PlayerStatsHandler>();
            if (playerController == null) return;

            playerController.TakeDamage((int)localStats.damage);
            Despawn();
        }
        else
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            if (enemy == null) return;

            enemy.TakeDamage((int)localStats.damage);

            Vector2 direction = (enemy.rigidbody.transform.position - GameUtils.instance.playerPosition).normalized;
            enemy.ApplyKnockback(direction * localStats.knockback);

            Despawn();
        }
    }
    
    private void Despawn()
    {
        GameManager.instance.AddInPool(localId, this);
        lifetimeCounter = 0;
        gameObject.SetActive(false);
    }
}

[Serializable]
public class ProjectileStats
{
    public float speed;
    public float damage;
    public float knockback;
    public float lifetime;
    public float sizeIncrease;

    public static ProjectileStats operator +(ProjectileStats a, ProjectileStats b)
    {
        return new ProjectileStats
        {
            speed = a.speed + b.speed,
            damage = a.damage + b.damage,
            knockback = a.knockback + b.knockback,
            lifetime = a.lifetime + b.lifetime,
            sizeIncrease = a.sizeIncrease + b.sizeIncrease
        };
    }

    public static ProjectileStats operator *(ProjectileStats a, ProjectileStats b)
    {
        return new ProjectileStats
        {
            speed = a.speed * (1f + b.speed),
            damage = a.damage * (1f + b.damage),
            knockback = a.knockback * (1f + b.knockback),
            lifetime = a.lifetime * (1f + b.lifetime),
            sizeIncrease = a.sizeIncrease * (1f + b.sizeIncrease)
        };
    }
}