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

    Rigidbody2D rigidBody;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        moveDirection.Normalize();

        rigidBody.linearVelocityX = moveDirection.x * speed * GameManager.instance.timeScale;
        rigidBody.linearVelocityY = moveDirection.y * speed * GameManager.instance.timeScale;
        if (spins) rigidBody.AddTorque(torque);
    }

    void FixedUpdate()
    {
        // Replace with a lifetime property
        if ((GameUtils.instance.playerPosition - transform.position).magnitude > 40) Despawn();
        
    }


    void OnCollisionEnter2D(Collision2D other)
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
    
    void Despawn()
    {
        GameManager.instance.AddInPool(id, gameObject);
        gameObject.SetActive(false);
    }
}
