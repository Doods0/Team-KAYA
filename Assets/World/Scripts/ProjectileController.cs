using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    // now EnemyBullet can be called from anywhere
    // It must be assigned its id by the shooter
    // It can be either pre-configured and prefabbed in terms of texture and stats
    // Or just assign these stats from the shooter (preferred)

    [Header("Stats")]
    public float speed;
    public int damage;
    public float lifetime;
    public bool isEnemy;

    [Header("Spins")]
    public float torque;
    public bool spins;

    [Header("Session")]
    public SpriteRenderer renderer;
    public Vector3 moveDirection;
    public string id;

    private Rigidbody2D rigidBody;
    private float lifetimeCounter;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        moveDirection.Normalize();

        if (spins) rigidBody.AddTorque(torque);
    }

    private void Update() => lifetimeCounter += Time.deltaTime;

    private void FixedUpdate()
    {
        // now the projectile can be coded to follow!
        rigidBody.linearVelocityX = moveDirection.x * speed * GameManager.instance.timeScale;
        rigidBody.linearVelocityY = moveDirection.y * speed * GameManager.instance.timeScale;

        if (lifetimeCounter >= lifetime / GameManager.instance.timeScale) Despawn();
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isEnemy)
        {
            PlayerStats playerController = other.gameObject.GetComponent<PlayerStats>();
            if (playerController == null) return;

            playerController.TakeDamage(damage);
        }
        else
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            if (enemy == null) return;

            enemy.TakeDamage(damage);
        }

        Despawn();
    }
    
    private void Despawn()
    {
        GameManager.instance.AddInPool(id, gameObject);
        lifetimeCounter = 0;
        gameObject.SetActive(false);
    }
}
