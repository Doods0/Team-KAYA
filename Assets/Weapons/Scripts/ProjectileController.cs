using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    // Stats can be either assigned here or by the shooter (shooter overrides)

    [Header("Stats")]
    public float speed;
    public int damage;
    public float knockback;
    public float lifetime;
    public bool isEnemy;

    [Header("Spins")]
    public float torque;
    public bool spins;

    [Header("Session")]
    public SpriteRenderer renderer;
    public string id;

    [HideInInspector]
    public Vector3 moveDirection;

    private Rigidbody2D rigidBody;
    private float lifetimeCounter;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        if (spins)
            rigidBody.AddTorque(torque);
    }

    private void Update() => lifetimeCounter += Time.deltaTime;

    private void FixedUpdate()
    {
        // now the projectile can be coded to follow!
        rigidBody.linearVelocityX = moveDirection.x * speed * GameManager.instance.timeScale;
        rigidBody.linearVelocityY = moveDirection.y * speed * GameManager.instance.timeScale;

        if (lifetimeCounter >= lifetime / GameManager.instance.timeScale)
            Despawn();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnemy)
        {
            PlayerStats playerController = other.gameObject.GetComponent<PlayerStats>();
            if (playerController == null)
                return;

            playerController.TakeDamage(damage);
            Despawn();
        }
        else
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            if (enemy == null)
                return;

            enemy.TakeDamage(damage);

            Vector2 direction = (
                enemy.rigidbody.transform.position - GameUtils.instance.playerPosition
            ).normalized;
            enemy.ApplyKnockback(direction * knockback);

            Despawn();
        }
    }

    private void Despawn()
    {
        GameManager.instance.AddInPool(id, gameObject);
        lifetimeCounter = 0;
        gameObject.SetActive(false);
    }
}
